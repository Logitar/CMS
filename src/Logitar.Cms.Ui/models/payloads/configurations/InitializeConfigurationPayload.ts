export type InitializeConfigurationPayload = {
  defaultLocale: string;
  user: {
    emailAddress: string;
    username: string;
    password: string;
    firstName: string;
    lastName: string;
  };
};
