import Avatar from '@mui/material/Avatar';
import BookIcon from '@mui/icons-material/Book';

type AppLogoProps = {
  size?: number;
};

export const AppLogo: React.FC<AppLogoProps> = ({ size = 64 }) => {
  return (
    <Avatar sx={{ m: 2, bgcolor: 'secondary.main', borderRadius: 1, width: size, height: size }}>
      <BookIcon />
    </Avatar>
  );
};
