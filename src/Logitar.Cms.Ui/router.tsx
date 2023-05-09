import { createBrowserRouter, redirect } from 'react-router-dom';

import { getLocales } from '~api/resources';
import { isConfigurationInitialized } from '~api/configurations';
import { getProfile } from '~api/account';

import { ForgotPasswordPage, ResetPasswordPage, SetupPage, SignInPage } from '~pages';

export const router = createBrowserRouter(
  [
    {
      path: '',
      element: <p>Home page — requires setup & authorization</p>,
      loader: async () => {
        const initialized = await isConfigurationInitialized();
        if (!initialized) {
          return redirect('/setup');
        }

        try {
          await getProfile();
        } catch (error) {
          return redirect('/sign-in');
        }
      },
    },
    {
      path: '/sign-in',
      element: <SignInPage />,
      loader: async () => {
        try {
          await getProfile();
          return redirect('/');
        } catch (error) {
          return;
        }
      },
    },
    {
      path: '/forgot-password',
      element: <ForgotPasswordPage />,
      loader: async () => {
        try {
          await getProfile();
          return redirect('/');
        } catch (error) {
          return;
        }
      },
    },
    {
      path: '/reset-password',
      element: <ResetPasswordPage />,
      loader: async () => {
        try {
          await getProfile();
          return redirect('/');
        } catch (error) {
          return;
        }
      },
    },
    {
      path: '/setup',
      element: <SetupPage />,
      loader: async () => {
        const initialized = await isConfigurationInitialized();
        if (initialized) {
          return redirect('/');
        }

        return getLocales();
      },
    },
  ],
  {
    basename: import.meta.env.MODE === 'production' ? '/cms' : undefined,
  }
);
