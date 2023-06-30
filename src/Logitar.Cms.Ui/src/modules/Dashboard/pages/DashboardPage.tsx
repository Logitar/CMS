import { Paper } from '@mui/material';
import { useLoaderData } from 'react-router-dom';
import { CurrentUser } from '~/Account';

export const DashboardPage: React.FC = () => {
  const user = useLoaderData() as CurrentUser;
  console.log(user);
  return (
    <Paper sx={{ borderRadius: 0, minHeight: '100vh' }} elevation={3}>
      Navbar
      <br />
      Sidebar
      <br />
      Outlet
      <br />
      <pre>{JSON.stringify(user)}</pre>
    </Paper>
  );
};
