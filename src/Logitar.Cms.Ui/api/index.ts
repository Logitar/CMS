export * from './auth';
export * from './configurations';
export * from './resources';

type RequestMethod = 'GET' | 'PATCH' | 'POST' | 'PUT' | 'DELETE';

type RequestParams = { [key: string]: string | number | boolean | null | undefined };

type RequestData = { [key: string]: unknown };

type ResponseResult = {
  data: unknown;
  status: number;
};

export const apiBaseUrl =
  import.meta.env.MODE === 'development' ? import.meta.env.VITE_API_BASE_URL : '/cms/api';

const execute = async (method: RequestMethod, url: string, data?: RequestData) => {
  const request: RequestInit = { method };

  if (data) {
    request.headers = {
      'Content-Type': 'application/json',
    };
    request.body = JSON.stringify(data);
  }

  const response = await fetch(url, request);
  const result: ResponseResult = { data: null, status: response.status };
  const dataType = response.headers.get('Content-Type');

  if (!dataType) {
    return result;
  }

  if (dataType.includes('application/json')) {
    result.data = await response.json();
  } else if (dataType.includes('text/html')) {
    result.data = await response.text();
  } else if (dataType.includes('text/')) {
    result.data = await response.text();
  } else {
    throw new Error(`Unsupported Content-Type: ${dataType}`);
  }

  return result;
};

export const get = async (url: string, params?: RequestParams) => {
  if (!params) {
    return execute('GET', url);
  }

  const queryString = Object.keys(params)
    .map(key => `${key}=${params[key]}`)
    .join('&');

  return execute('GET', `${url}?${queryString}`);
};

export const _delete = async (url: string, data?: RequestData) => execute('DELETE', url, data);

export const patch = async (url: string, data?: RequestData) => execute('PATCH', url, data);

export const post = async (url: string, data?: RequestData) => execute('POST', url, data);

export const put = async (url: string, data?: RequestData) => execute('PUT', url, data);
