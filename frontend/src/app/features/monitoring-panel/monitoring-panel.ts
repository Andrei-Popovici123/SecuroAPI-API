import { Component, OnInit, inject, input, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { catchError, forkJoin, of } from 'rxjs';
import { MonitoringApi } from '../../core/api/monitoring.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { Sparkline } from '../../../Common/sparkline/sparkline';
import { Spinner } from '../../../Common/spinner/spinner';
import type {
  MonitoredEndpointDto,
  MonitoringSummaryDto,
  ProbeResult,
} from '../../core/models';

@Component({
  selector: 'app-monitoring-panel',
  standalone: true,
  imports: [DatePipe, Sparkline, Spinner],
  templateUrl: './monitoring-panel.html',
  styleUrl: './monitoring-panel.css',
})
export class MonitoringPanel implements OnInit {
  private readonly api = inject(MonitoringApi);

  readonly apiid = input.required<string>();

  readonly summary = signal<MonitoringSummaryDto | null>(null);
  readonly endpoints = signal<MonitoredEndpointDto[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');

  // add form
  readonly showForm = signal(false);
  readonly url = signal('');
  readonly label = signal('');
  readonly interval = signal(300);
  readonly creating = signal(false);

  readonly intervalOptions = [
    { v: 60, l: '1 min' },
    { v: 300, l: '5 min' },
    { v: 900, l: '15 min' },
    { v: 1800, l: '30 min' },
    { v: 3600, l: '1 hour' },
  ];

  // per-endpoint transient state
  readonly probes = signal<Record<string, ProbeResult>>({});
  readonly series = signal<Record<string, number[]>>({});
  readonly busy = signal<Record<string, boolean>>({});

  val(e: Event): string {
    return (e.target as HTMLInputElement).value;
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    forkJoin({
      summary: this.api.summary(this.apiid()).pipe(catchError(() => of(null))),
      endpoints: this.api.listForApi(this.apiid()),
    }).subscribe({
      next: ({ summary, endpoints }) => {
        this.summary.set(summary);
        this.endpoints.set(endpoints);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loading.set(false);
      },
    });
  }

  readonly canAdd = () =>
    /^https?:\/\/.+/i.test(this.url().trim()) && !!this.label().trim();

  addEndpoint(): void {
    if (!this.canAdd()) return;
    this.creating.set(true);
    this.error.set('');
    this.api
      .create(this.apiid(), {
        url: this.url().trim(),
        label: this.label().trim(),
        intervalSeconds: this.interval(),
      })
      .subscribe({
        next: (ep) => {
          this.endpoints.set([...this.endpoints(), ep]);
          this.url.set('');
          this.label.set('');
          this.showForm.set(false);
          this.creating.set(false);
          this.reloadSummary();
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.creating.set(false);
        },
      });
  }

  toggleActive(ep: MonitoredEndpointDto): void {
    this.setBusy(ep.endpointId, true);
    this.api.setActive(ep.endpointId, !ep.isActive).subscribe({
      next: (updated) => {
        this.replace(updated);
        this.setBusy(ep.endpointId, false);
        this.reloadSummary();
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.setBusy(ep.endpointId, false);
      },
    });
  }

  probe(ep: MonitoredEndpointDto): void {
    this.setBusy(ep.endpointId, true);
    this.api.probe(ep.endpointId).subscribe({
      next: (res) => {
        this.probes.set({ ...this.probes(), [ep.endpointId]: res });
        this.setBusy(ep.endpointId, false);
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.setBusy(ep.endpointId, false);
      },
    });
  }

  loadSeries(ep: MonitoredEndpointDto): void {
    if (this.series()[ep.endpointId]) {
      // toggle off
      const next = { ...this.series() };
      delete next[ep.endpointId];
      this.series.set(next);
      return;
    }
    this.api.series(ep.endpointId).subscribe({
      next: (s) => {
        const values = (s.points ?? [])
          .sort((a, b) => +new Date(a.checkedAt) - +new Date(b.checkedAt))
          .map((p) => p.latencyMs);
        this.series.set({ ...this.series(), [ep.endpointId]: values });
      },
      error: (err) => this.error.set(extractErrorMessage(err)),
    });
  }

  remove(ep: MonitoredEndpointDto): void {
    this.setBusy(ep.endpointId, true);
    this.api.delete(ep.endpointId).subscribe({
      next: () => {
        this.endpoints.set(
          this.endpoints().filter((e) => e.endpointId !== ep.endpointId)
        );
        this.reloadSummary();
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.setBusy(ep.endpointId, false);
      },
    });
  }

  private reloadSummary(): void {
    this.api
      .summary(this.apiid())
      .pipe(catchError(() => of(null)))
      .subscribe((s) => this.summary.set(s));
  }

  private replace(ep: MonitoredEndpointDto): void {
    this.endpoints.set(
      this.endpoints().map((e) => (e.endpointId === ep.endpointId ? ep : e))
    );
  }

  private setBusy(id: string, v: boolean): void {
    this.busy.set({ ...this.busy(), [id]: v });
  }
}
