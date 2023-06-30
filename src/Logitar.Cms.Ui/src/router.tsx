import { createBrowserRouter } from 'react-router-dom';

import {
  ForgotPasswordPage,
  ResetPasswordPage,
  SignInPage,
  forgotPasswordPageLoader,
  resetPasswordPageLoader,
  signInPageLoader,
} from '~/Account';

import { SetupPageLoader, SetupPage } from '~/Configurations';
import { dashboardLoader, DashboardPage } from '~/Dashboard';

export const router = createBrowserRouter(
  [
    // Dashboard
    {
      path: '',
      element: <DashboardPage />,
      loader: ({ request }) => dashboardLoader(request),
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
