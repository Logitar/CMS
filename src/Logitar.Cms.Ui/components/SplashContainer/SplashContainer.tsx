import Container from '@mui/material/Container';

type SplashContainerProps = React.PropsWithChildren;

export const SplashContainer: React.FC<SplashContainerProps> = ({ children }) => {
  return (
    <Container
      sx={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '100vh',
      }}
    >
      {children}
    </Container>
  );
};
