

import type { ApiStatus, UserStatus } from './enums';

// ---- auth ----
export interface RegisterUserDTO {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  companyName: string;
}
export interface LoginUserDTO {
  email: string;
  password: string;
}
export interface GetRegisteredUserDTO {
  id?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  companyName?: string;
  status?: UserStatus; // STRING on the wire
}

// ---- targets ----
export interface CreateAPIRegistryDTO {
  targetURL: string;
  authType?: string;
}
export interface UpdateAPIRegistryDTO {
  targetURL: string;
  authType?: string;
}
export interface APIRegistryDTO {
  apiid: string;
  userID?: string;
  targetURL?: string;
  authType?: string;
  status?: ApiStatus; // STRING on the wire
  createdAt: string;
  lastModifiedAt?: string;
  verificationToken?: string;
  verifiedAt?: string;
}
export interface VerificationStatusDTO {
  apiid: string;
  verified: boolean;
  verifiedAt?: string;
  recordName?: string; // echo back to user on failure
  expectedValue?: string;
  message?: string;
}

// ---- test config ----
export interface CatalogEntry {
  id: string;
  checkId?: string;
  category: number; // OwaspCategory (int)
  name?: string;
  description?: string;
  severity: number; // Severity (int)
}
export interface CreateTestConfigDto {
  apiid: string;
  enabledTestIds: string[]; // 1..10
}
export interface UpdateTestConfigDto {
  enabledTestIds: string[];
}
export interface TestConfigDto {
  configId: string;
  apiid: string;
  enabledTestIds?: string[];
}

// ---- jobs ----
export interface TestRunTriggeredDto {
  jobId: string;
  apiid: string;
  jobStatus?: string; // STRING ("Queued") — not the int enum
}
export interface TestJobDto {
  jobId: string;
  apiid: string;
  status: number; // JobStatus (int) — poll this
  failReason?: string;
  createdAt: string;
  finishedAt?: string;
  userId?: string;
  ratingId?: string;
}

// ---- findings ----
export interface RatingDto {
  ratingId: string;
  vulnerabilityScore: number;
  numberOfTests: number;
  coveragePercent: number;
  overallScore: number;
  apiid: string;
  createdAt: string;
  lastModifiedAt?: string;
}
export interface ScoreReportDto {
  reportId: string;
  severity: number; // Severity (int)
  check?: string;
  summary?: string;
  recommendation?: string;
  evidence?: string;
  ratingId: string;
  finishedAt: string;
}

// ---- monitoring ----
export interface CreateMonitoredEndpointDto {
  url: string;
  label: string;
  intervalSeconds: number; // 60..3600
}
export interface MonitoredEndpointDto {
  endpointId: string;
  apiid: string;
  url?: string;
  label?: string;
  isActive: boolean;
  intervalSeconds: number;
  lastCheckedAt?: string;
  lastOk?: boolean;
  lastStatusCode?: number;
  lastLatencyMs?: number;
  lastErrorType?: string;
  hasHsts: boolean;
  hasCsp: boolean;
  hasNosniff: boolean;
  hasFrameOptions: boolean;
  createdAt: string;
}
export interface ProbeResult {
  ok: boolean;
  statusCode?: number;
  latencyMs: number;
  errorType?: string;
  hsts: boolean;
  csp: boolean;
  nosniff: boolean;
  frameOptions: boolean;
}
export interface MonitoringSummaryDto {
  apiid: string;
  endpointCount: number;
  activeEndpointCount: number;
  endpointsUp: number;
  endpointsDown: number;
  uptimePercent?: number;
  avgLatencyMs?: number;
  p95LatencyMs?: number;
  probeCount: number;
  windowHours: number;
  lastCheckedAt?: string;
}
export interface TelemetrySeriesPointDto {
  checkedAt: string;
  latencyMs: number;
  ok: boolean;
  statusCode?: number;
}
export interface TelemetrySeriesDto {
  endpointId: string;
  label?: string;
  url?: string;
  windowHours: number;
  points?: TelemetrySeriesPointDto[];
}

// ---- anomalies ----
export interface AnomalyLogDto {
  anomalyId: string;
  anomalyType?: string;
  severity: number; // Severity (int)
  notificationSent: boolean;
  timeStamp: string;
  apiid: string;
}
