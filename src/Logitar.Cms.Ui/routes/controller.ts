import { redirect } from 'react-router-dom';
import { getCurrentUser } from '~api/account';
import { isConfigurationInitialized } from '~api/configurations';

export const basename = import.meta.env.MODE === 'production' ? '/cms' : '';

export const getPathFromUrl = (url: string): string => {
  const path = new URL(url).pathname;
  if (basename !== '' && path.startsWith(basename)) {
    return path.substring(basename.length);
  }

  return path;
};

export const rootLoader = async (request: Request) => {
  const initialized = await isConfigurationInitialized();
  if (!initialized) {
    return redirect('/setup');
  }

  const currentUser = await getCurrentUser();
  if (!currentUser.isAuthenticated) {
    const path = getPathFromUrl(request.url);

    let redirectUrl = '/sign-in';
    if (path.length && path !== '/') {
      redirectUrl += `?redirectUrl=${encodeURIComponent(path)}`;
    }

    return redirect(redirectUrl);
  }

  return currentUser;
};
