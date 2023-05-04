import { createBrowserRouter /*, redirect*/ } from 'react-router-dom';
import { getLocales } from '~api';
import { ForgotPasswordPage, ResetPasswordPage, SignInPage } from '~pages';
import { SetupPage } from '~pages/Setup';
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
      element: <SetupPage />,
      // loader: async () => {
      //   const initialized = await isConfigurationInitialized();
      //   if (initialized) {
      //     return redirect('/');
      //   }
      // },
      loader: async () => getLocales(),
    },
  ],
  {
    basename: import.meta.env.MODE === 'production' ? '/cms' : undefined,
  }
);
