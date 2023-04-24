import { useTranslation } from 'react-i18next';
import { Box, Button } from '@mui/material';
import * as Yup from 'yup';

import { Field, Form, Formik } from 'formik';
import { CheckboxWithLabel, TextField } from 'formik-mui';

import { WithTranslateFormErrors } from '~components';
import { flexCol, flexColCenter as flexColCenter } from '~styles';

type SignInFormProps = {
  onClickForgotPassword: () => void;
};

export const SignInForm: React.FC<SignInFormProps> = ({ onClickForgotPassword }) => {
  const { t } = useTranslation('Auth');

  return (
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
                name="username"
                label={t('signIn.form.labels.username')}
                component={TextField}
                fullWidth
              />
              <Box sx={{ ...flexCol, width: '100%' }}>
                <Button
                  sx={{ alignSelf: 'end', textTransform: 'none' }}
                  onClick={onClickForgotPassword}
                >
                  {t('signIn.form.buttons.forgotPassword')}
                </Button>
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
              <Button variant="contained" type="submit" fullWidth>
                {t('signIn.form.buttons.submit')}
              </Button>
            </Box>
          </Form>
        </WithTranslateFormErrors>
      )}
    </Formik>
  );
};
