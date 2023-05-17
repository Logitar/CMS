import { createTheme } from '@mui/material';

export const getDefaultTheme = (dark: boolean) =>
  createTheme({
    palette: {
      mode: dark ? 'dark' : 'light',
    },
  });
