import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, Form, Formik } from 'formik';
import { Box, Button } from '@mui/material';
import { TextField } from 'formik-mui';

import { WithTranslateFormErrors } from '~components';
import { flexColCenter } from '~styles/containerStyles';

type ResetPasswordFormProps = {
  onClickRememberPassword: () => void;
  onClickDontHaveCode: () => void;
};

export const ResetPasswordForm: React.FC<ResetPasswordFormProps> = ({
  onClickRememberPassword,
  onClickDontHaveCode,
}) => {
  const { t } = useTranslation('Auth');

  return (
    <Formik
      initialValues={{
        code: '',
      }}
      validationSchema={Yup.object({
        password: Yup.string().required(t('resetPassword.form.validations.required') as string),
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
                name="code"
                label={t('resetPassword.form.labels.code')}
                component={TextField}
                fullWidth
              />
              <Box sx={{ ...flexColCenter, gap: 1, width: '100%' }}>
                <Button variant="contained" type="submit" fullWidth>
                  {t('resetPassword.form.buttons.submit')}
                </Button>
                <Button
                  sx={{ textTransform: 'none' }}
                  onClick={() => {
                    onClickDontHaveCode();
                  }}
                  fullWidth
                >
                  {t('resetPassword.form.buttons.dontHaveCode')}
                </Button>
                <Button
                  sx={{ textTransform: 'none' }}
                  onClick={() => {
                    onClickRememberPassword();
                  }}
                  fullWidth
                >
                  {t('resetPassword.form.buttons.rememberPassword')}
                </Button>
              </Box>
            </Box>
          </Form>
        </WithTranslateFormErrors>
      )}
    </Formik>
  );
};
