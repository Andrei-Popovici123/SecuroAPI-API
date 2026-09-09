import { Component, computed, inject } from '@angular/core';
import { AuthStore } from '../../../core/auth/auth.store';
import { AuthService } from '../../../core/auth/auth.service';
import { StatusPill } from '../../../../Common/status-pill/status-pill';
import { userStatusMeta } from '../../../../Common/status-pill/status-meta';

@Component({
  selector: 'app-pending',
  standalone: true,
  imports: [StatusPill],
  templateUrl: './pending.html',
  styleUrl: './pending.css',
})
export class Pending {
  readonly store = inject(AuthStore);
  private readonly auth = inject(AuthService);

  readonly meta = computed(() =>
    userStatusMeta(this.store.status() ?? undefined)
  );

  // Once approved server-side, the security stamp bumps → this /me call 401s →
  // the error interceptor clears the token and redirects to login. That IS the
  // "you're approved, sign in again" path.
  refresh(): void {
    this.auth.refreshMe().subscribe();
  }

  logout(): void {
    this.auth.logout();
  }
}
