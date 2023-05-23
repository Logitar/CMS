import { createBrowserRouter, redirect } from 'react-router-dom';

import {
  ForgotPasswordPage,
  ResetPasswordPage,
  SignInPage,
  forgotPasswordPageLoader,
  getCurrentUser,
  resetPasswordPageLoader,
  signInPageLoader,
} from '~/Account';

import { isConfigurationInitialized, SetupPageLoader, SetupPage } from '~/Configurations';
import { getPathFromUrl } from '~/Http';

// TODO: Move loader to Home/Dashboard module
const rootLoader = async (request: Request) => {
  const initialized = await isConfigurationInitialized();
  if (!initialized) {
    return redirect('/setup');
  }

  const currentUser = await getCurrentUser();
  if (!currentUser.isAuthenticated) {
    const path = getPathFromUrl(request.url);

    let redirectUrl = '/sign-in';
    if (path.length && path !== '/') {
      redirectUrl += `?redirectUrl=${encodeURIComponent(path)}`;
    }

    return redirect(redirectUrl);
  }

  return currentUser;
};

export const router = createBrowserRouter(
  [
    // Root
    {
      path: '',
      element: <p>Home page — requires setup & authorization</p>,
      loader: ({ request }) => rootLoader(request),
    },
    // Account
    {
      path: '/forgot-password',
      element: <ForgotPasswordPage />,
      loader: forgotPasswordPageLoader,
    },
    {
      path: '/reset-password',
      element: <ResetPasswordPage />,
      loader: resetPasswordPageLoader,
    },
    {
      path: '/sign-in',
      element: <SignInPage />,
      loader: signInPageLoader,
    },
    // Configurations
    {
      path: '/setup',
      element: <SetupPage />,
      loader: SetupPageLoader,
    },
  ],
  {
    basename: import.meta.env.MODE === 'production' ? '/cms' : undefined,
  }
);
