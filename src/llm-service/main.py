import os
import json
import pika
import uuid
import logging
from datetime import datetime
from enum import Enum, IntEnum
import time
import requests
from typing import Dict, List, Optional, Any, Tuple
from dotenv import load_dotenv
from minio import Minio
from minio.error import S3Error
import io

#
# ДА, ЭТО ОЧЕНЬ ПЛОХОЙ КОД, Я ЗНАЮ, НО ЗАТО БЫСТРО
#


load_dotenv()

logging.basicConfig(
    level=logging.DEBUG,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

class TestGenerationStatus(str, Enum):
    QUEUED = "Queued"
    IN_PROGRESS = "InProgress"
    FAILED = "Failed"
    SUCCEEDED = "Succeeded"

class ResponseType(IntEnum):
    STATUS = 0
    RESULT = 1
    RECOMMENDATION = 2

RABBITMQ_URL = os.getenv("RABBITMQ_URL", "amqp://rmuser:rmpassword@rabbitmq:5672")
TASK_QUEUE_NAME = os.getenv("TASK_QUEUE_NAME", "task_queue")
RESPONSE_QUEUE_NAME = os.getenv("RESPONSE_QUEUE_NAME", "response_queue")
TASK_ROUTING_KEY = os.getenv("TASK_ROUTING_KEY", "llm.tasks")
RESPONSE_ROUTING_KEY = os.getenv("RESPONSE_ROUTING_KEY", "llm.response")
EXCHANGE_NAME = os.getenv("EXCHANGE_NAME", "llm.services")

OPENROUTER_API_URL = os.getenv("OPENROUTER_API_URL", "https://openrouter.ai/api/v1")
OPENROUTER_API_KEY = os.getenv("OPENROUTER_API_KEY", "")
OPENROUTER_MODEL = os.getenv("OPENROUTER_MODEL", "google/gemini-2.5-pro-exp-03-25:free")

MINIO_ENDPOINT = os.getenv("MINIO_ENDPOINT", "minio:9000")
MINIO_ACCESS_KEY = os.getenv("MINIO_ACCESS_KEY", "")
MINIO_SECRET_KEY = os.getenv("MINIO_SECRET_KEY", "")
MINIO_BUCKET_NAME = os.getenv("MINIO_BUCKET_NAME", "huipetrovich")

class EnumEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj, Enum):
            return int(obj)
        return super().default(obj)

