import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, switchMap, tap } from 'rxjs';
import { AuthApi } from '../api/auth.api';
import { AuthStore } from './auth.store';
import type {
  GetRegisteredUserDTO,
  LoginUserDTO,
  RegisterUserDTO,
} from '../models';


@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(AuthApi);
  private readonly store = inject(AuthStore);
  private readonly router = inject(Router);

  // login → store raw token → immediately fetch /me for the freshest status.
  login(dto: LoginUserDTO): Observable<GetRegisteredUserDTO> {
    return this.api.login(dto).pipe(
      tap((token) => this.store.setToken(token)),
      switchMap(() => this.api.me()),
      tap((user) => this.store.setUser(user))
    );
  }

  register(dto: RegisterUserDTO): Observable<GetRegisteredUserDTO> {
    return this.api.register(dto);
  }

  // Refresh the cached profile (e.g. on the pending screen, or app bootstrap).
  refreshMe(): Observable<GetRegisteredUserDTO> {
    return this.api.me().pipe(tap((user) => this.store.setUser(user)));
  }

  logout(): void {
    this.store.clear();
    this.router.navigate(['/login']);
  }
}
