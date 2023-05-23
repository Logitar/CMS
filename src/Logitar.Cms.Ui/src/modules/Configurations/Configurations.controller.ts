import { redirect } from 'react-router-dom';
import { isConfigurationInitialized } from './Configurations.api';

export const SetupPageLoader = async () => {
  const initialized = await isConfigurationInitialized();
  if (initialized) {
    return redirect('/');
  }

  return null;
};
