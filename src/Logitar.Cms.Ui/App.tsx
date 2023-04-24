import { createHashRouter, RouterProvider } from 'react-router-dom';
import { useMemo } from 'react';

import { CssBaseline, ThemeProvider, useMediaQuery } from '@mui/material';
import { getDefaultTheme } from 'themes';

import { useToggle } from '~hooks';

const router = createHashRouter([]);

export const App: React.FC = () => {
  const [darkMode] = useToggle(useMediaQuery('(prefers-color-scheme: dark)'));

  const theme = useMemo(() => getDefaultTheme(darkMode), [darkMode]);

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <RouterProvider router={router} />
    </ThemeProvider>
  );
};
