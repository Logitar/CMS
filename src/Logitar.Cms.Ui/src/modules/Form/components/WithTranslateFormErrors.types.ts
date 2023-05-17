import { FormikErrors, FormikTouched } from 'formik';

type SetFieldTouched = (
  field: string,
  isTouched?: boolean | undefined,
  shouldValidate?: boolean | undefined
) => void;

type WithTranslateFormErrorsProps = React.PropsWithChildren<{
  errors: FormikErrors<unknown>;
  touched: FormikTouched<unknown>;
  setFieldTouched: SetFieldTouched;
}>;

export type { SetFieldTouched, WithTranslateFormErrorsProps };
