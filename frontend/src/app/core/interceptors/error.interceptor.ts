import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthStore } from '../auth/auth.store';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthStore);
  const router = inject(Router);

  const isLogin = req.url.includes('/Auth/login');

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 && !isLogin && auth.isAuthenticated()) {
        auth.clear();
        router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
      } else if (err.status === 401 && !isLogin && !auth.isAuthenticated()) {
        // no valid token at all → straightforward redirect
        router.navigate(['/login']);
      }
      return throwError(() => err);
    })
  );
};
