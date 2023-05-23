import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

export const useTitle = (title: string) => {
  const { t } = useTranslation('Common');
  useEffect(() => {
    document.title = `${title}${t('siteTitleSeparator')}${t('siteTitle')}`;
  });
};
