import React, { useState } from "react";
import "./styles.css";

interface Option {
  id: number;
  option: string;
  isCorrect: boolean;
}

interface Question {
  id: number;
  question: string;
  options: Option[];
}

interface SingleChoiceProps {
  question: Question;
  onAnswer: (selectedOption: number) => void;
}

export const SingleChoice: React.FC<SingleChoiceProps> = ({ question, onAnswer }) => {
  const [selectedOption, setSelectedOption] = useState<number | null>(null);

  const handleOptionSelect = (optionId: number) => {
    setSelectedOption(optionId);
  };

  const handleSubmit = () => {
    if (selectedOption !== null) {
      onAnswer(selectedOption);
      setSelectedOption(null);
    }
  };

  return (
    <div className="fade-in">
      <h2 className="question-title">{question.question}</h2>
      <div className="options-container">
        {question.options.map((option) => (
          <div key={option.id} className="option-item">
            <input
              type="radio"
              id={`option-${option.id}`}
              name="single-choice"
              checked={selectedOption === option.id}
              onChange={() => handleOptionSelect(option.id)}
            />
            <label htmlFor={`option-${option.id}`}>
              {option.option}
            </label>
          </div>
        ))}
      </div>
      <button
        onClick={handleSubmit}
        disabled={selectedOption === null}
        className="quiz-button"
      >
        Ответить
      </button>
    </div>
  );
};   