import { Alert, Collapse, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import { useTranslation } from 'react-i18next';

export type FormErrorProps = {
  open: boolean;
  namespace: string;
  error?: string;
  onClose: () => void;
};

export const FormError: React.FC<FormErrorProps> = ({
  open,
  namespace,
  error,
  onClose,
}: FormErrorProps) => {
  const { t } = useTranslation(namespace);

  return (
    <Collapse in={open}>
      <Alert
        sx={{ marginTop: 4 }}
        variant="filled"
        severity="error"
        action={
          <IconButton aria-label="close" color="inherit" size="small" onClick={onClose}>
            <CloseIcon fontSize="inherit" />
          </IconButton>
        }
      >
        {error && t(`${error}`)}
      </Alert>
    </Collapse>
  );
};
