import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthStore } from '../../app/core/auth/auth.store';
import { AuthService } from '../../app/core/auth/auth.service';
import { StatusPill } from '../status-pill/status-pill';
import { userStatusMeta } from '../status-pill/status-meta';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, StatusPill],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  readonly store = inject(AuthStore);
  private readonly auth = inject(AuthService);

  readonly statusMeta = computed(() => userStatusMeta(this.store.status() ?? undefined));

  readonly initials = computed(() => {
    const parts = (this.store.name() ?? '').trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase() || 'U';
  });
}
