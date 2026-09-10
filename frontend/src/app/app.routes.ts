import { Routes } from '@angular/router';
import { adminGuard, approvedGuard, authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/landing/landing').then((m) => m.Landing),
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((m) => m.Register),
  },
  {
    path: 'pending',
    canActivate: [authGuard],
    loadComponent: () => import('./features/auth/pending/pending').then((m) => m.Pending),
  },
  {
    path: 'apis',
    canActivate: [approvedGuard],
    loadComponent: () => import('./features/apis/apis').then((m) => m.Apis),
  },
  {
    path: 'dashboard',
    canActivate: [approvedGuard],
    loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () => import('./features/profile/profile').then((m) => m.Profile),
  },
  {
    path: 'targets/new',
    canActivate: [approvedGuard],
    loadComponent: () =>
      import('./features/targets/target-register/target-register').then((m) => m.TargetRegister),
  },
  {
    path: 'targets/:id',
    canActivate: [approvedGuard],
    loadComponent: () =>
      import('./features/targets/target-detail/target-detail').then((m) => m.TargetDetail),
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./features/admin/admin').then((m) => m.Admin),
  },
  {
    path: 'scans',
    canActivate: [approvedGuard],
    loadComponent: () => import('./features/scans/scans').then((m) => m.Scans),
  },
  {
    path: 'monitoring',
    canActivate: [approvedGuard],
    loadComponent: () => import('./features/monitoring/monitoring').then((m) => m.Monitoring),
  },
  { path: '**', redirectTo: '' },
];
