import { HttpErrorResponse } from '@angular/common/http';

export function extractErrorMessage(
  err: unknown,
  fallback = 'Something went wrong.'
): string {
  if (err instanceof HttpErrorResponse) {
    const body = err.error;

    if (typeof body === 'string' && body.trim()) return body;

    if (body && typeof body === 'object') {
      const pd = body as { detail?: string; title?: string };
      if (pd.detail) return pd.detail;
      if (pd.title) return pd.title;
    }

    if (err.status === 0) return 'Cannot reach the server.';

    return err.message || fallback;
  }
  return fallback;
}
