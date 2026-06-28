import { HttpErrorResponse } from '@angular/common/http';

type ErrorBody = {
  error?: string;
  message?: string;
  errors?: Record<string, string[]>;
};

export function getApiErrorMessage(err: unknown, fallback: string): string {
  if (err instanceof HttpErrorResponse) {
    const fromBody = extractBodyMessage(err.error);
    if (fromBody) return fromBody;

    if (err.status === 0) {
      return 'Unable to reach the server. It may be waking up on Render — wait 30 seconds and try again.';
    }

    return statusFallback(err.status, fallback);
  }

  if (err && typeof err === 'object' && 'message' in err && typeof (err as { message: unknown }).message === 'string') {
    return (err as { message: string }).message;
  }

  return fallback;
}

function extractBodyMessage(body: unknown): string | null {
  if (typeof body === 'string' && body.trim()) {
    return body.trim();
  }

  if (!body || typeof body !== 'object') {
    return null;
  }

  const record = body as ErrorBody;
  if (typeof record.error === 'string' && record.error.trim()) {
    return record.error.trim();
  }

  if (typeof record.message === 'string' && record.message.trim()) {
    return record.message.trim();
  }

  if (record.errors) {
    const messages = Object.values(record.errors)
      .flat()
      .filter((value): value is string => typeof value === 'string' && value.trim().length > 0);
    if (messages.length > 0) {
      return messages.join(' ');
    }
  }

  return null;
}

function statusFallback(status: number, fallback: string): string {
  switch (status) {
    case 400:
      return fallback || 'The request was invalid. Check your input and try again.';
    case 401:
      return 'Your session expired or is invalid. Please log in again.';
    case 403:
      return 'You do not have permission to perform this action.';
    case 404:
      return fallback || 'The requested item was not found.';
    case 409:
      return fallback || 'This action conflicts with existing data.';
    case 502:
      return fallback || 'The AI service is temporarily unavailable. Please try again shortly.';
    case 503:
      return fallback || 'The service is temporarily unavailable. Please try again shortly.';
    default:
      return fallback;
  }
}
