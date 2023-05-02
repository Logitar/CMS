import { apiBaseUrl, get, post } from '~api';
import { SignInPayload } from '~models';

export const signIn = async (payload: SignInPayload): Promise<void> => {
  const response = await post(`${apiBaseUrl}/sign/in`, payload);
  if (response.status === 204) {
    return;
  }

  if (response.status === 401) {
    throw new Error('Invalid credentials');
  }

  throw new Error('An unexpected error has occured');
};

export const signOut = async (): Promise<void> => {
  await get(`${apiBaseUrl}/sign/out`);
};
