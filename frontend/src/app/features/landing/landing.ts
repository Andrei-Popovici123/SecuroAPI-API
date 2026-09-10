import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthStore } from '../../core/auth/auth.store';

interface HomeLink {
  path: string;
  label: string;
  desc: string;
  admin?: boolean;
}

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {
  readonly store = inject(AuthStore);

  readonly firstName = computed(() => this.store.name()?.split(' ')[0] ?? 'there');

  readonly links: HomeLink[] = [
    { path: '/apis', label: 'My APIs', desc: 'Your registered targets and their latest scores.' },
    {
      path: '/dashboard',
      label: 'Dashboard',
      desc: 'Score trends and endpoint health at a glance.',
    },
    { path: '/scans', label: 'Scans', desc: 'Run and review security scans.' },
    {
      path: '/monitoring',
      label: 'Monitoring',
      desc: 'Track uptime and latency of your endpoints.',
    },
    { path: '/admin', label: 'Admin', desc: 'Approve pending users and targets.', admin: true },
  ];

  readonly visible = computed(() => this.links.filter((l) => !l.admin || this.store.isAdmin()));
}
