import { createBrowserRouter, redirect } from 'react-router-dom';

import { getLocales } from '~api/resources';
import { isConfigurationInitialized } from '~api/configurations';
import { getCurrentUser } from '~api/account';

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

        const currentUser = await getCurrentUser();
        if (!currentUser.isAuthenticated) {
          return redirect('/sign-in');
        }

        return currentUser;
      },
    },
    {
      path: '/sign-in',
      element: <SignInPage />,
      loader: async () => {
        const initialized = await isConfigurationInitialized();
        if (!initialized) {
          return redirect('/setup');
        }

        const currentUser = await getCurrentUser();
        if (currentUser.isAuthenticated) {
          return redirect('/');
        }

        return null;
      },
    },
    {
      path: '/forgot-password',
      element: <ForgotPasswordPage />,
      loader: async () => {
        const initialized = await isConfigurationInitialized();
        if (!initialized) {
          return redirect('/setup');
        }

        const currentUser = await getCurrentUser();
        if (currentUser.isAuthenticated) {
          return redirect('/');
        }

        return null;
      },
    },
    {
      path: '/reset-password',
      element: <ResetPasswordPage />,
      loader: async () => {
        const initialized = await isConfigurationInitialized();
        if (!initialized) {
          return redirect('/setup');
        }

        const currentUser = await getCurrentUser();
        if (currentUser.isAuthenticated) {
          return redirect('/');
        }

        return null;
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
