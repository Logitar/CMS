import { createBrowserRouter /*, redirect*/ } from 'react-router-dom';
// import { isConfigurationInitialized } from '~api';
import { AuthPage } from '~pages/auth';

export const router = createBrowserRouter(
  [
    {
      path: '',
      element: <p>Home page — requires setup & authorization</p>,
      // loader: async () => {
      //   const initialized = await isConfigurationInitialized();
      //   if (!initialized) {
      //     return redirect('/setup');
      //   }

      //   const authenticated = await isUserAuthenticated();
      //   if (!authenticated) {
      //     return redirect('/auth');
      //   }

      //   return redirect('/home');
      // },
    },
    {
      path: '/auth',
      element: <AuthPage />,
      // loader: async () => {
      //   const authenticated = await isUserAuthenticated();
      //   if (authenticated) {
      //     return redirect('/home');
      //   }

      // },
    },
    {
      path: '/setup',
      element: <div>Setup page — requires no setup</div>,
      // loader: async () => {
      //   const initialized = await isConfigurationInitialized();
      //   if (initialized) {
      //     return redirect('/');
      //   }
      // },
    },
  ],
  {
    basename: import.meta.env.MODE === 'production' ? '/cms' : undefined,
  }
);
