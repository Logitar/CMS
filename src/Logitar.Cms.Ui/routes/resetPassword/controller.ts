import { redirect } from 'react-router-dom';
import { getCurrentUser } from '~api/account';
import { isConfigurationInitialized } from '~api/configurations';

export const resetPasswordLoader = async () => {
  const initialized = await isConfigurationInitialized();
  if (!initialized) {
    return redirect('/setup');
  }

  const currentUser = await getCurrentUser();
  if (currentUser.isAuthenticated) {
    return redirect('/');
  }

  return null;
};
