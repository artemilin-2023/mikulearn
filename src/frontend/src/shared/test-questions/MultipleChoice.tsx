import React, { useState } from 'react';
import './styles.css';

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

interface MultipleChoiceProps {
  question: Question;
  onAnswer: (selectedOptions: number[]) => void;
}

export const MultipleChoice: React.FC<MultipleChoiceProps> = ({ question, onAnswer }) => {
  const [selectedOptions, setSelectedOptions] = useState<number[]>([]);

  const handleOptionToggle = (optionId: number) => {
    setSelectedOptions((prev) => {
      if (prev.includes(optionId)) {
        return prev.filter((id) => id !== optionId);
      } else {
        return [...prev, optionId];
      }
    });
  };

  const handleSubmit = () => {
    onAnswer(selectedOptions);
    setSelectedOptions([]);
  };

  return (
    <div className="fade-in">
      <h2 className="question-title">{question.question}</h2>
      <div className="options-container">
        {question.options.map((option) => (
          <div key={option.id} className="option-item">
            <input
              type="checkbox"
              id={`option-${option.id}`}
              checked={selectedOptions.includes(option.id)}
              onChange={() => handleOptionToggle(option.id)}
            />
            <label htmlFor={`option-${option.id}`}>
              {option.option}
            </label>
          </div>
        ))}
      </div>
      <button
        onClick={handleSubmit}
        disabled={selectedOptions.length === 0}
        className="quiz-button"
      >
        Ответить
      </button>
    </div>
  );
};       
