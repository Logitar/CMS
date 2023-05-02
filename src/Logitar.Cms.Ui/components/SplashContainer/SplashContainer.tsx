import Box from '@mui/material/Box';

type SplashContainerProps = React.PropsWithChildren;

export const SplashContainer: React.FC<SplashContainerProps> = ({ children }) => {
  return (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '100vh',
        backgroundImage: "url('./assets/splash.webp')",
        backgroundPosition: 'center',
        backgroundSize: 'cover',
      }}
    >
      {children}
    </Box>
  );
};
