import { useState } from 'react';
import { AutocompleteRenderInputParams, Box, Button, TextField, Typography } from '@mui/material';
import { TextField as FmuiTextField } from 'formik-mui';
import { grey } from '@mui/material/colors';
import { useTranslation } from 'react-i18next';
import { useLoaderData, useNavigate } from 'react-router-dom';
import { Field, Form, Formik } from 'formik';
import * as Yup from 'yup';

import { AuthLayout, FormError, WithTranslateFormErrors } from '~components';
import { useTitle } from '~hooks';
import { Locale } from '~models/Locale';
import { Autocomplete } from 'formik-mui';
import { flexCol } from '~styles';
import { initializeConfiguration } from '~api';

export const SetupPage: React.FC = () => {
  const { t, i18n } = useTranslation('Auth');

  const locales = useLoaderData() as Locale[];
  const navigate = useNavigate();

  const [errorOpen, setErrorOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | undefined>();

  useTitle(t('setup.title'));

  return (
    <AuthLayout>
      <Typography component="h1" variant="h4" sx={{ marginBottom: 1 }}>
        {t(`setup.heading`)}
      </Typography>
      <Typography component="p" variant="subtitle1" color={grey['500']}>
        {t(`setup.subheading`)}
      </Typography>
      <Formik
        initialValues={{
          defaultLocale: i18n.language,
          firstName: '',
          lastName: '',
          email: '',
          password: '',
        }}
        validationSchema={Yup.object({
          defaultLocale: Yup.string().required(t('setup.form.validations.required') as string),
          firstName: Yup.string().required(t('setup.form.validations.required') as string),
          lastName: Yup.string().required(t('setup.form.validations.required') as string),
          email: Yup.string()
            .email(t('setup.form.validations.email') as string)
            .required(t('setup.form.validations.required') as string),
          password: Yup.string().required(t('setup.form.validations.required') as string),
        })}
        onSubmit={async (values, { setSubmitting }) => {
          try {
            setSubmitting(true);
            const { firstName, lastName, email, password, defaultLocale } = values;
            await initializeConfiguration({
              defaultLocale,
              user: {
                firstName,
                lastName,
                emailAddress: email,
                password,
                username: email,
              },
            });
            navigate('/', { relative: 'path' });
          } catch (error) {
            const message = error instanceof Error ? error.message : 'Unknown';
            setErrorMessage(`setup.form.errors.${message}`);
            setErrorOpen(true);
            setSubmitting(false);
          }
        }}
      >
        {({ errors, touched, isSubmitting, setFieldTouched }) => (
          <WithTranslateFormErrors
            errors={errors}
            touched={touched}
            setFieldTouched={setFieldTouched}
          >
            <FormError
              open={errorOpen}
              onClose={() => setErrorOpen(false)}
              namespace="Auth"
              error={errorMessage}
            />
            <Form style={{ width: '100%' }}>
              <Typography
                component="h2"
                variant="h6"
                sx={{ marginTop: 4, marginBottom: 2, textAlign: 'left', width: '100%' }}
              >
                {t(`setup.form.headings.application`)}
              </Typography>
              <Field
                name="defaultLocale"
                component={Autocomplete}
                options={locales.map(locale => locale.code)}
                getOptionLabel={(option: string) =>
                  locales.find(locale => locale.code === option)?.nativeName ?? ''
                }
                formControl={{ fullWidth: true }}
                style={{ textAlign: 'left' }}
                renderInput={(params: AutocompleteRenderInputParams) => (
                  <TextField
                    {...params}
                    name="defaultLocale"
                    error={touched['defaultLocale'] && !!errors['defaultLocale']}
                    helperText={errors['defaultLocale']}
                    label={t('setup.form.labels.defaultLocale')}
                    variant="outlined"
                  />
                )}
              />
              <Typography
                component="h2"
                variant="h6"
                sx={{ marginTop: 4, marginBottom: 2, textAlign: 'left', width: '100%' }}
              >
                {t(`setup.form.headings.user`)}
              </Typography>
              <Box sx={{ ...flexCol, width: '100%', gap: 3 }}>
                <Field
                  type="text"
                  name="firstName"
                  component={FmuiTextField}
                  fullWidth
                  label={t('setup.form.labels.firstName')}
                />
                <Field
                  type="text"
                  name="lastName"
                  component={FmuiTextField}
                  fullWidth
                  label={t('setup.form.labels.lastName')}
                />
                <Field
                  type="text"
                  name="email"
                  component={FmuiTextField}
                  fullWidth
                  label={t('setup.form.labels.email')}
                />
                <Field
                  type="password"
                  name="password"
                  component={FmuiTextField}
                  fullWidth
                  label={t('setup.form.labels.password')}
                />
                <Button type="submit" variant="contained" disabled={isSubmitting}>
                  {t('setup.form.buttons.submit')}
                </Button>
              </Box>
            </Form>
          </WithTranslateFormErrors>
        )}
      </Formik>
    </AuthLayout>
  );
};
