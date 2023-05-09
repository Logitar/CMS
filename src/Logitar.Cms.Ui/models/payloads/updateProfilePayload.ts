export type UpdateProfilePayload = {
  email: {
    address?: string;
    verify?: boolean;
  };
  firstName?: string;
  lastName?: string;
  locale?: string;
  picture?: string;
};
