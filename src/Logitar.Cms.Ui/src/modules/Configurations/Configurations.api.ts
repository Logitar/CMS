import { ApiError, ApiErrorResponse, apiBaseUrl, get, post } from '~/Http';
import { InitializeConfigurationPayload } from './models';

export const initializeConfiguration = async (payload: InitializeConfigurationPayload) => {
  const result = await post(`${apiBaseUrl}/configurations`, payload);
  if (result.status === 200) {
    return result.data;
  }

  if (result.status === 403) {
    throw new ApiError(result.data as ApiErrorResponse);
  }

  throw new Error('Unknown');
};

export const isConfigurationInitialized = async (): Promise<boolean> => {
  const result = await get(`${apiBaseUrl}/configurations/initialized`);
  if (result.status === 200) {
    return result.data as boolean;
  }

  throw new Error(`Unknown`);
};
