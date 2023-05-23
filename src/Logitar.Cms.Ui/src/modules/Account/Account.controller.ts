import { redirect } from 'react-router-dom';
import { isConfigurationInitialized } from '~/Configurations';
import { getCurrentUser } from './Account.api';

export const forgotPasswordPageLoader = async () => {
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

// TODO refactor this with token validation
export const resetPasswordPageLoader = async () => {
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

export const signInPageLoader = async () => {
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
