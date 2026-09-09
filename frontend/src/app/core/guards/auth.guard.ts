import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from '../auth/auth.store';

export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (auth.isAuthenticated()) return true;
  return router.createUrlTree(['/login'], {
    queryParams: { returnUrl: state.url },
  });
};

export const approvedGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (!auth.isAuthenticated()) {
    return router.createUrlTree(['/login'], {
      queryParams: { returnUrl: state.url },
    });
  }
  return auth.isApproved() ? true : router.createUrlTree(['/pending']);
};

export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthStore);
  const router = inject(Router);
  if (auth.isAuthenticated() && auth.isAdmin()) return true;
  return router.createUrlTree(['/dashboard']);
};
