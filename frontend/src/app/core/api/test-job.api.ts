import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type { TestJobDto, TestRunTriggeredDto } from '../models';

@Injectable({ providedIn: 'root' })
export class TestJobApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/TestJob`;

  // Triggers a scan. Returns {jobId, jobStatus:"Queued"} (jobStatus is a STRING).
  // Gate failures (cooldown, no config, active job) come back 4xx-with-string.
  publish(apiId: string): Observable<TestRunTriggeredDto> {
    return this.http.post<TestRunTriggeredDto>(
      `${this.base}/publish/${apiId}`,
      {}
    );
  }

  myJobs(): Observable<TestJobDto[]> {
    return this.http.get<TestJobDto[]>(`${this.base}/myJobs`);
  }

  // Poll this until status is 3 (Completed) or 4 (Failed) — see isTerminal().
  getById(id: string): Observable<TestJobDto> {
    return this.http.get<TestJobDto>(`${this.base}/${id}`);
  }

  allByApiId(apiId: string): Observable<TestJobDto[]> {
    return this.http.get<TestJobDto[]>(`${this.base}/allByApiId/${apiId}`);
  }

  // Refuses Queued/Running jobs server-side.
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
