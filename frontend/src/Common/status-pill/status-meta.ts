import { JobStatus, JobStatusLabel } from '../../app/core/models';
import type { ApiStatus, UserStatus } from '../../app/core/models';

// Tone drives the pill colour. Presentational, so it lives in shared/, not core/.
export type Tone = 'ok' | 'warn' | 'danger' | 'info' | 'neutral';

export interface StatusMeta {
  label: string;
  tone: Tone;
}

export function jobStatusMeta(status: number): StatusMeta {
  switch (status) {
    case JobStatus.Completed:
      return { label: JobStatusLabel[JobStatus.Completed], tone: 'ok' };
    case JobStatus.Running:
      return { label: JobStatusLabel[JobStatus.Running], tone: 'info' };
    case JobStatus.Queued:
      return { label: JobStatusLabel[JobStatus.Queued], tone: 'neutral' };
    case JobStatus.Failed:
      return { label: JobStatusLabel[JobStatus.Failed], tone: 'danger' };
    default:
      return { label: 'Unknown', tone: 'neutral' };
  }
}

export function apiStatusMeta(status: ApiStatus | undefined): StatusMeta {
  switch (status) {
    case 'Approved':
      return { label: 'Approved', tone: 'ok' };
    case 'Pending':
      return { label: 'Pending', tone: 'warn' };
    case 'Inactive':
      return { label: 'Inactive', tone: 'neutral' };
    default:
      return { label: 'Unknown', tone: 'neutral' };
  }
}

export function userStatusMeta(status: UserStatus | undefined): StatusMeta {
  switch (status) {
    case 'Approved':
      return { label: 'Approved', tone: 'ok' };
    case 'Pending':
      return { label: 'Pending', tone: 'warn' };
    case 'New':
      return { label: 'New', tone: 'info' };
    case 'Banned':
      return { label: 'Banned', tone: 'danger' };
    default:
      return { label: 'Unknown', tone: 'neutral' };
  }
}
