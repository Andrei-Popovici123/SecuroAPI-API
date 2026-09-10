import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { catchError, forkJoin, map, of, switchMap } from 'rxjs';
import { ApiRegistryApi } from '../../core/api/api-registry.api';
import { RatingApi } from '../../core/api/rating.api';
import { MonitoringApi } from '../../core/api/monitoring.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { ScoreDial } from '../../../Common/score-dial/score-dial';
import { TrendChart, TrendPoint } from '../../../Common/trend-chart/trend-chart';
import { Spinner } from '../../../Common/spinner/spinner';
import { StatusPill } from '../../../Common/status-pill/status-pill';
import { apiStatusMeta } from '../../../Common/status-pill/status-meta';
import type { APIRegistryDTO, MonitoredEndpointDto, RatingDto } from '../../core/models';

interface EndpointView {
  ep: MonitoredEndpointDto;
  latency: TrendPoint[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe, RouterLink, ScoreDial, TrendChart, Spinner, StatusPill],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  private readonly registryApi = inject(ApiRegistryApi);
  private readonly ratingApi = inject(RatingApi);
  private readonly monitoringApi = inject(MonitoringApi);

  readonly apis = signal<APIRegistryDTO[]>([]);
  readonly selectedId = signal<string | null>(null);
  readonly ratings = signal<RatingDto[]>([]);
  readonly endpoints = signal<EndpointView[]>([]);

  readonly loadingApis = signal(true);
  readonly loadingDetail = signal(false);
  readonly error = signal('');

  readonly selected = computed(
    () => this.apis().find((a) => a.apiid === this.selectedId()) ?? null,
  );
  readonly statusMeta = computed(() => apiStatusMeta(this.selected()?.status));
  readonly displayUrl = computed(() => this.hostOf(this.selected()?.targetURL));

  // ratings oldest → newest for the trend lines
  private readonly ordered = computed(() =>
    [...this.ratings()].sort((a, b) => +new Date(a.createdAt) - +new Date(b.createdAt)).slice(-10),
  );
  readonly latest = computed(() => this.ordered().at(-1) ?? null);
  readonly overallTrend = computed<TrendPoint[]>(() =>
    this.ordered().map((r, i) => ({ x: i, y: r.overallScore, label: this.short(r.createdAt) })),
  );
  readonly vulnTrend = computed<TrendPoint[]>(() =>
    this.ordered().map((r, i) => ({
      x: i,
      y: r.vulnerabilityScore,
      label: this.short(r.createdAt),
    })),
  );
  readonly lastTwo = computed(() =>
    [...this.ratings()].sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt)).slice(0, 5),
  );

  constructor() {
    this.loadApis();
  }

  loadApis(): void {
    this.loadingApis.set(true);
    this.error.set('');
    this.registryApi.myApis().subscribe({
      next: (apis) => {
        const sorted = [...apis].sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt));
        this.apis.set(sorted);
        this.loadingApis.set(false);
        if (sorted.length) this.select(sorted[0].apiid); // latest first
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loadingApis.set(false);
      },
    });
  }

  select(apiid: string): void {
    this.selectedId.set(apiid);
    this.loadingDetail.set(true);
    this.error.set('');
    this.ratings.set([]);
    this.endpoints.set([]);

    forkJoin({
      ratings: this.ratingApi.allByApiId(apiid).pipe(catchError(() => of([] as RatingDto[]))),
      endpoints: this.monitoringApi
        .dashboardEndpoints(apiid)
        .pipe(catchError(() => of([] as MonitoredEndpointDto[]))),
    })
      .pipe(
        switchMap(({ ratings, endpoints }) => {
          this.ratings.set(ratings);
          const top5 = endpoints.slice(0, 5); // max 5
          if (!top5.length) return of([] as EndpointView[]);
          return forkJoin(
            top5.map((ep) =>
              this.monitoringApi.series(ep.endpointId, 1).pipe(
                // 7d
                map((s) => ({
                  ep,
                  latency: (s.points ?? [])
                    .slice()
                    .sort((a, b) => +new Date(a.checkedAt) - +new Date(b.checkedAt))
                    .map((p, i) => ({ x: i, y: p.latencyMs, label: this.short(p.checkedAt) })),
                })),
                catchError(() => of({ ep, latency: [] as TrendPoint[] })),
              ),
            ),
          );
        }),
      )
      .subscribe({
        next: (views) => {
          this.endpoints.set(views);
          this.loadingDetail.set(false);
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.loadingDetail.set(false);
        },
      });
  }

  onSelect(e: Event): void {
    this.select((e.target as HTMLSelectElement).value);
  }

  hostOf(url?: string): string {
    return (url ?? '').replace(/^https?:\/\//, '');
  }

  dot(ep: MonitoredEndpointDto): string {
    if (ep.lastOk === false) return 'var(--color-danger)';
    if (ep.lastOk) return 'var(--color-ok)';
    return 'var(--color-text-dim)';
  }

  private short(iso: string): string {
    return new Date(iso).toLocaleDateString(undefined, { hour: '2-digit', minute: '2-digit' });
  }
}
