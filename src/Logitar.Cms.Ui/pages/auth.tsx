import { useState } from 'react';

import { Card, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { grey } from '@mui/material/colors';
import useMediaQuery from '@mui/material/useMediaQuery';
import { useTranslation } from 'react-i18next';
import { SplashContainer } from '~components/SplashContainer';

import { AppLogo } from '~components';
import { useTitle } from '~hooks';
import { flexColCenter } from '~styles';
import { ForgotPasswordForm, SignInForm } from '~components/Auth';

type AuthPageViewType = 'signIn' | 'forgotPassword';

export const AuthPage: React.FC = () => {
  const [view, setView] = useState<AuthPageViewType>('signIn');

  const { t } = useTranslation('Auth');

  const theme = useTheme();
  const isSmall = useMediaQuery(theme.breakpoints.up('sm'));

  useTitle(t(`${view}.title`));

  return (
    <SplashContainer>
      <Card
        elevation={1}
        sx={{
          ...flexColCenter,
          ...(isSmall && { maxWidth: '600px' }),
          padding: 4,
          width: '100%',
          transition: 'height 1s ease-in-out',
          textAlign: 'center',
        }}
      >
        <AppLogo />
        <Typography component="h1" variant="h4" sx={{ marginBottom: 1 }}>
          {t(`${view}.heading`)}
        </Typography>
        <Typography component="p" variant="subtitle1" color={grey['500']}>
          {t(`${view}.subheading`)}
        </Typography>
        {view === 'signIn' && (
          <SignInForm
            onClickForgotPassword={() => {
              setView('forgotPassword');
            }}
          />
        )}
        {view === 'forgotPassword' && (
          <ForgotPasswordForm
            onClickRememberPassword={() => {
              setView('signIn');
            }}
          />
        )}
      </Card>
    </SplashContainer>
  );
};
