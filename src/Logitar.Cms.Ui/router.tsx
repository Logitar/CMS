import { createHashRouter } from 'react-router-dom';
import { AuthPage } from '~pages/auth';

export const router = createHashRouter([
  {
    path: '/',
    element: <h1>Hello world!</h1>,
  },
  {
    path: '/auth',
    element: <AuthPage />,
  },
]);
