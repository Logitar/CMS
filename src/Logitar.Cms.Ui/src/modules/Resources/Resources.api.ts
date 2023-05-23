import { apiBaseUrl, get } from '~/Http';
import { Locale } from './models';

export const getLocales = async (): Promise<Locale[]> => {
  const response = await get(`${apiBaseUrl}/resources/locales`);
  if (response.status === 200) {
    return response.data as Locale[];
  }

  throw new Error('Unknown');
};
