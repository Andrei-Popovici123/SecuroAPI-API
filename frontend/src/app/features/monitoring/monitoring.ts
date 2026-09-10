import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, forkJoin, map, of, switchMap } from 'rxjs';
import { ApiRegistryApi } from '../../core/api/api-registry.api';
import { MonitoringApi } from '../../core/api/monitoring.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { Sparkline } from '../../../Common/sparkline/sparkline';
import { Spinner } from '../../../Common/spinner/spinner';
import type { MonitoredEndpointDto } from '../../core/models';

interface EndpointRow {
  ep: MonitoredEndpointDto;
  host: string;
  latency: number[];
}

@Component({
  selector: 'app-monitoring',
  standalone: true,
  imports: [Sparkline, Spinner],
  templateUrl: './monitoring.html',
  styleUrl: './monitoring.css',
})
export class Monitoring {
  private readonly registryApi = inject(ApiRegistryApi);
  private readonly monitoringApi = inject(MonitoringApi);
  private readonly router = inject(Router);

  readonly rows = signal<EndpointRow[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    this.registryApi
      .myApis()
      .pipe(
        switchMap((apis) => {
          if (!apis.length) return of([] as EndpointRow[]);
          return forkJoin(
            apis.map((a) => {
              const host = (a.targetURL ?? '').replace(/^https?:\/\//, '');
              return this.monitoringApi.listForApi(a.apiid).pipe(
                map((eps) => eps.map((ep) => ({ ep, host, latency: [] as number[] }))),
                catchError(() => of([] as EndpointRow[])),
              );
            }),
          ).pipe(map((groups) => groups.flat()));
        }),
      )
      .subscribe({
        next: (rows) => {
          this.rows.set(rows);
          this.loading.set(false);
          rows.forEach((r) => this.loadSeries(r)); // fill sparklines after paint
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.loading.set(false);
        },
      });
  }

  private loadSeries(row: EndpointRow): void {
    this.monitoringApi
      .series(row.ep.endpointId, 1) // last hour
      .pipe(catchError(() => of(null)))
      .subscribe((s) => {
        if (!s) return;
        const latency = (s.points ?? [])
          .slice()
          .sort((a, b) => +new Date(a.checkedAt) - +new Date(b.checkedAt))
          .map((p) => p.latencyMs);
        this.rows.set(
          this.rows().map((r) => (r.ep.endpointId === row.ep.endpointId ? { ...r, latency } : r)),
        );
      });
  }

  dot(ep: MonitoredEndpointDto): string {
    if (ep.lastOk === false) return 'var(--color-danger)';
    if (ep.lastOk) return 'var(--color-ok)';
    return 'var(--color-text-dim)';
  }

  open(row: EndpointRow): void {
    this.router.navigate(['/targets', row.ep.apiid], { queryParams: { tab: 'monitoring' } });
  }
}
