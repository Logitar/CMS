export const basename = import.meta.env.MODE === 'production' ? '/cms' : '';

export const getPathFromUrl = (url: string): string => {
  const path = new URL(url).pathname;
  if (basename !== '' && path.startsWith(basename)) {
    return path.substring(basename.length);
  }

  return path;
};
