import { redirect } from 'react-router-dom';
import { isConfigurationInitialized } from '~api/configurations';
import { getLocales } from '~api/resources';

export const setupLoader = async () => {
  const initialized = await isConfigurationInitialized();
  if (initialized) {
    return redirect('/');
  }

  return getLocales();
};
