import { AutocompleteRenderInputParams, Box, Button, TextField, Typography } from '@mui/material';
import { TextField as FmuiTextField } from 'formik-mui';
import { grey } from '@mui/material/colors';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { Field, Form, Formik } from 'formik';
import * as Yup from 'yup';

import { AuthLayout, WithTranslateFormErrors } from '~components';
import { useTitle } from '~hooks';
import { Locale } from '~models/Locale';
import { Autocomplete } from 'formik-mui';
import { flexCol } from '~styles';

export const SetupPage: React.FC = () => {
  const { t, i18n } = useTranslation('Auth');

  const locales = useLoaderData() as Locale[];

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
          locale: i18n.language,
        }}
        validationSchema={Yup.object({
          locale: Yup.string().required(t('setup.form.validations.required') as string),
          firstName: Yup.string().required(t('setup.form.validations.required') as string),
          lastName: Yup.string().required(t('setup.form.validations.required') as string),
          email: Yup.string()
            .email(t('setup.form.validations.email') as string)
            .required(t('setup.form.validations.required') as string),
          password: Yup.string().required(t('setup.form.validations.required') as string),
        })}
        onSubmit={async (values, { setSubmitting }) => {
          setSubmitting(true);
          console.log(values);
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
              <Typography
                component="h2"
                variant="h6"
                sx={{ marginTop: 4, marginBottom: 2, textAlign: 'left', width: '100%' }}
              >
                {t(`setup.form.headings.application`)}
              </Typography>
              <Field
                name="locale"
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
                    name="locale"
                    error={touched['locale'] && !!errors['locale']}
                    helperText={errors['locale']}
                    label={t('setup.form.labels.locale')}
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
