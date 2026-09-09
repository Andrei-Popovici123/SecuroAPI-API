import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type {
  CreateMonitoredEndpointDto,
  MonitoredEndpointDto,
  MonitoringSummaryDto,
  ProbeResult,
  TelemetrySeriesDto,
} from '../models';

@Injectable({ providedIn: 'root' })
export class MonitoringApi {
  private readonly http = inject(HttpClient);
  private readonly mon = `${environment.apiBaseUrl}/Monitoring`;
  private readonly dash = `${environment.apiBaseUrl}/MonitoringDashboard`;

  // ---- endpoint management ----
  listForApi(apiId: string): Observable<MonitoredEndpointDto[]> {
    return this.http.get<MonitoredEndpointDto[]>(`${this.mon}/api/${apiId}`);
  }

  create(
    apiId: string,
    dto: CreateMonitoredEndpointDto
  ): Observable<MonitoredEndpointDto> {
    return this.http.post<MonitoredEndpointDto>(`${this.mon}/api/${apiId}`, dto);
  }

  delete(endpointId: string): Observable<void> {
    return this.http.delete<void>(`${this.mon}/${endpointId}`);
  }

  setActive(
    endpointId: string,
    isActive: boolean
  ): Observable<MonitoredEndpointDto> {
    return this.http.patch<MonitoredEndpointDto>(
      `${this.mon}/${endpointId}/active`,
      {},
      { params: new HttpParams().set('isActive', isActive) }
    );
  }

  probe(endpointId: string): Observable<ProbeResult> {
    return this.http.post<ProbeResult>(`${this.mon}/${endpointId}/probe`, {});
  }

  // ---- dashboard reads ----
  summary(apiId: string, windowHours = 24): Observable<MonitoringSummaryDto> {
    return this.http.get<MonitoringSummaryDto>(
      `${this.dash}/api/${apiId}/summary`,
      { params: new HttpParams().set('windowHours', windowHours) }
    );
  }

  dashboardEndpoints(apiId: string): Observable<MonitoredEndpointDto[]> {
    return this.http.get<MonitoredEndpointDto[]>(
      `${this.dash}/api/${apiId}/endpoints`
    );
  }

  series(
    endpointId: string,
    windowHours = 24
  ): Observable<TelemetrySeriesDto> {
    return this.http.get<TelemetrySeriesDto>(
      `${this.dash}/endpoint/${endpointId}/series`,
      { params: new HttpParams().set('windowHours', windowHours) }
    );
  }
}
