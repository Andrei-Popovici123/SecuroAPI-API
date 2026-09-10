import { Injectable, computed, inject, signal } from '@angular/core';
import { TokenStorage } from './token-storage';
import { decodeJwt, getRoles, isExpired, ROLE_ADMIN } from './jwt';
import type { GetRegisteredUserDTO, UserStatus } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly storage = inject(TokenStorage);

  private readonly _token = signal<string | null>(this.storage.get());
  private readonly _user = signal<GetRegisteredUserDTO | null>(null);

  readonly token = this._token.asReadonly();
  readonly user = this._user.asReadonly();

  readonly claims = computed(() => {
    const t = this._token();
    return t ? decodeJwt(t) : null;
  });

  readonly isAuthenticated = computed(() => !!this.claims() && !isExpired(this.claims()));

  readonly userId = computed(() => this.claims()?.sub ?? null);
  readonly email = computed(() => this._user()?.email ?? this.claims()?.email ?? null);
  readonly name = computed(() => this.claims()?.name ?? null);

  // Prefer the freshest source: /me profile, then the token claim.
  readonly status = computed<UserStatus | null>(
    () => this._user()?.status ?? (this.claims()?.status as UserStatus) ?? null,
  );

  readonly roles = computed(() => getRoles(this.claims()));
  readonly isAdmin = computed(() => this.roles().includes(ROLE_ADMIN));
  readonly isApproved = computed(() => this.status() === 'Approved');

  setToken(token: string): void {
    this.storage.set(token);
    this._token.set(token);
  }

  setUser(user: GetRegisteredUserDTO | null): void {
    this._user.set(user);
  }

  clear(): void {
    this.storage.clear();
    this._token.set(null);
    this._user.set(null);
  }
}
