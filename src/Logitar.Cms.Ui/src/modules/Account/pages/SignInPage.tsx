import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Box, Button, Typography } from '@mui/material';
import { grey } from '@mui/material/colors';
import { Field, Form, Formik } from 'formik';
import * as Yup from 'yup';

import { CheckboxWithLabel, TextField } from 'formik-mui';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';

import { useTitle } from '@/hooks';
import { flexCol, flexColCenter } from '@/styles';

import { FormError, WithTranslateFormErrors } from '~/Form';
import { SplashLayout } from '~/Layout';

import { signIn } from '../Account.api';

export const SignInPage: React.FC = () => {
  const { t } = useTranslation('Account');

  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const [errorOpen, setErrorOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | undefined>();

  useTitle(t('signIn.title'));

  return (
    <SplashLayout>
      <Typography component="h1" variant="h4" sx={{ marginBottom: 1 }}>
        {t(`signIn.heading`)}
      </Typography>
      <Typography component="p" variant="subtitle1" color={grey['500']}>
        {t(`signIn.subheading`)}
      </Typography>
      <Formik
        initialValues={{
          username: '',
          password: '',
          remember: false,
        }}
        validationSchema={Yup.object({
          username: Yup.string().required(t('signIn.form.validations.required') as string),
          password: Yup.string().required(t('signIn.form.validations.required') as string),
          remember: Yup.boolean(),
        })}
        onSubmit={async ({ username, password, remember }, { setSubmitting }) => {
          try {
            setSubmitting(true);
            await signIn({ username, password, remember });

            if (searchParams.has('redirectUrl')) {
              const redirectUrl = searchParams.get('redirectUrl') as string;
              return navigate(redirectUrl);
            }

            return navigate('/');
          } catch (error) {
            const message = error instanceof Error ? error.message : 'unknown';
            setErrorMessage(`signIn.form.errors.${message}`);
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
              <Box sx={{ ...flexColCenter, mt: 4, gap: 3 }}>
                <Field
                  name="username"
                  label={t('signIn.form.labels.username')}
                  component={TextField}
                  fullWidth
                />
                <Box sx={{ ...flexCol, width: '100%' }}>
                  <Link
                    to="/forgot-password"
                    style={{
                      textAlign: 'right',
                      marginBottom: 8,
                      width: 'fit-content',
                      alignSelf: 'end',
                    }}
                  >
                    {t('signIn.form.buttons.forgotPassword')}
                  </Link>
                  <Field
                    name="password"
                    label={t('signIn.form.labels.password')}
                    type="password"
                    component={TextField}
                    fullWidth
                  />
                </Box>
                <Box sx={{ width: '100%', textAlign: 'left' }}>
                  <Field
                    name="remember"
                    Label={{ label: t('signIn.form.labels.remember') }}
                    type="checkbox"
                    component={CheckboxWithLabel}
                  />
                </Box>
                <Button variant="contained" type="submit" disabled={isSubmitting} fullWidth>
                  {t('signIn.form.buttons.submit')}
                </Button>
              </Box>
            </Form>
          </WithTranslateFormErrors>
        )}
      </Formik>
    </SplashLayout>
  );
};
