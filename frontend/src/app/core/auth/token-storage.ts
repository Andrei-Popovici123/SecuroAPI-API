import { Injectable } from '@angular/core';

const KEY = 'securoapi_token';

@Injectable({ providedIn: 'root' })
export class TokenStorage {
  get(): string | null {
    return localStorage.getItem(KEY);
  }
  set(token: string): void {
    localStorage.setItem(KEY, token);
  }
  clear(): void {
    localStorage.removeItem(KEY);
  }
}
