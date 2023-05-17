import { useTranslation } from 'react-i18next';
import { Box, Button, Typography } from '@mui/material';
import { grey } from '@mui/material/colors';
import { Field, Form, Formik } from 'formik';
import * as Yup from 'yup';

import { TextField } from 'formik-mui';
import { Link } from 'react-router-dom';

import { useTitle } from '@/hooks';
import { flexCol, flexColCenter } from '@/styles';

import { WithTranslateFormErrors } from '~/Form';
import { SplashLayout } from '~/Layout';

export const ForgotPasswordPage: React.FC = () => {
  const { t } = useTranslation('Account');

  useTitle(t('forgotPassword.title'));

  return (
    <SplashLayout>
      <Typography component="h1" variant="h4" sx={{ marginBottom: 1 }}>
        {t(`forgotPassword.heading`)}
      </Typography>
      <Typography component="p" variant="subtitle1" color={grey['500']}>
        {t(`forgotPassword.subheading`)}
      </Typography>
      <Formik
        initialValues={{
          email: '',
        }}
        validationSchema={Yup.object({
          email: Yup.string()
            .email(t('forgotPassword.form.validations.email') as string)
            .required(t('forgotPassword.form.validations.required') as string),
        })}
        onSubmit={async (values, { setSubmitting }) => {
          console.log(values);
          setSubmitting(true);
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
                <Box sx={{ ...flexCol, width: '100%' }}>
                  <Link
                    to="/sign-in"
                    style={{
                      textAlign: 'right',
                      marginBottom: 8,
                      width: 'fit-content',
                      alignSelf: 'end',
                    }}
                  >
                    {t('forgotPassword.form.buttons.rememberPassword')}
                  </Link>
                  <Field
                    name="email"
                    label={t('forgotPassword.form.labels.email')}
                    component={TextField}
                    fullWidth
                  />
                </Box>
                <Box sx={{ ...flexColCenter, gap: 1, width: '100%' }}>
                  <Button variant="contained" type="submit" disabled={isSubmitting} fullWidth>
                    {t('forgotPassword.form.buttons.submit')}
                  </Button>
                </Box>
              </Box>
            </Form>
          </WithTranslateFormErrors>
        )}
      </Formik>
    </SplashLayout>
  );
};
