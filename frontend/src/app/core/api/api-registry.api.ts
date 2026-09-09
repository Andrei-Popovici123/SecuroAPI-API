import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type {
  APIRegistryDTO,
  CreateAPIRegistryDTO,
  UpdateAPIRegistryDTO,
  VerificationStatusDTO,
} from '../models';

@Injectable({ providedIn: 'root' })
export class ApiRegistryApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/APIRegistry`;

  myApis(): Observable<APIRegistryDTO[]> {
    return this.http.get<APIRegistryDTO[]>(`${this.base}/myApis`);
  }

  getById(id: string): Observable<APIRegistryDTO> {
    return this.http.get<APIRegistryDTO>(`${this.base}/${id}`);
  }

  create(dto: CreateAPIRegistryDTO): Observable<APIRegistryDTO> {
    return this.http.post<APIRegistryDTO>(this.base, dto);
  }

  // PUT nulls verification server-side (the old proof was for a different host).
  update(id: string, dto: UpdateAPIRegistryDTO): Observable<APIRegistryDTO> {
    return this.http.put<APIRegistryDTO>(`${this.base}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  // Runs the DNS TXT check now; returns recordName/expectedValue to show on fail.
  verify(id: string): Observable<VerificationStatusDTO> {
    return this.http.post<VerificationStatusDTO>(`${this.base}/${id}/verify`, {});
  }
}
