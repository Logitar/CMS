import Avatar from '@mui/material/Avatar';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import useMediaQuery from '@mui/material/useMediaQuery';
import useTheme from '@mui/material/styles/useTheme';

import BookIcon from '@mui/icons-material/Book';

import { flexColCenter } from '@/styles/containerStyles';
import { SplashLayoutProps } from './SplashLayout.types';

export const SplashLayout: React.FC<SplashLayoutProps> = ({ children }) => {
  const theme = useTheme();
  const isSmall = useMediaQuery(theme.breakpoints.up('sm'));

  return (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '100vh',
        backgroundImage: `url('https://images.unsplash.com/photo-1519681393784-d120267933ba?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1740&q=80')`,
        backgroundPosition: 'center',
        backgroundSize: 'cover',
      }}
    >
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
        <Avatar sx={{ m: 2, bgcolor: 'secondary.main', borderRadius: 1, width: 64, height: 64 }}>
          <BookIcon />
        </Avatar>
        {children}
      </Card>
    </Box>
  );
};
