import { Component, inject } from '@angular/core';
import { AuthStore } from '../../core/auth/auth.store';
import { AuthService } from '../../core/auth/auth.service';

// PLACEHOLDER — shows current status; approval will 401 the token → re-login.
@Component({
  selector: 'app-pending',
  standalone: true,
  template: `
    <div style="margin:4rem auto;max-width:420px;display:grid;gap:.5rem">
      <h1>Awaiting approval</h1>
      <p>Status: {{ store.status() ?? 'unknown' }}</p>
      <button (click)="refresh()">Refresh status</button>
    </div>
  `,
})
export class Pending {
  readonly store = inject(AuthStore);
  private readonly auth = inject(AuthService);
  refresh(): void {
    this.auth.refreshMe().subscribe();
  }
}
