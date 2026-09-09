import { Routes } from '@angular/router';
import { adminGuard, approvedGuard, authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/landing/landing').then((m) => m.Landing),
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register').then((m) => m.Register),
  },
  {
    path: 'pending',
    canActivate: [authGuard],
    loadComponent: () => import('./features/auth/pending').then((m) => m.Pending),
  },
  {
    path: 'dashboard',
    canActivate: [approvedGuard],
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./features/admin/admin').then((m) => m.Admin),
  },
  { path: '**', redirectTo: '' },
];
