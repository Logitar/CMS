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
        backgroundImage: `url('https://images.unsplash.com/photo-1519681393784-d120267933ba?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1740&q=80')`,
        backgroundPosition: 'center',
        backgroundSize: 'cover',
      }}
    >
      {children}
    </Box>
  );
};
