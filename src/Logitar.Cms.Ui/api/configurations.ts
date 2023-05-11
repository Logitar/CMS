import { apiBaseUrl, get, post } from '~api';
import { ApiErrorResponse } from '~models';
import { InitializeConfigurationPayload } from '~models/payloads/configurations';

export const initializeConfiguration = async (payload: InitializeConfigurationPayload) => {
  const result = await post(`${apiBaseUrl}/configurations`, payload);
  if (result.status === 200) {
    return result.data;
  }

  if (result.status === 403) {
    throw new Error((result.data as ApiErrorResponse).code);
  }

  throw new Error('Unknown');
};

export const isConfigurationInitialized = async (): Promise<boolean> => {
  const response = await get(`${apiBaseUrl}/configurations/initialized`);
  if (response.status === 200) {
    return response.data as boolean;
  }

  throw new Error(`Unknown`);
};
