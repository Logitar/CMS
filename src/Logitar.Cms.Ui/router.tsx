import { createBrowserRouter /*, redirect*/ } from 'react-router-dom';
import { ForgotPasswordPage, ResetPasswordPage, SignInPage } from '~pages';
// import { isConfigurationInitialized } from '~api';

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
      path: '/sign-in',
      element: <SignInPage />,
      // loader: async () => {
      //   const authenticated = await isUserAuthenticated();
      //   if (authenticated) {
      //     return redirect('/home');
      //   }

      // },
    },
    {
      path: '/forgot-password',
      element: <ForgotPasswordPage />,
      // loader: async () => {
      //   const authenticated = await isUserAuthenticated();
      //   if (authenticated) {
      //     return redirect('/home');
      //   }

      // },
    },
    {
      path: '/reset-password',
      element: <ResetPasswordPage />,
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
