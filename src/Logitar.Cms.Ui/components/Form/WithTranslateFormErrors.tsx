import { FormikErrors, FormikTouched } from 'formik/dist/types';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

type SetFieldTouched = (
  field: string,
  isTouched?: boolean | undefined,
  shouldValidate?: boolean | undefined
) => void;

interface WithTranslateFormErrorsProps extends React.PropsWithChildren {
  errors: FormikErrors<unknown>;
  touched: FormikTouched<unknown>;
  setFieldTouched: SetFieldTouched;
}

const useTranslateFormErrors = (
  errors: FormikErrors<unknown>,
  touched: FormikTouched<unknown>,
  setFieldTouched: SetFieldTouched
) => {
  const { i18n } = useTranslation();
  useEffect(() => {
    const onLanguageChanged = () => {
      // ? This is hacky but we will find a better approach eventually... 1ms is not too bad
      setTimeout(() => {
        Object.keys(errors).forEach(fieldName => {
          if (Object.keys(touched).includes(fieldName)) {
            setFieldTouched(fieldName);
          }
        });
      }, 1);
    };

    i18n.on('languageChanged', onLanguageChanged);
    return () => {
      i18n.off('languageChanged', onLanguageChanged);
    };
  }, [i18n, errors, setFieldTouched, touched]);
};

export const WithTranslateFormErrors: React.FC<WithTranslateFormErrorsProps> = ({
  errors,
  touched,
  setFieldTouched,
  children,
}) => {
  useTranslateFormErrors(errors, touched, setFieldTouched);
  return <>{children}</>;
};
