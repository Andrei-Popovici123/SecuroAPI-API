import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { catchError, forkJoin, map, of } from 'rxjs';
import { TestJobApi } from '../../core/api/test-job.api';
import { RatingApi } from '../../core/api/rating.api';
import { ApiRegistryApi } from '../../core/api/api-registry.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { JobStatus } from '../../core/models';
import { StatusPill } from '../../../Common/status-pill/status-pill';
import { Spinner } from '../../../Common/spinner/spinner';
import { jobStatusMeta } from '../../../Common/status-pill/status-meta';
import type { RatingDto, ScoreReportDto, TestJobDto } from '../../core/models';

interface ScanRow {
  job: TestJobDto;
  host: string;
  rating: RatingDto | null;
  findings: number | null;
}

@Component({
  selector: 'app-scans',
  standalone: true,
  imports: [DatePipe, StatusPill, Spinner],
  templateUrl: './scans.html',
  styleUrl: './scans.css',
})
export class Scans {
  private readonly jobApi = inject(TestJobApi);
  private readonly ratingApi = inject(RatingApi);
  private readonly registryApi = inject(ApiRegistryApi);
  private readonly router = inject(Router);

  readonly rows = signal<ScanRow[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly jobMeta = jobStatusMeta;

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    forkJoin({
      jobs: this.jobApi.myJobs(),
      apis: this.registryApi.myApis(),
    })
      .pipe(
        map(({ jobs, apis }) => {
          const hostById = new Map(
            apis.map((a) => [a.apiid, (a.targetURL ?? '').replace(/^https?:\/\//, '')]),
          );
          const last50 = [...jobs]
            .sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt))
            .slice(0, 50);
          return { last50, hostById };
        }),
      )
      .subscribe({
        next: ({ last50, hostById }) => {
          // build rows; fetch Rating + findings for completed jobs (N+1, thesis-scale)
          if (!last50.length) {
            this.rows.set([]);
            this.loading.set(false);
            return;
          }
          forkJoin(
            last50.map((job) => {
              const host = hostById.get(job.apiid) ?? job.apiid;
              if (job.status !== JobStatus.Completed || !job.ratingId) {
                return of<ScanRow>({ job, host, rating: null, findings: null });
              }
              return forkJoin({
                rating: this.ratingApi.getById(job.ratingId).pipe(catchError(() => of(null))),
                reports: this.ratingApi
                  .reportsByRatingId(job.ratingId)
                  .pipe(catchError(() => of([] as ScoreReportDto[]))),
              }).pipe(
                map(({ rating, reports }) => ({
                  job,
                  host,
                  rating,
                  findings: reports.length,
                })),
              );
            }),
          ).subscribe({
            next: (rows) => {
              this.rows.set(rows);
              this.loading.set(false);
            },
            error: (err) => {
              this.error.set(extractErrorMessage(err));
              this.loading.set(false);
            },
          });
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.loading.set(false);
        },
      });
  }

  open(row: ScanRow): void {
    this.router.navigate(['/targets', row.job.apiid], { queryParams: { tab: 'scans' } });
  }
}
