import { createBrowserRouter } from 'react-router-dom';

import { basename, rootLoader } from '~routes';
import ForgotPasswordPage, { forgotPasswordLoader } from '~routes/forgotPassword';
import ResetPasswordPage, { resetPasswordLoader } from '~routes/resetPassword';
import SetupPage, { setupLoader } from '~routes/setup';
import SignInPage, { signInLoader } from '~routes/signIn';

export const router = createBrowserRouter(
  [
    {
      path: '',
      element: <p>Home page — requires setup & authorization</p>,
      loader: ({ request }) => rootLoader(request),
      children: [
        {
          path: '/home',
          element: <p>Home page</p>,
        },
      ],
    },
    {
      path: '/sign-in',
      element: <SignInPage />,
      loader: signInLoader,
    },
    {
      path: '/forgot-password',
      element: <ForgotPasswordPage />,
      loader: forgotPasswordLoader,
    },
    {
      path: '/reset-password',
      element: <ResetPasswordPage />,
      loader: resetPasswordLoader,
    },
    {
      path: '/setup',
      element: <SetupPage />,
      loader: setupLoader,
    },
  ],
  {
    basename,
  }
);
