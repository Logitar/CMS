import { useTranslation } from 'react-i18next';
import { Box, Button, Typography } from '@mui/material';
import { grey } from '@mui/material/colors';
import { Field, Form, Formik } from 'formik';
import * as Yup from 'yup';

import { CheckboxWithLabel, TextField } from 'formik-mui';
import { Link, useNavigate } from 'react-router-dom';

import { signIn } from '~api';
import { AuthLayout, WithTranslateFormErrors } from '~components';
import { useTitle } from '~hooks';
import { SignInPayload } from '~models';
import { flexCol, flexColCenter } from '~styles';

export const SignInPage: React.FC = () => {
  const { t } = useTranslation('Auth');
  const navigate = useNavigate();

  useTitle(t('signIn.title'));

  const handleSignIn = async ({ username, password, remember }: SignInPayload) => {
    try {
      await signIn({ username, password, remember });
      return navigate('/');
    } catch (error) {
      // TODO error handling
      console.error(error);
    }
  };

  return (
    <AuthLayout>
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
        onSubmit={async (values, { setSubmitting }) => {
          setSubmitting(true);
          await handleSignIn(values);
          setSubmitting(false);
        }}
      >
        {({ errors, touched, isSubmitting, setFieldTouched }) => (
          <WithTranslateFormErrors
            errors={errors}
            touched={touched}
            setFieldTouched={setFieldTouched}
          >
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
    </AuthLayout>
  );
};
