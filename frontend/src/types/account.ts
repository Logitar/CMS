import type { Change } from "./change";

export type ChangePasswordInput = {
  current: string;
  new: string;
};

export type CurrentUser = {
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type PersonNameType = "first" | "last" | "middle" | "nick";

export type SaveProfilePayload = {
  password?: ChangePasswordInput;
  firstName?: Change<string>;
  middleName?: Change<string>;
  lastName?: Change<string>;
  emailAddress?: Change<string>;
  pictureUrl?: Change<string>;
};

export type SignInPayload = {
  username: string;
  password: string;
};

export type UserProfile = {
  username: string;
  passwordChangedOn?: string;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  fullName?: string;
  emailAddress?: string;
  pictureUrl?: string;
  createdOn: string;
  updatedOn: string;
  authenticatedOn?: string;
};
