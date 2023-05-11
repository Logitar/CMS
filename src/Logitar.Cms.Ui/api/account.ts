import { apiBaseUrl, get, post, put } from '~api';
import { ApiErrorResponse, CurrentUser } from '~models';
import {
  ChangePasswordPayload,
  SignInPayload,
  UpdateProfilePayload,
} from '~models/payloads/account';

export const changePassword = async (payload: ChangePasswordPayload) => {
  const result = await put(`${apiBaseUrl}/account/password/change`, payload);
  if (result.status === 200) {
    return result.data;
  }

  if (result.status === 401) {
    throw new Error((result.data as ApiErrorResponse).code);
  }

  throw new Error('Unknown');
};

export const getCurrentUser = async (): Promise<CurrentUser> => {
  const result = await get(`${apiBaseUrl}/account/authenticated`);
  if (result.status === 200) {
    return result.data as CurrentUser;
  }

  throw new Error('Unknown');
};

export const getProfile = async () => {
  const result = await get(`${apiBaseUrl}/account/profile`);
  if (result.status === 200) {
    return result.data;
  }

  if (result.status === 401) {
    throw new Error((result.data as ApiErrorResponse).code);
  }

  throw new Error('Unknown');
};

export const updateProfile = async (payload: UpdateProfilePayload) => {
  const result = await put(`${apiBaseUrl}/account/profile`, payload);
  if (result.status === 200) {
    return result.data;
  }

  if (result.status === 401) {
    throw new Error((result.data as ApiErrorResponse).code);
  }

  throw new Error('Unknown');
};

export const signIn = async (payload: SignInPayload) => {
  const result = await post(`${apiBaseUrl}/account/sign/in`, payload);
  if (result.status === 204) {
    return;
  }
  
  if (result.status === 400) {
    throw new Error((result.data as ApiErrorResponse).code);
  }

  throw new Error('Unknown');
};

export const signOut = async () => {
  const result = await post(`${apiBaseUrl}/account/sign/out`);
  if (result.status === 204) {
    return;
  }

  throw new Error('Unknown');
};

export const signOutAll = async () => {
  const result = await post(`${apiBaseUrl}/account/sign/out/all`);
  if (result.status === 204) {
    return;
  }

  if (result.status === 401) {
    throw new Error((result.data as ApiErrorResponse).code);
  }

  throw new Error('Unknown');
};
