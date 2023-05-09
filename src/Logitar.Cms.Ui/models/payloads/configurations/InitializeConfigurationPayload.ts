export type InitializeConfigurationPayload = {
  defaultLocale: string;
  user: {
    email: string;
    username: string;
    password: string;
    firstName: string;
    lastName: string;
  };
};
