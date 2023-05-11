import { useTranslation } from 'react-i18next';
import { Box, Button, Typography } from '@mui/material';
import { grey } from '@mui/material/colors';
import { Field, Form, Formik } from 'formik';
import * as Yup from 'yup';

import { AuthLayout, WithTranslateFormErrors } from '~components';
import { useTitle } from '~hooks';
import { flexCol, flexColCenter } from '~styles';
import { TextField } from 'formik-mui';
import { Link } from 'react-router-dom';

export const ResetPasswordPage: React.FC = () => {
  const { t } = useTranslation('Auth');

  useTitle(t('resetPassword.title'));

  return (
    <AuthLayout>
      <Typography component="h1" variant="h4" sx={{ marginBottom: 1 }}>
        {t(`resetPassword.heading`)}
      </Typography>
      <Typography component="p" variant="subtitle1" color={grey['500']}>
        {t(`resetPassword.subheading`)}
      </Typography>
      <Formik
        initialValues={{
          password: '',
          confirmPassword: '',
        }}
        validationSchema={Yup.object({
          password: Yup.string().required(t('resetPassword.form.validations.required') as string),
          confirmPassword: Yup.string()
            .required(t('resetPassword.form.validations.required') as string)
            .equals(
              [Yup.ref('password')],
              t('resetPassword.form.validations.confirmPassword') as string
            ),
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
                    {t('resetPassword.form.buttons.rememberPassword')}
                  </Link>
                  <Field
                    type="password"
                    name="password"
                    label={t('resetPassword.form.labels.password')}
                    component={TextField}
                    fullWidth
                  />
                </Box>
                <Field
                  type="password"
                  name="confirmPassword"
                  label={t('resetPassword.form.labels.confirmPassword')}
                  component={TextField}
                  fullWidth
                />
                <Box sx={{ ...flexColCenter, gap: 1, width: '100%' }}>
                  <Button variant="contained" type="submit" disabled={isSubmitting} fullWidth>
                    {t('resetPassword.form.buttons.submit')}
                  </Button>
                </Box>
              </Box>
            </Form>
          </WithTranslateFormErrors>
        )}
      </Formik>
    </AuthLayout>
  );
};
