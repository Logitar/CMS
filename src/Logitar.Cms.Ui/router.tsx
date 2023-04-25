import { createHashRouter } from 'react-router-dom';
import { apiBaseUrl, get } from '~api';
import { AuthPage } from '~pages/auth';

export const router = createHashRouter([
  {
    path: '/',
    element: <h1>Hello world!</h1>,
    loader: async () => {
      const response = await get(`${apiBaseUrl}/resources/locales`);
      console.log(response);
      return 'patate';
    },
  },
  {
    path: '/auth',
    element: <AuthPage />,
  },
]);
