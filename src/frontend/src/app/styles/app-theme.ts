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
})
      