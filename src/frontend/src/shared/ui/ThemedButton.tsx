import { Button, ButtonProps, MantineTheme } from '@mantine/core';
import { getThemeColor } from './mantine.helpers';

/**
 * Styled Button that uses the project's theme variables
 */
export const ThemedButton = (props: ButtonProps) => {
  // Define properly typed button styles
  const buttonStyles = {
    root: (theme: MantineTheme) => ({
      borderRadius: 'var(--radius-md)',
      transition: 'var(--transition-normal)',
    }),
    filled: (theme: MantineTheme) => ({
      backgroundColor: getThemeColor('primary'),
      '&:hover': {
        backgroundColor: getThemeColor('primary') + 'dd', // Adding transparency for hover state
      }
    }),
    outline: (theme: MantineTheme) => ({
      borderColor: getThemeColor('primary'),
      color: getThemeColor('primary'),
      '&:hover': {
        backgroundColor: getThemeColor('primary') + '15', // Very light background on hover
      }
    })
  };

  return <Button styles={buttonStyles} {...props} />;
};

/**
 * Example usage:
 * 
 * <ThemedButton>My Button</ThemedButton>
 * <ThemedButton variant="outline">Outline Button</ThemedButton>
 */ 