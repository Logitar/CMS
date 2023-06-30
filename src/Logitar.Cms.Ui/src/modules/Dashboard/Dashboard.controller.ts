import { redirect } from 'react-router-dom';

import { getCurrentUser } from '~/Account';
import { isConfigurationInitialized } from '~/Configurations';
import { getPathFromUrl } from '~/Http';

export const dashboardLoader = async (request: Request) => {
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
