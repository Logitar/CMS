import { ApiErrorResponse } from './ApiErrorResponse';

export class ApiError extends Error {
  public code: string;

  constructor(response: ApiErrorResponse, message?: string) {
    super(message);
    this.code = response.code;
  }
}
