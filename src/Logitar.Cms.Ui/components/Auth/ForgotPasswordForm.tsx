import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, Form, Formik } from 'formik';
import { Box, Button } from '@mui/material';
import { TextField } from 'formik-mui';

import { WithTranslateFormErrors } from '~components';
import { flexColCenter } from '~styles/containerStyles';

type ForgotPasswordFormProps = {
  onClickRememberPassword: () => void;
};

export const ForgotPasswordForm: React.FC<ForgotPasswordFormProps> = ({
  onClickRememberPassword,
}) => {
  const { t } = useTranslation('Auth');

  return (
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
      {({ errors, touched, setFieldTouched }) => (
        <WithTranslateFormErrors
          errors={errors}
          touched={touched}
          setFieldTouched={setFieldTouched}
        >
          <Form style={{ width: '100%' }}>
            <Box sx={{ ...flexColCenter, mt: 4, gap: 3 }}>
              <Field
                name="email"
                label={t('forgotPassword.form.labels.email')}
                component={TextField}
                fullWidth
              />
              <Box sx={{ ...flexColCenter, gap: 1, width: '100%' }}>
                <Button variant="contained" type="submit" fullWidth>
                  {t('forgotPassword.form.buttons.submit')}
                </Button>
                <Button
                  sx={{ textTransform: 'none' }}
                  onClick={() => {
                    onClickRememberPassword();
                  }}
                  fullWidth
                >
                  {t('forgotPassword.form.buttons.rememberPassword')}
                </Button>
              </Box>
            </Box>
          </Form>
        </WithTranslateFormErrors>
      )}
    </Formik>
  );
};
