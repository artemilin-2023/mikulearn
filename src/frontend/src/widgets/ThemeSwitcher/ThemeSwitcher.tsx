import { useState, useEffect, ReactNode } from 'react';
import './ThemeSwitcher.css';

interface ThemeSwitcherProps {
  maskImage?: string;
  switchDuration?: number;
  switchName?: string;
  children?: ReactNode;
}

const ThemeSwitcher: React.FC<ThemeSwitcherProps> = ({
  maskImage = '/shigure-ui.webp',
  switchDuration = 1000,
  switchName = 'theme-transition',
  children
}) => {
    const [isDarkTheme, setIsDarkTheme] = useState<boolean>(() => {

    const savedTheme = localStorage.getItem('mantine-color-scheme');
    const htmlTheme = document.documentElement.getAttribute('data-mantine-color-scheme');
    return savedTheme === 'dark' || htmlTheme === 'dark';
  });

  useEffect(() => {
    document.documentElement.style.setProperty('--switch-duration', `${switchDuration}ms`);
    document.documentElement.style.setProperty('--switch-name', switchName);
    document.documentElement.style.setProperty('--mask-image', `url(${maskImage})`);
    
    applyTheme(isDarkTheme);
  }, [isDarkTheme, switchDuration, switchName, maskImage]);

  const applyTheme = (isDark: boolean) => {
    if (isDark) {
      document.documentElement.classList.add('dark-theme');
      document.documentElement.setAttribute('data-mantine-color-scheme', 'dark');
      localStorage.setItem('mantine-color-scheme', 'dark');
    } else {
      document.documentElement.classList.remove('dark-theme');
      document.documentElement.setAttribute('data-mantine-color-scheme', 'light');
      localStorage.setItem('mantine-color-scheme', 'light');
    }
  };

  const toggleTheme = () => {
    if (!document.startViewTransition) {
      setIsDarkTheme(!isDarkTheme);
      return;
    }

    document.startViewTransition(() => {
      setIsDarkTheme(!isDarkTheme);
    });
  };

  return (
    <div className="theme-switcher-container">
      <button 
        onClick={toggleTheme}
        className="theme-switch-button"
        aria-label={isDarkTheme ? "Переключить на светлую тему" : "Переключить на темную тему"}
      >
        <div className="theme-switch-icon-container">
          <svg 
            className={`theme-icon sun-icon ${isDarkTheme ? 'active' : ''}`} 
            xmlns="http://www.w3.org/2000/svg" 
            viewBox="0 0 24 24"
          >
            <circle className="sun-circle" cx="12" cy="12" r="5"></circle>
            <line className="sun-ray" x1="12" y1="1" x2="12" y2="3"></line>
            <line className="sun-ray" x1="12" y1="21" x2="12" y2="23"></line>
            <line className="sun-ray" x1="4.22" y1="4.22" x2="5.64" y2="5.64"></line>
            <line className="sun-ray" x1="18.36" y1="18.36" x2="19.78" y2="19.78"></line>
            <line className="sun-ray" x1="1" y1="12" x2="3" y2="12"></line>
            <line className="sun-ray" x1="21" y1="12" x2="23" y2="12"></line>
            <line className="sun-ray" x1="4.22" y1="19.78" x2="5.64" y2="18.36"></line>
            <line className="sun-ray" x1="18.36" y1="5.64" x2="19.78" y2="4.22"></line>
          </svg>
          
          <svg 
            className={`theme-icon moon-icon ${!isDarkTheme ? 'active' : ''}`} 
            xmlns="http://www.w3.org/2000/svg" 
            viewBox="0 0 24 24"
          >
            <path className="moon" d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
          </svg>
        </div>
      </button>
      {children}
    </div>
  );
};

export default ThemeSwitcher;