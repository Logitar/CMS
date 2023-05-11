import { PropsWithChildren } from 'react';
import { Card, useMediaQuery, useTheme } from '@mui/material';

import { AppLogo, SplashContainer } from '~components';
import { flexColCenter } from '~styles/containerStyles';

export const AuthLayout: React.FC<PropsWithChildren> = ({ children }) => {
  const theme = useTheme();
  const isSmall = useMediaQuery(theme.breakpoints.up('sm'));

  return (
    <SplashContainer>
      <Card
        elevation={1}
        sx={{
          ...flexColCenter,
          ...(isSmall && { maxWidth: '500px' }),
          padding: 4,
          width: '100%',
          transition: 'height 1s ease-in-out',
          textAlign: 'center',
        }}
      >
        <AppLogo />
        {children}
      </Card>
    </SplashContainer>
  );
};
