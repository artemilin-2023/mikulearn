import { createTheme } from "@mantine/core"

export const theme = createTheme({
    colors: {
        primary: [
        '#ecfeff', // 50
        '#cffafe', // 100
        '#a5f3fc', // 200
        '#67e8f9', // 300
        '#22d3ee', // 400
        '#06b6d4', // 500
        '#0891b2', // 600
        '#0e7490', // 700
        '#155e75', // 800
        '#164e63', // 900
        ],
        secondary: [
        '#f0fdf4', // 50
        '#dcfce7', // 100
        '#bbf7d0', // 200
        '#86efac', // 300
        '#4ade80', // 400
        '#22c55e', // 500
        '#16a34a', // 600
        '#15803d', // 700
        '#166534', // 800
        '#14532d', // 900
        ],
    },
    primaryColor: 'primary',
    radius: {
        xs: '0.25rem',
        sm: '0.25rem',
        md: '0.5rem',
        lg: '1rem',
        xl: '9999px',
    },
    shadows: {
        xs: '0 1px 2px rgba(0, 0, 0, 0.05)',
        sm: '0 1px 2px rgba(0, 0, 0, 0.05)',
        md: '0 4px 6px rgba(0, 0, 0, 0.1)',
        lg: '0 10px 15px rgba(0, 0, 0, 0.1)',
        xl: '0 20px 25px rgba(0, 0, 0, 0.1)',
    },
    spacing: {
        xs: '0.25rem',
        sm: '0.5rem',
        md: '1rem',
        lg: '1.5rem',
        xl: '2rem',
    },
    defaultRadius: 'md',
    fontFamily: 'inherit',
    components: {
        Button: {
            styles: {
                root: {
                    fontWeight: 500,
                    '&[data-variant="gradient"]': {
                        background: 'var(--gradient-primary-secondary-light)',
                        color: 'var(--color-text)',
                        transition: '0.3s ease-in-out',
                        backgroundSize: '200% 100%',
                        backgroundPosition: '0% 0%',
                        '&:hover': {
                            backgroundPosition: '100% 0%',
                            animation: 'gradientShift 1.5s ease infinite',
                        },
                    },
                },
            },
        },
        AppShell: {
            styles: {
                root: {
                    backgroundColor: 'var(--background-color)',
                    color: 'var(--color-text)',
                }
            }
        },
        Paper: {
            styles: {
                root: {
                    backgroundColor: 'var(--color-surface)',
                    color: 'var(--color-text)',
                    borderColor: 'var(--color-border)',
                }
            }
        },
        Card: {
            styles: {
                root: {
                    backgroundColor: 'var(--color-surface)',
                    color: 'var(--color-text)',
                    borderColor: 'var(--color-border)',
                }
            }
        },
        Input: {
            styles: {
                input: {
                    backgroundColor: 'var(--color-surface)',
                    color: 'var(--color-text)',
                    borderColor: 'var(--color-border)',
                    '&::placeholder': {
                        color: 'var(--color-text-light)',
                    },
                }
            }
        },
    },
})

if (typeof document !== 'undefined') {
    const styleElement = document.createElement('style');
    styleElement.textContent = `
        @keyframes gradientShift {
            0% { background-position: 0% 50%; }
            50% { background-position: 100% 50%; }
            100% { background-position: 0% 50%; }
        }

        :root {
            --gradient-primary-secondary-light: linear-gradient(45deg, var(--mantine-color-primary-5), var(--mantine-color-secondary-5));
        }
        
        .dark-theme, html[data-mantine-color-scheme="dark"] {
            --gradient-primary-secondary-light: linear-gradient(45deg, var(--mantine-color-primary-6), var(--mantine-color-secondary-6));
        }
    `;
    document.head.appendChild(styleElement);
}
      