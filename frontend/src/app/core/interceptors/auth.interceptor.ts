import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthStore } from '../auth/auth.store';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const isAnonymous =
    req.url.includes('/Auth/login') || req.url.includes('/Auth/register');

  if (isAnonymous) return next(req);

  const token = inject(AuthStore).token();
  if (!token) return next(req);

  return next(
    req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
  );
};
