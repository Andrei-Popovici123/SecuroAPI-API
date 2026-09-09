import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type {
  GetRegisteredUserDTO,
  LoginUserDTO,
  RegisterUserDTO,
} from '../models';

@Injectable({ providedIn: 'root' })
export class AuthApi {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/Auth`;

  register(dto: RegisterUserDTO): Observable<GetRegisteredUserDTO> {
    return this.http.post<GetRegisteredUserDTO>(`${this.base}/register`, dto);
  }

  // login returns the RAW JWT string — must be responseType 'text', or
  // HttpClient tries to JSON.parse it and throws.
  login(dto: LoginUserDTO): Observable<string> {
    return this.http.post(`${this.base}/login`, dto, { responseType: 'text' });
  }

  me(): Observable<GetRegisteredUserDTO> {
    return this.http.get<GetRegisteredUserDTO>(`${this.base}/me`);
  }
}
