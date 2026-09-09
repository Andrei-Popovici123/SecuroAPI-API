import {
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  input,
  signal,
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { forkJoin, switchMap, takeWhile, timer } from 'rxjs';
import { TestJobApi } from '../../core/api/test-job.api';
import { RatingApi } from '../../core/api/rating.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { JobStatus, isTerminal } from '../../core/models';
import { ScoreDial } from '../../../Common/score-dial/score-dial';
import { SeverityBadge } from '../../../Common/severity-badge/severity-badge';
import { StatusPill } from '../../../Common/status-pill/status-pill';
import { Spinner } from '../../../Common/spinner/spinner';
import { jobStatusMeta } from '../../../Common/status-pill/status-meta';
import type {
  APIRegistryDTO,
  RatingDto,
  ScoreReportDto,
  TestJobDto,
} from '../../core/models';

@Component({
  selector: 'app-scans-panel',
  standalone: true,
  imports: [DatePipe, ScoreDial, SeverityBadge, StatusPill, Spinner],
  templateUrl: './scans-panel.html',
  styleUrl: './scans-panel.css',
})
export class ScansPanel implements OnInit {
  private readonly testJob = inject(TestJobApi);
  private readonly ratingApi = inject(RatingApi);
  private readonly destroyRef = inject(DestroyRef);

  readonly registry = input.required<APIRegistryDTO>();

  readonly jobs = signal<TestJobDto[]>([]);
  readonly running = signal(false);
  readonly loading = signal(true);
  readonly error = signal('');

  // findings view for the selected/last-completed job
  readonly rating = signal<RatingDto | null>(null);
  readonly reports = signal<ScoreReportDto[]>([]);
  readonly loadingFindings = signal(false);
  readonly selectedJobId = signal<string | null>(null);

  readonly canScan = computed(
    () => this.registry().status === 'Approved' && !this.running()
  );

  readonly jobMeta = jobStatusMeta;

  ngOnInit(): void {
    this.loadJobs(true);
  }

  loadJobs(resume = false): void {
    this.loading.set(true);
    this.testJob.allByApiId(this.registry().apiid).subscribe({
      next: (jobs) => {
        const sorted = [...jobs].sort(
          (a, b) => +new Date(b.createdAt) - +new Date(a.createdAt)
        );
        this.jobs.set(sorted);
        this.loading.set(false);
        if (resume) {
          // a scan already in flight (e.g. page reload mid-scan) → resume polling
          const active = sorted.find(
            (j) => j.status === JobStatus.Queued || j.status === JobStatus.Running
          );
          if (active) {
            this.running.set(true);
            this.poll(active.jobId);
          }
        }
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loading.set(false);
      },
    });
  }

  runScan(): void {
    if (!this.canScan()) return;
    this.error.set('');
    this.running.set(true);
    // gate/cooldown refusals come back 4xx-with-string → surface verbatim
    this.testJob.publish(this.registry().apiid).subscribe({
      next: (t) => this.poll(t.jobId),
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.running.set(false);
      },
    });
  }

  private poll(jobId: string): void {
    timer(0, 3000)
      .pipe(
        switchMap(() => this.testJob.getById(jobId)),
        // emit the terminal value too, then complete
        takeWhile((job) => !isTerminal(job.status), true),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: (job) => {
          this.upsertJob(job);
          if (isTerminal(job.status)) {
            this.running.set(false);
            if (job.status === JobStatus.Completed && job.ratingId) {
              this.viewFindings(job);
            }
          }
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.running.set(false);
        },
      });
  }

  private upsertJob(job: TestJobDto): void {
    const list = [...this.jobs()];
    const i = list.findIndex((j) => j.jobId === job.jobId);
    if (i >= 0) list[i] = job;
    else list.unshift(job);
    this.jobs.set(list);
  }

  viewFindings(job: TestJobDto): void {
    if (!job.ratingId) return;
    this.selectedJobId.set(job.jobId);
    this.loadingFindings.set(true);
    this.error.set('');
    forkJoin({
      rating: this.ratingApi.getById(job.ratingId),
      reports: this.ratingApi.reportsByRatingId(job.ratingId),
    }).subscribe({
      next: ({ rating, reports }) => {
        this.rating.set(rating);
        this.reports.set(
          [...reports].sort((a, b) => b.severity - a.severity)
        );
        this.loadingFindings.set(false);
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loadingFindings.set(false);
      },
    });
  }

  readonly Completed = JobStatus.Completed;
}
