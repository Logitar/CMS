import { apiBaseUrl, get, post, put } from '~api';
import { UpdateProfilePayload } from '~models/payloads';

export const getProfile = async () => {
  const result = await get(`${apiBaseUrl}/account/profile`);

  if (result.status === 204) {
    return result.data;
  }

  if (result.status === 401) {
    return null;
  }
};

export const updateProfile = async (payload: UpdateProfilePayload) => {};
