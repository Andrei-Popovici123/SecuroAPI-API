export const Severity = { Low: 1, Medium: 2, High: 3, Critical: 4 } as const;
export type Severity = (typeof Severity)[keyof typeof Severity];

export const JobStatus = {
  Queued: 1,
  Running: 2,
  Completed: 3,
  Failed: 4,
} as const;
export type JobStatus = (typeof JobStatus)[keyof typeof JobStatus];

// OWASP categories
export const OwaspCategory = {
  BrokenAccessControl: 1,
  CryptographicFailures: 2,
  InsecureDesign: 4,
  SecurityMisconfiguration: 5,
  SSRF: 10,
} as const;
export type OwaspCategory =
  (typeof OwaspCategory)[keyof typeof OwaspCategory];

// ---- string enums ----

export type ApiStatus = 'Inactive' | 'Pending' | 'Approved';
export type UserStatus = 'New' | 'Pending' | 'Approved' | 'Banned';

// ---- display labels ----

export const SeverityLabel: Record<Severity, string> = {
  [Severity.Low]: 'Low',
  [Severity.Medium]: 'Medium',
  [Severity.High]: 'High',
  [Severity.Critical]: 'Critical',
};

export const JobStatusLabel: Record<JobStatus, string> = {
  [JobStatus.Queued]: 'Queued',
  [JobStatus.Running]: 'Running',
  [JobStatus.Completed]: 'Completed',
  [JobStatus.Failed]: 'Failed',
};

export const OwaspCategoryLabel: Record<OwaspCategory, string> = {
  [OwaspCategory.BrokenAccessControl]: 'A01 Broken Access Control',
  [OwaspCategory.CryptographicFailures]: 'A02 Cryptographic Failures',
  [OwaspCategory.InsecureDesign]: 'A04 Insecure Design',
  [OwaspCategory.SecurityMisconfiguration]: 'A05 Security Misconfiguration',
  [OwaspCategory.SSRF]: 'A10 SSRF',
};

// A job is done (terminal) — stop polling.
// A job is done (terminal) — stop polling.
export const isTerminal = (s: number): boolean =>
  s === JobStatus.Completed || s === JobStatus.Failed;
