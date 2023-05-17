import { useMemo } from 'react';

import { CssBaseline, ThemeProvider, useMediaQuery } from '@mui/material';

import { I18nextProvider } from 'react-i18next';

import { RouterProvider } from 'react-router-dom';

import { useToggle } from '@/hooks';
import { i18n } from '@/locales';
import { router } from '@/router';
import { getDefaultTheme } from '@/themes';

import './App.scss';

export const App: React.FC = () => {
  const [darkMode] = useToggle(useMediaQuery('(prefers-color-scheme: dark)'));

  const theme = useMemo(() => getDefaultTheme(darkMode), [darkMode]);

  return (
    <>
      <CssBaseline />
      <I18nextProvider i18n={i18n}>
        <ThemeProvider theme={theme}>
          <RouterProvider router={router} />
        </ThemeProvider>
      </I18nextProvider>
    </>
  );
};
