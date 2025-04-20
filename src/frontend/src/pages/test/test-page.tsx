import React, { useState, useEffect } from "react";
import { SingleChoice } from "@shared/test-questions/SingleChoice";
import { MultipleChoice } from "@shared/test-questions/MultipleChoice";
import "@shared/test-questions/styles.css";

interface Option {
  id: number;
  option: string;
  isCorrect: boolean;
}

interface Question {
  id: number;
  question: string;
  options: Option[];
  type: "single" | "multiple";
}

const questions: Question[] = [
  {
    id: 1,
    question: "What is the capital of France?",
    type: "single",
    options: [
      { id: 1, option: "Paris", isCorrect: true },
      { id: 2, option: "London", isCorrect: false },
      { id: 3, option: "Berlin", isCorrect: false },
      { id: 4, option: "Madrid", isCorrect: false }
    ]
  },
  {
    id: 2,
    question: "Which of these are programming languages?",
    type: "multiple",
    options: [
      { id: 1, option: "JavaScript", isCorrect: true },
      { id: 2, option: "HTML", isCorrect: false },
      { id: 3, option: "Python", isCorrect: true },
      { id: 4, option: "CSS", isCorrect: false }
    ]
  },
  {
    id: 3,
    question: "Which planet is known as the Red Planet?",
    type: "single",
    options: [
      { id: 1, option: "Earth", isCorrect: false },
      { id: 2, option: "Mars", isCorrect: true },
      { id: 3, option: "Jupiter", isCorrect: false },
      { id: 4, option: "Venus", isCorrect: false }
    ]
  },
  {
    id: 4,
    question: "Select all continents:",
    type: "multiple",
    options: [
      { id: 1, option: "Europe", isCorrect: true },
      { id: 2, option: "Africa", isCorrect: true },
      { id: 3, option: "Atlantis", isCorrect: false },
      { id: 4, option: "Asia", isCorrect: true }
    ]
  }
];

export const TestPage: React.FC = () => {
  const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0);
  const [answers, setAnswers] = useState<Array<number | number[]>>([]);
  const [score, setScore] = useState(0);
  const [isTestComplete, setIsTestComplete] = useState(false);
  const [timeLeft, setTimeLeft] = useState(300);
  const [isTimerRunning, setIsTimerRunning] = useState(true);

  const currentQuestion = questions[currentQuestionIndex];
  const totalQuestions = questions.length;
  const progress = ((currentQuestionIndex) / totalQuestions) * 100;

  useEffect(() => {
    if (isTimerRunning && timeLeft > 0) {
      const timer = setTimeout(() => {
        setTimeLeft(timeLeft - 1);
      }, 1000);
      return () => clearTimeout(timer);
    } else if (timeLeft === 0 && !isTestComplete) {
      finishTest();
    }
  }, [timeLeft, isTimerRunning]);

  const formatTime = (seconds: number) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
  };

  const handleAnswer = (selectedOption: number | number[]) => {
    const newAnswers = [...answers];
    newAnswers[currentQuestionIndex] = selectedOption;
    setAnswers(newAnswers);


    const question = questions[currentQuestionIndex];
    
    if (question.type === "single") {
      const selectedOptionId = selectedOption as number;
      const option = question.options.find(opt => opt.id === selectedOptionId);
      
      if (option && option.isCorrect) {
        setScore(prevScore => prevScore + 1);
      }
    } else if (question.type === "multiple") {
      const selectedOptions = selectedOption as number[];
      const correctOptions = question.options.filter(opt => opt.isCorrect).map(opt => opt.id);
      const incorrectSelections = question.options.filter(opt => !opt.isCorrect).map(opt => opt.id);
      
      const allCorrectSelected = correctOptions.every(id => selectedOptions.includes(id));
      const noIncorrectSelected = !selectedOptions.some(id => incorrectSelections.includes(id));
      
      if (allCorrectSelected && noIncorrectSelected) {
        setScore(prevScore => prevScore + 1);
      }
    }

    if (currentQuestionIndex < totalQuestions - 1) {
      setCurrentQuestionIndex(currentQuestionIndex + 1);
    } else {
      finishTest();
    }
  };

  const finishTest = () => {
    setIsTestComplete(true);
    setIsTimerRunning(false);
  };

  const restartTest = () => {
    setCurrentQuestionIndex(0);
    setAnswers([]);
    setScore(0);
    setIsTestComplete(false);
    setTimeLeft(300);
    setIsTimerRunning(true);
  };

  return (
    <div className="quiz-container">
      {!isTestComplete ? (
        <div>
          <div className="quiz-header">
            <h1 className="quiz-title">Тест</h1>
            <div className="quiz-timer">
              Время: <span className={timeLeft < 60 ? 'quiz-timer-warning' : 'quiz-timer-normal'}>
                {formatTime(timeLeft)}
              </span>
            </div>
          </div>
          
          <div className="quiz-card">
            <div className="quiz-progress-container">
              <div className="quiz-progress-text">
                <span>Вопрос {currentQuestionIndex + 1} из {totalQuestions}</span>
                <span>Прогресс: {Math.floor(progress)}%</span>
              </div>
              <div className="quiz-progress-bar-container">
                <div className="quiz-progress-bar" style={{ width: `${progress}%` }}></div>
              </div>
            </div>
            
            {currentQuestion.type === "single" ? (
              <SingleChoice
                question={currentQuestion}
                onAnswer={(option) => handleAnswer(option)}
              />
            ) : (
              <MultipleChoice
                question={currentQuestion}
                onAnswer={(options) => handleAnswer(options)}
              />
            )}
          </div>
        </div>
      ) : (
        <div className="quiz-card results-container">
          <h1 className="results-title">Тест завершен!</h1>
          <div className="results-score">{score} / {totalQuestions}</div>
          <p className="results-percentage">
            Вы набрали {Math.round((score / totalQuestions) * 100)}%
          </p>
          <button
            onClick={restartTest}
            className="quiz-button"
          >
            Пройти тест снова
          </button>
        </div>
      )}
    </div>
  );
};   