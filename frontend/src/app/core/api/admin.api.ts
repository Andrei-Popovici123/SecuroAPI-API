import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type { APIRegistryDTO, GetRegisteredUserDTO } from '../models';

@Injectable({ providedIn: 'root' })
export class AdminApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/Admin`;

  users(): Observable<GetRegisteredUserDTO[]> {
    return this.http.get<GetRegisteredUserDTO[]>(`${this.base}/users`);
  }

  pendingUsers(): Observable<GetRegisteredUserDTO[]> {
    return this.http.get<GetRegisteredUserDTO[]>(`${this.base}/users/pending`);
  }

  pendingApis(): Observable<APIRegistryDTO[]> {
    return this.http.get<APIRegistryDTO[]>(`${this.base}/apis/pending`);
  }

  approveUser(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/users/${id}/approve`, {});
  }

  rejectUser(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/users/${id}/reject`, {});
  }

  approveApi(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/apis/${id}/approve`, {});
  }

  // NOTE the capital R — the backend route is /apis/{id}/Reject.
  rejectApi(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/apis/${id}/Reject`, {});
  }
}
