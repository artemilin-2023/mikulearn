import { MantineStyleProp } from '@mantine/core';

/**
 * Helper functions for working with Mantine components with custom project styling
 */

type StyleKey = 'primary' | 'secondary' | 'background' | 'surface' | 'text' | 'textLight';

/**
 * Get CSS variable values from the project's theme
 */
export const getThemeColor = (key: StyleKey): string => {
  const cssVarMap = {
    primary: 'var(--color-primary)',
    secondary: 'var(--color-secondary)',
    background: 'var(--color-background)',
    surface: 'var(--color-surface)',
    text: 'var(--color-text)',
    textLight: 'var(--color-text-light)',
  };

  return cssVarMap[key];
};

/**
 * Get consistent spacing values from the project's theme
 */
export const getThemeSpacing = (size: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl'): string => {
  const spacingVarMap = {
    xs: 'var(--spacing-xs)',
    sm: 'var(--spacing-sm)',
    md: 'var(--spacing-md)',
    lg: 'var(--spacing-lg)',
    xl: 'var(--spacing-xl)',
    xxl: 'var(--spacing-xxl)',
  };

  return spacingVarMap[size];
};

/**
 * Create a style object for Mantine components that uses the project's theme variables
 */
export const createThemeStyle = (styleObj: Record<string, any>): MantineStyleProp => {
  return styleObj;
};

/**
 * Example usage:
 * 
 * const buttonStyles = createThemeStyle({
 *   root: {
 *     backgroundColor: getThemeColor('primary'),
 *     padding: `${getThemeSpacing('sm')} ${getThemeSpacing('md')}`,
 *   }
 * });
 * 
 * <Button styles={buttonStyles}>Custom Button</Button>
 */ 