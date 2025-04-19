import os
import json
import pika
import uuid
import logging
from datetime import datetime
from enum import Enum, IntEnum
import time
from typing import Dict, List, Optional, Any

# Настройка логирования
logging.basicConfig(
    level=logging.DEBUG,  # DEBUG режим для отладки
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# Константы для статусов
class TestGenerationStatus(str, Enum):
    QUEUED = "Queued"
    IN_PROGRESS = "InProgress"
    FAILED = "Failed"
    SUCCEEDED = "Succeeded"

# Константы для типов ответов (соответствуют C# enum)
class ResponseType(IntEnum):
    STATUS = 0  # Соответствует C# enum ResponseType.Status
    RESULT = 1  # Соответствует C# enum ResponseType.Result

# Конфигурация из переменных окружения с значениями по умолчанию из appsettings.json
RABBITMQ_URL = os.getenv("RABBITMQ_URL", "amqp://rmuser:rmpassword@rabbitmq:5672")
TASK_QUEUE_NAME = os.getenv("TASK_QUEUE_NAME", "task_queue")
RESPONSE_QUEUE_NAME = os.getenv("RESPONSE_QUEUE_NAME", "response_queue")
TASK_ROUTING_KEY = os.getenv("TASK_ROUTING_KEY", "llm.tasks")
RESPONSE_ROUTING_KEY = os.getenv("RESPONSE_ROUTING_KEY", "llm.response")
EXCHANGE_NAME = os.getenv("EXCHANGE_NAME", "llm.services")

# Класс для сериализации Enum в JSON
class EnumEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj, Enum):
            return int(obj)
        return super().default(obj)

class LlmService:
    def __init__(self):
        self.connection = None
        self.channel = None
        self.connect_to_rabbitmq()

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
                
                # Объявление exchange
                self.channel.exchange_declare(
                    exchange=EXCHANGE_NAME,
                    exchange_type='direct',
                    durable=True
                )
                
                # Объявление очередей
                self.channel.queue_declare(queue=TASK_QUEUE_NAME, durable=True)
                self.channel.queue_declare(queue=RESPONSE_QUEUE_NAME, durable=True)
                
                # Привязка очередей к exchange
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

    def send_status_update(self, request_id: str, status: TestGenerationStatus):
        """Отправка обновления статуса в очередь ответов"""
        try:
            # Создаем объект ответа в формате, ожидаемом C# десериализатором
            # Body объект будет десериализован в LlmStatusResponse
            status_response = {
                "Type": int(ResponseType.STATUS),
                "Body": {
                    "RequestId": request_id,
                    "Status": status
                }
            }
            
            # Преобразуем в JSON строку и логируем для отладки
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
            # Создаем объект ответа в формате, ожидаемом C# десериализатором
            # Body объект будет десериализован в ResultLlmResponse
            result_response = {
                "Type": int(ResponseType.RESULT),
                "Body": {
                    "RequestId": request_id,
                    "TestEntity": test_entity
                }
            }
            
            # Преобразуем в JSON строку и логируем для отладки
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

    def generate_test(self, request_id: str, test_description: str, file_name: str) -> Dict:
        """
        Генерация теста с помощью LLM на основе описания.
        В реальном сервисе здесь будет интеграция с моделью LLM.
        """
        try:
            # В этом месте должен быть код для вызова LLM API
            # Для примера создаем тестовый объект
            test_id = str(uuid.uuid4())
            created_at = datetime.utcnow().isoformat()
            
            # Генерация примера вопросов
            questions = [
                {
                    "Id": str(uuid.uuid4()),
                    "QuestionText": f"Вопрос 1 по теме {test_description}",
                    "Description": "Описание вопроса 1",
                    "QuestType": "MultipleChoice",
                    "Options": ["Вариант A", "Вариант B", "Вариант C", "Вариант D"],
                    "CorrectAnswers": ["Вариант A"],
                    "CreatedAt": created_at,
                    "GeneratedByAi": True
                },
                {
                    "Id": str(uuid.uuid4()),
                    "QuestionText": f"Вопрос 2 по теме {test_description}",
                    "Description": "Описание вопроса 2",
                    "QuestType": "SingleChoice",
                    "Options": ["Вариант X", "Вариант Y", "Вариант Z"],
                    "CorrectAnswers": ["Вариант Y"],
                    "CreatedAt": created_at,
                    "GeneratedByAi": True
                }
            ]
            
            # Формирование объекта TestEntity
            test_entity = {
                "Id": test_id,
                "Title": f"Тест: {test_description}",
                "Description": f"Сгенерированный тест на основе '{file_name}'",
                "CreatedAt": created_at,
                "ModifiedAt": created_at,
                "Access": "Private",
                "CreatedBy": "00000000-0000-0000-0000-000000000000",  # Заглушка, в реальном сервисе будет ID пользователя
                "Questions": questions
            }
            
            return test_entity
        except Exception as e:
            logger.error(f"Ошибка при генерации теста: {e}")
            raise

    def process_message(self, ch, method, properties, body):
        """Обработка входящего сообщения из очереди задач"""
        try:
            # Декодирование сообщения
            message = json.loads(body.decode('utf-8'))
            request_id = message.get("RequestId")
            test_description = message.get("TestDescription")
            file_name = message.get("FileName")
            
            logger.info(f"Получено сообщение: RequestId={request_id}, TestDescription={test_description}")
            
            # Отправка статуса "В обработке"
            self.send_status_update(request_id, TestGenerationStatus.IN_PROGRESS)
            
            # Генерация теста
            test_entity = self.generate_test(request_id, test_description, file_name)
            
            # Отправка результата
            self.send_test_result(request_id, test_entity)
            
            # Отправка статуса "Успешно завершено"
            self.send_status_update(request_id, TestGenerationStatus.SUCCEEDED)
            
            # Подтверждение обработки сообщения
            ch.basic_ack(delivery_tag=method.delivery_tag)
        except Exception as e:
            logger.error(f"Ошибка при обработке сообщения: {e}")
            # Отправка статуса "Ошибка"
            if 'request_id' in locals():
                self.send_status_update(request_id, TestGenerationStatus.FAILED)
            # Подтверждение обработки сообщения даже в случае ошибки
            ch.basic_ack(delivery_tag=method.delivery_tag)

    def start_consuming(self):
        """Запуск прослушивания очереди задач"""
        try:
            # Настройка получения только одного сообщения за раз
            self.channel.basic_qos(prefetch_count=1)
            
            # Настройка обработчика сообщений
            self.channel.basic_consume(
                queue=TASK_QUEUE_NAME,
                on_message_callback=self.process_message
            )
            
            logger.info(f"Ожидание сообщений в очереди {TASK_QUEUE_NAME}...")
            # Запуск бесконечного цикла для получения сообщений
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
