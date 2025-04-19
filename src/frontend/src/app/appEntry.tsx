import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider } from './providers/Router.provider'

// global styles
import './styles/layout.css'
import './styles/colors.css'
import './styles/reset.css'
import './styles/transitions.css'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <RouterProvider />
    </StrictMode>,
)
