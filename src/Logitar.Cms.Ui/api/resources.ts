import { apiBaseUrl, get } from '~api';
import { Locale } from '~models/Locale';

export const getLocales = async (): Promise<Locale[]> => {
  const response = await get(`${apiBaseUrl}/resources/locales`);
  if (response.status === 200) {
    return response.data as Locale[];
  }

  throw new Error('Unknown');
};
