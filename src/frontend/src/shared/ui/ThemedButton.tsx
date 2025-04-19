import { Button, ButtonProps } from '@mantine/core';
import { getThemeColor, getThemeSpacing, createThemeStyle } from './mantine.helpers';

/**
 * Styled Button that uses the project's theme variables
 */
export const ThemedButton = (props: ButtonProps) => {
  const buttonStyles = createThemeStyle({
    root: {
      borderRadius: 'var(--radius-md)',
      transition: 'var(--transition-normal)',
    },
    filled: {
      backgroundColor: getThemeColor('primary'),
      '&:hover': {
        backgroundColor: getThemeColor('primary') + 'dd', // Adding transparency for hover state
      }
    },
    outline: {
      borderColor: getThemeColor('primary'),
      color: getThemeColor('primary'),
      '&:hover': {
        backgroundColor: getThemeColor('primary') + '15', // Very light background on hover
      }
    }
  });

  return <Button styles={buttonStyles} {...props} />;
};

/**
 * Example usage:
 * 
 * <ThemedButton>My Button</ThemedButton>
 * <ThemedButton variant="outline">Outline Button</ThemedButton>
 */ 