import { Component, inject } from '@angular/core';
import { AuthStore } from '../../core/auth/auth.store';

// PLACEHOLDER — the real dashboard (score + trend hero) is its own pass.
@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div style="margin:2rem">
      <h1>Dashboard</h1>
      <p>Signed in as {{ store.name() }} ({{ store.status() }})</p>
    </div>
  `,
})
export class Dashboard {
  readonly store = inject(AuthStore);
}