class LlmService:
    def __init__(self):
        self.connection = None
        self.channel = None
        self.minio_client = None
        self.connect_to_rabbitmq()
        self.connect_to_minio()

    def connect_to_rabbitmq(self):
        """Установка соединения с RabbitMQ и настройка каналов"""
        retry_count = 0
        max_retries = 5
        
        while retry_count < max_retries:
            try:
                logger.info(f"Попытка подключения к RabbitMQ: {RABBITMQ_URL}")
                parameters = pika.URLParameters(RABBITMQ_URL)
                self.connection = pika.BlockingConnection(parameters)
                self.channel = self.connection.channel()
                
                self.channel.exchange_declare(
                    exchange=EXCHANGE_NAME,
                    exchange_type='direct',
                    durable=True
                )
                
                self.channel.queue_declare(queue=TASK_QUEUE_NAME, durable=True)
                self.channel.queue_declare(queue=RESPONSE_QUEUE_NAME, durable=True)
                
                self.channel.queue_bind(
                    exchange=EXCHANGE_NAME,
                    queue=TASK_QUEUE_NAME,
                    routing_key=TASK_ROUTING_KEY
                )
                self.channel.queue_bind(
                    exchange=EXCHANGE_NAME,
                    queue=RESPONSE_QUEUE_NAME,
                    routing_key=RESPONSE_ROUTING_KEY
                )
                
                logger.info("Успешное подключение к RabbitMQ")
                return
            except Exception as e:
                retry_count += 1
                logger.error(f"Ошибка подключения к RabbitMQ: {e}")
                if retry_count < max_retries:
                    wait_time = 5 * retry_count
                    logger.info(f"Повторная попытка через {wait_time} секунд...")
                    time.sleep(wait_time)
                else:
                    logger.error("Превышено максимальное количество попыток подключения.")
                    raise

    def connect_to_minio(self):
        """Установка соединения с MinIO"""
        retry_count = 0
        max_retries = 5
        
        while retry_count < max_retries:
            try:
                logger.info(f"Попытка подключения к MinIO: {MINIO_ENDPOINT}")
                
                self.minio_client = Minio(
                    endpoint=MINIO_ENDPOINT,
                    access_key=MINIO_ACCESS_KEY,
                    secret_key=MINIO_SECRET_KEY,
                    secure=False
                )
                
                if not self.minio_client.bucket_exists(MINIO_BUCKET_NAME):
                    logger.warning(f"Корзина {MINIO_BUCKET_NAME} не существует, создаем...")
                    self.minio_client.make_bucket(MINIO_BUCKET_NAME)
                
                logger.info("Успешное подключение к MinIO")
                return
            except Exception as e:
                retry_count += 1
                logger.error(f"Ошибка подключения к MinIO: {e}")
                if retry_count < max_retries:
                    wait_time = 5 * retry_count
                    logger.info(f"Повторная попытка через {wait_time} секунд...")
                    time.sleep(wait_time)
                else:
                    logger.error("Превышено максимальное количество попыток подключения к MinIO.")
                    self.minio_client = None
    
    def download_file(self, file_name: str) -> Tuple[Optional[bytes], Optional[str]]:
        """
        Скачивает файл из MinIO по его имени.
        Возвращает содержимое файла и его MIME-тип.
        """
        if self.minio_client is None:
            logger.error("Нет подключения к MinIO, невозможно скачать файл")
            return None, None
        
        try:
            logger.info(f"Попытка скачать файл '{file_name}' из корзины {MINIO_BUCKET_NAME}")
            
            response = self.minio_client.get_object(
                bucket_name=MINIO_BUCKET_NAME,
                object_name=file_name
            )
            
            file_data = response.read()
            content_type = response.headers.get('Content-Type', 'application/octet-stream')
            
            response.close()
            
            logger.info(f"Файл '{file_name}' успешно скачан. Размер: {len(file_data)} байт. Тип: {content_type}")
            return file_data, content_type
        except S3Error as e:
            logger.error(f"Ошибка S3 при скачивании файла '{file_name}': {e}")
            return None, None
        except Exception as e:
            logger.error(f"Неожиданная ошибка при скачивании файла '{file_name}': {e}")
            return None, None

    def extract_text_from_file(self, file_data: bytes, content_type: str) -> Optional[str]:
        """
        Извлекает текстовое содержимое из файла на основе его типа.
        Поддерживаются текстовые файлы и PDF.
        """
        try:
            if content_type.startswith('text/'):
                return file_data.decode('utf-8')
            
            elif content_type == 'application/pdf':
                try:
                    from PyPDF2 import PdfReader
                    reader = PdfReader(io.BytesIO(file_data))
                    text = ""
                    for page in reader.pages:
                        text += page.extract_text() + "\n"
                    return text
                except ImportError:
                    logger.warning("Библиотека PyPDF2 не установлена. Невозможно извлечь текст из PDF.")
                    return None
            
            else:
                logger.warning(f"Неподдерживаемый тип файла: {content_type}")
                return None
        except Exception as e:
            logger.error(f"Ошибка при извлечении текста из файла: {e}")
            return None

    def send_status_update(self, request_id: str, status: TestGenerationStatus):
        """Отправка обновления статуса в очередь ответов"""
        try:
            status_response = {
                "Type": int(ResponseType.STATUS),
                "Body": {
                    "RequestId": request_id,
                    "Status": status
                }
            }
            
            json_message = json.dumps(status_response)
            logger.debug(f"Отправляемое JSON сообщение (статус): {json_message}")
            
            self.channel.basic_publish(
                exchange=EXCHANGE_NAME,
                routing_key=RESPONSE_ROUTING_KEY,
                body=json_message.encode('utf-8'),
                properties=pika.BasicProperties(
                    delivery_mode=2,
                    content_type='application/json'
                )
            )
            logger.info(f"Отправлено обновление статуса: {status} для запроса {request_id}")
        except Exception as e:
            logger.error(f"Ошибка при отправке обновления статуса: {e}")

    def send_test_result(self, request_id: str, test_entity: Dict):
        """Отправка результата генерации теста в очередь ответов"""
        try:
            result_response = {
                "Type": int(ResponseType.RESULT),
                "Body": {
                    "RequestId": request_id,
                    "TestEntity": test_entity
                }
            }
            
            json_message = json.dumps(result_response)
            logger.debug(f"Отправляемое JSON сообщение (результат): {json_message}")
            
            self.channel.basic_publish(
                exchange=EXCHANGE_NAME,
                routing_key=RESPONSE_ROUTING_KEY,
                body=json_message.encode('utf-8'),
                properties=pika.BasicProperties(
                    delivery_mode=2,
                    content_type='application/json'
                )
            )
            logger.info(f"Отправлен результат генерации для запроса {request_id}")
        except Exception as e:
            logger.error(f"Ошибка при отправке результата: {e}")
            raise

    def send_recommendation(self, request_id: str, test_result_id: str, recommendation: str):
        """Отправка рекомендации в очередь ответов"""
        try:
            recommendation_response = {
                "Type": int(ResponseType.RECOMMENDATION),
                "Body": {
                    "TestResultId": test_result_id,
                    "Recommendation": recommendation
                }
            }
            
            json_message = json.dumps(recommendation_response)
            logger.debug(f"Отправляемое JSON сообщение (рекомендация): {json_message}")
            
            self.channel.basic_publish(
                exchange=EXCHANGE_NAME,
                routing_key=RESPONSE_ROUTING_KEY,
                body=json_message.encode('utf-8'),
                properties=pika.BasicProperties(
                    delivery_mode=2,
                    content_type='application/json'
                )
            )
            logger.info(f"Отправлена рекомендация для запроса {request_id}")
        except Exception as e:
            logger.error(f"Ошибка при отправке рекомендации: {e}")
            raise

    def generate_test(self, request_id: str, test_description: str, file_name: str) -> Dict:
        """
        Генерация теста с помощью LLM на основе описания.
        Использует модель OpenRouter API для генерации содержимого теста.
        Если указано имя файла, скачивает его из MinIO и использует в промпте.
        """
        try:
            logger.info(f"Генерация теста для запроса {request_id}")
            
            file_content = None
            if file_name:
                file_data, content_type = self.download_file(file_name)
                if file_data:
                    file_content = self.extract_text_from_file(file_data, content_type)
                    if not file_content:
                        logger.warning(f"Не удалось извлечь текст из файла '{file_name}'. Генерация будет выполнена без учета содержимого файла.")
            
            prompt = self._create_test_prompt(test_description, file_name, file_content)
            
            test_content = self._call_openrouter_api(prompt)
            
            if not test_content:
                logger.error(f"Ошибка: не получены данные от LLM для запроса {request_id}")
                raise Exception("Не удалось получить ответ от модели LLM")
  
            created_at = datetime.now().isoformat()
            questions = self._parse_llm_response_to_questions(test_content, created_at)
            
            return questions
        except Exception as e:
            logger.error(f"Ошибка при генерации теста: {e}")
            raise
    
    def _create_test_prompt(self, test_description: str, file_name: str, file_content: Optional[str] = None) -> str:
        """
        Формирует промпт для запроса к LLM на основе описания теста.
        Если доступно содержимое файла, включает его в промпт.
        """
        if file_content:
            return f"""
            Создай тестовый набор по теме "{test_description}" на основе материала из файла "{file_name}".
            
            Содержимое файла:
            ----------------
            {file_content}
            ----------------
            
            Сформируй 10 вопросов на основе содержимого файла. Для каждого вопроса укажи:
            1. Текст вопроса (основанный на информации из файла)
            2. Краткое описание
            3. Тип вопроса (SingleChoice - один ответ, MultipleChoice - несколько ответов)
            4. Варианты ответов (от 3 до 5 вариантов)
            5. Правильные ответы (один или несколько в зависимости от типа)
            
            Верни результат в формате JSON:
            {{
              "questions": [
                {{
                  "question_text": "Текст вопроса",
                  "description": "Описание вопроса",
                  "quest_type": "SingleChoice|MultipleChoice",
                  "options": ["Вариант 1", "Вариант 2", ...],
                  "correct_answers": ["Правильный ответ", ...]
                }},
                ...
              ]
            }}
            """
        else:
            return f"""
            Создай тестовый набор по теме "{test_description}"{' на основе материала "' + file_name + '"' if file_name else ''}.
            
            Сформируй 5 вопросов разного типа. Для каждого вопроса укажи:
            1. Текст вопроса
            2. Краткое описание
            3. Тип вопроса (SingleChoice - один ответ, MultipleChoice - несколько ответов)
            4. Варианты ответов (от 3 до 5 вариантов)
            5. Правильные ответы (один или несколько в зависимости от типа)
            
            Верни результат в формате JSON:
            {{
              "questions": [
                {{
                  "question_text": "Текст вопроса",
                  "description": "Описание вопроса",
                  "quest_type": "SingleChoice|MultipleChoice",
                  "options": ["Вариант 1", "Вариант 2", ...],
                  "correct_answers": ["Правильный ответ", ...]
                }},
                ...
              ]
            }}
            """
    
    def _call_openrouter_api(self, prompt: str) -> str:
        """
        Выполняет запрос к OpenRouter API для генерации текста.
        """
            
        try:
            headers = {
                "Authorization": f"Bearer {OPENROUTER_API_KEY}",
                "Content-Type": "application/json"
            }
            
            data = {
                "model": OPENROUTER_MODEL,
                "messages": [
                    {"role": "system", "content": "Ты - генератор тестовых вопросов. Твоя задача создавать качественные вопросы с вариантами ответов по заданной теме. Вопросы должны быть на русском языке. Внимательно проверяй, что генерируешь корректный JSON. Всегда завершай каждый объект правильными закрывающими скобками."},
                    {"role": "user", "content": prompt}
                ],
                "temperature": 0.7,
                "max_tokens": 2000,
                "response_format": {"type": "json_object"}  # Запрашиваем структурированный JSON-ответ
            }
            
            logger.debug(f"Отправка запроса к OpenRouter API: {json.dumps(data)}")
            logger.info(f"Используемая модель: {OPENROUTER_MODEL}")
            
            response = requests.post(
                f"{OPENROUTER_API_URL}/chat/completions",
                headers=headers,
                json=data,
                timeout=120
            )
            
            logger.debug(f"Получен ответ от API: Статус {response.status_code}")
            
            if response.status_code != 200:
                error_message = f"Ошибка API: {response.status_code} - {response.text}"
                logger.error(error_message)
                
                if response.status_code == 404 and "model" in response.text.lower():
                    try:
                        error_data = json.loads(response.text)
                        error_msg = error_data.get('error', {}).get('message', '')
                        
                        if 'model' in error_msg.lower():
                            logger.warning(f"Модель {OPENROUTER_MODEL} недоступна, пробуем использовать резервную модель")
                            
                            backup_model = "anthropic/claude-instant-1:free"
                            data["model"] = backup_model
                            
                            logger.info(f"Используем резервную модель: {backup_model}")
                            
                            response = requests.post(
                                f"{OPENROUTER_API_URL}/chat/completions",
                                headers=headers,
                                json=data,
                                timeout=120
                            )
                            
                            if response.status_code == 200:
                                response_data = response.json()
                                logger.info("Успешный запрос с использованием резервной модели")
                                return response_data['choices'][0]['message']['content']
                    except Exception as e:
                        logger.error(f"Ошибка при обработке ошибки модели: {e}")
            
            response_data = response.json()
            logger.debug(f"Полный ответ API: {json.dumps(response_data)}")
            return response_data['choices'][0]['message']['content']
            
        except Exception as e:
            logger.error(f"Ошибка при вызове OpenRouter API: {e}")
            
    def _parse_llm_response_to_questions(self, llm_response: str, created_at: str) -> List[Dict]:
        """
        Парсит ответ от LLM и преобразует его в формат вопросов.
        """
        try:
            llm_response = self._fix_incomplete_json(llm_response)

            try:
                response_data = json.loads(llm_response)
            except json.JSONDecodeError:
                import re
                json_match = re.search(r'```json\n(.*?)\n```', llm_response, re.DOTALL)
                if json_match:
                    response_data = json.loads(json_match.group(1))
                else:
                    json_match = re.search(r'\{.*\}', llm_response, re.DOTALL)
                    if json_match:
                        response_data = json.loads(json_match.group(0))
                    else:
                        raise ValueError("Не удалось извлечь JSON из ответа LLM")
            
            questions = []
            for q in response_data.get("questions", []):
                question = {
                    "Id": str(uuid.uuid4()),
                    "QuestionText": q.get("question_text", ""),
                    "Description": q.get("description", ""),
                    "QuestType": q.get("quest_type", "SingleChoice"),
                    "Options": q.get("options", []),
                    "CorrectAnswers": q.get("correct_answers", []),
                    "CreatedAt": created_at,
                    "GeneratedByAi": True
                }
                questions.append(question)
            
            return questions
        except Exception as e:
            logger.error(f"Ошибка при парсинге ответа LLM: {e}")
            logger.debug(f"Полученный ответ LLM: {llm_response}")
            raise e
    
    def _fix_incomplete_json(self, json_str: str) -> str:
        """
        Пытается восстановить неполный JSON-ответ от LLM.
        """
        try:
            json.loads(json_str)
            return json_str
        except json.JSONDecodeError as e:
            logger.warning(f"Получен неполный JSON, пытаемся восстановить: {e}")
            
            try:
                parsed_length = e.pos
                
                if parsed_length < 10:
                    raise ValueError("Недостаточно данных для восстановления JSON")
                
                if "Expecting ',' delimiter" in str(e) or "Expecting property name" in str(e):
                    last_valid_brace = json_str.rfind('}', 0, parsed_length)
                    if last_valid_brace > 0:
                        open_braces = json_str[:last_valid_brace+1].count('{')
                        close_braces = json_str[:last_valid_brace+1].count('}')
                        missing_braces = open_braces - close_braces
                        
                        if missing_braces > 0:
                            fixed_json = json_str[:last_valid_brace+1] + "}" * missing_braces
                            logger.info(f"JSON восстановлен, добавлено закрывающих скобок: {missing_braces}")
                            return fixed_json
                
                try:
                    json_obj = json.loads(json_str[:parsed_length] + ']}')
                    if "questions" in json_obj and len(json_obj["questions"]) > 0:
                        logger.info(f"JSON восстановлен до последнего полного вопроса, всего вопросов: {len(json_obj['questions'])}")
                        return json.dumps(json_obj)
                except:
                    pass
                
                try:
                    import re
                    question_pattern = r'\{\s*"question_text"\s*:.*?"correct_answers"\s*:\s*\[.*?\]\s*\}'
                    questions = re.findall(question_pattern, json_str, re.DOTALL)
                    
                    if questions:
                        reconstructed = {"questions": []}
                        for q in questions:
                            try:
                                question_obj = json.loads(q)
                                reconstructed["questions"].append(question_obj)
                            except:
                                continue
                        
                        if reconstructed["questions"]:
                            logger.info(f"JSON реконструирован из {len(reconstructed['questions'])} полных вопросов")
                            return json.dumps(reconstructed)
                except:
                    pass
            
            except Exception as inner_e:
                logger.error(f"Ошибка при попытке восстановления JSON: {inner_e}")
            
            raise ValueError(f"Не удалось восстановить неполный JSON: {e}")

    def generate_recommendation(self, request_id: str, test_result_id: str, incorrect_answers: List[str], question_contexts: List[Dict]) -> str:
        """
        Генерация персональной рекомендации на основе результатов теста с помощью LLM.
        Использует модель OpenRouter API для генерации рекомендации.
        """
        try:
            logger.info(f"Генерация рекомендации для запроса {request_id}, тест ID: {test_result_id}")
            
            prompt = self._create_recommendation_prompt(incorrect_answers, question_contexts)
            
            recommendation_content = self._call_openrouter_api(prompt)
            
            if not recommendation_content:
                logger.error(f"Ошибка: не получены данные от LLM для запроса {request_id}")
                raise Exception("Не удалось получить ответ от модели LLM")
  
            return recommendation_content
        except Exception as e:
            logger.error(f"Ошибка при генерации рекомендации: {e}")
            raise
    
    def _create_recommendation_prompt(self, incorrect_answers: List[str], question_contexts: List[Dict]) -> str:
        """
        Формирует промпт для запроса к LLM на основе неправильных ответов и контекста вопросов.
        """
        context_str = ""
        for i, context in enumerate(question_contexts):
            options_str = ", ".join(context.get("QuestionAnswers", []))
            context_str += f"\nВопрос {i+1}: {context.get('QuestionText', '')}\n"
            context_str += f"Описание: {context.get('QuestionDescription', '')}\n"
            context_str += f"Варианты ответов: {options_str}\n"

        incorrect_answers_str = ", ".join(incorrect_answers)
        
        return f"""
        Проанализируй результаты теста пользователя и сформируй персональную рекомендацию.
        
        Контекст вопросов:
        {context_str}
        
        Неправильные ответы пользователя:
        {incorrect_answers_str}
        
        На основе этих данных:
        1. Определи основные пробелы в знаниях пользователя
        2. Предложи конкретные рекомендации для изучения (материалы, ресурсы)
        3. Составь краткий план обучения для восполнения пробелов
        4. Рекомендуй дополнительные тесты или задания для закрепления знаний
        
        Сформируй подробную и полезную рекомендацию, которая поможет пользователю улучшить свои знания.
        Рекомендация должна быть структурирована, содержать конкретные шаги и быть написана на русском языке.
        """

    def process_message(self, ch, method, properties, body):
        """Обработка входящего сообщения из очереди задач"""
        try:
            message = json.loads(body.decode('utf-8'))
            request_id = message.get("RequestId")
            
            if "TestDescription" in message:
                test_description = message.get("TestDescription")
                file_name = message.get("FileName")
                
                logger.info(f"Получено сообщение для генерации теста: RequestId={request_id}, TestDescription={test_description}")
                
                self.send_status_update(request_id, TestGenerationStatus.IN_PROGRESS)
                
                try:
                    test_entity = self.generate_test(request_id, test_description, file_name)
                    
                    self.send_test_result(request_id, test_entity)
                    
                    self.send_status_update(request_id, TestGenerationStatus.SUCCEEDED)
                except Exception as test_e:
                    logger.error(f"Ошибка при генерации теста: {test_e}")
                    self.send_status_update(request_id, TestGenerationStatus.FAILED)
                    raise
                
            elif "UserIncorrectAnswers" in message:
                test_result_id = message.get("TestResultId")
                incorrect_answers = message.get("UserIncorrectAnswers", [])
                contexts = message.get("Context", [])
                
                logger.info(f"Получено сообщение для генерации рекомендации: RequestId={request_id}, TestResultId={test_result_id}")
                
                try:
                    recommendation = self.generate_recommendation(request_id, test_result_id, incorrect_answers, contexts)
                    
                    self.send_recommendation(request_id, test_result_id, recommendation)
                    
                except Exception as rec_e:
                    logger.error(f"Ошибка при генерации рекомендации: {rec_e}")
                    raise
            
            ch.basic_ack(delivery_tag=method.delivery_tag)
        except Exception as e:
            logger.error(f"Ошибка при обработке сообщения: {e}")
            ch.basic_ack(delivery_tag=method.delivery_tag)

    def start_consuming(self):
        """Запуск прослушивания очереди задач"""
        try:
            self.channel.basic_qos(prefetch_count=1)
            
            self.channel.basic_consume(
                queue=TASK_QUEUE_NAME,
                on_message_callback=self.process_message
            )
            
            logger.info(f"Ожидание сообщений в очереди {TASK_QUEUE_NAME}...")
            self.channel.start_consuming()
        except KeyboardInterrupt:
            logger.info("Получено прерывание. Закрытие соединения...")
            self.connection.close()
        except Exception as e:
            logger.error(f"Ошибка при получении сообщений: {e}")
            if self.connection and self.connection.is_open:
                self.connection.close()

if __name__ == "__main__":
    # time.sleep(10)
    
    service = LlmService()
    
    try:
        service.start_consuming()
    except Exception as e:
        logger.error(f"Неожиданная ошибка: {e}")
