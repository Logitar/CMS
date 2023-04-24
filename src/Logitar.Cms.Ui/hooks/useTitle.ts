import { useEffect } from 'react';

const SITE_TITLE = 'Logitar CMS';
const SEPARATOR = ' | ';

export const useTitle = (title: string) => {
  useEffect(() => {
    document.title = `${title}${SEPARATOR}${SITE_TITLE}`;
  });
};
