import { createHashRouter, RouterProvider } from 'react-router-dom';
import { useMemo } from 'react';

import { I18nextProvider } from 'react-i18next';
import { CssBaseline, ThemeProvider, useMediaQuery } from '@mui/material';

import { useToggle } from '~hooks';
import { i18n } from '~locales';
import { getDefaultTheme } from '~themes';

const router = createHashRouter([
  {
    path: '/',
    element: <></>,
  },
]);

export const App: React.FC = () => {
  const [darkMode] = useToggle(useMediaQuery('(prefers-color-scheme: dark)'));

  const theme = useMemo(() => getDefaultTheme(darkMode), [darkMode]);

  return (
    <I18nextProvider i18n={i18n}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <RouterProvider router={router} />
      </ThemeProvider>
    </I18nextProvider>
  );
};
