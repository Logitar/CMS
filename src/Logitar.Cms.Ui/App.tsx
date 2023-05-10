import { useMemo } from 'react';

import { I18nextProvider } from 'react-i18next';
import { RouterProvider } from 'react-router-dom';

import { LogitarUi, useToggle, useMediaQuery, defaultTheme } from '@logitar/logitar-ui';

import { router } from './router';

import { i18n } from '~locales';

export const App: React.FC = () => {
  const [darkMode] = useToggle(useMediaQuery('(prefers-color-scheme: dark)'));

  const theme = useMemo(() => defaultTheme(darkMode), [darkMode]);

  return (
    <>
      <I18nextProvider i18n={i18n}>
        <LogitarUi theme={theme} reset>
          <RouterProvider router={router} />
        </LogitarUi>
      </I18nextProvider>
    </>
  );
};
