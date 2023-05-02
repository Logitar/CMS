import { apiBaseUrl, get } from '~api';

export const isConfigurationInitialized = async (): Promise<boolean> => {
  const response = await get(`${apiBaseUrl}/configurations/initialized`);
  if (response.status === 200) {
    return response.data as boolean;
  }

  throw new Error(`Error while checking if config is initialized: ${response.status}`);
};
