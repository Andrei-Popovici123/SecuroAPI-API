import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type {
  CatalogEntry,
  CreateTestConfigDto,
  TestConfigDto,
  UpdateTestConfigDto,
} from '../models';

@Injectable({ providedIn: 'root' })
export class TestConfigApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/TestConfig`;

  catalog(): Observable<CatalogEntry[]> {
    return this.http.get<CatalogEntry[]>(`${this.base}/catalog`);
  }

  byApiId(apiId: string): Observable<TestConfigDto> {
    return this.http.get<TestConfigDto>(`${this.base}/byApiId/${apiId}`);
  }

  create(dto: CreateTestConfigDto): Observable<TestConfigDto> {
    return this.http.post<TestConfigDto>(this.base, dto);
  }

  update(id: string, dto: UpdateTestConfigDto): Observable<TestConfigDto> {
    return this.http.put<TestConfigDto>(`${this.base}/${id}`, dto);
  }
}
