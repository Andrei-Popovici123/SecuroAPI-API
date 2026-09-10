import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiRegistryApi } from '../../../core/api/api-registry.api';
import { extractErrorMessage } from '../../../core/http/http-error';
import { StatusPill } from '../../../../Common/status-pill/status-pill';
import { Spinner } from '../../../../Common/spinner/spinner';
import { apiStatusMeta } from '../../../../Common/status-pill/status-meta';
import { VerifyPanel } from '../../verify-panel/verify-panel';
import { TestConfigPanel } from '../../test-config-panel/test-config-panel';
import { ScansPanel } from '../../scans-panel/scans-panel';
import { MonitoringPanel } from '../../monitoring-panel/monitoring-panel';
import type { APIRegistryDTO } from '../../../core/models';

type Tab = 'verify' | 'config' | 'scans' | 'monitoring';

@Component({
  selector: 'app-target-detail',
  standalone: true,
  imports: [
    RouterLink,
    StatusPill,
    Spinner,
    VerifyPanel,
    TestConfigPanel,
    ScansPanel,
    MonitoringPanel,
  ],
  templateUrl: './target-detail.html',
  styleUrl: './target-detail.css',
})
export class TargetDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ApiRegistryApi);

  readonly id = signal('');
  readonly registry = signal<APIRegistryDTO | null>(null);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly tab = signal<Tab>('verify');

  readonly statusMeta = computed(() => apiStatusMeta(this.registry()?.status));
  readonly isVerified = computed(() => !!this.registry()?.verifiedAt);
  readonly displayUrl = computed(() =>
    (this.registry()?.targetURL ?? '').replace(/^https?:\/\//, ''),
  );

  readonly tabs: { key: Tab; label: string }[] = [
    { key: 'verify', label: 'Verification' },
    { key: 'config', label: 'Test config' },
    { key: 'scans', label: 'Scans' },
    { key: 'monitoring', label: 'Monitoring' },
  ];

  ngOnInit(): void {
    this.id.set(this.route.snapshot.paramMap.get('id') ?? '');
    const wanted = this.route.snapshot.queryParamMap.get('tab') as Tab | null;
    this.load(wanted);
  }

  load(preferredTab: Tab | null = null): void {
    this.loading.set(true);
    this.error.set('');
    this.api.getById(this.id()).subscribe({
      next: (r) => {
        this.registry.set(r);
        this.loading.set(false);
        if (preferredTab) this.tab.set(preferredTab);
        else if (r.verifiedAt) this.tab.set('config');
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loading.set(false);
      },
    });
  }

  setTab(t: Tab): void {
    this.tab.set(t);
  }

  onVerified(): void {
    this.load();
  }
}
