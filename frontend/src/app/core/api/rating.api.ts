import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type { RatingDto, ScoreReportDto } from '../models';

@Injectable({ providedIn: 'root' })
export class RatingApi {
  private readonly http = inject(HttpClient);
  private readonly ratings = `${environment.apiBaseUrl}/Rating`;
  private readonly reports = `${environment.apiBaseUrl}/ScoreReport`;

  // Dashboard: latest rating per target.
  myLatest(): Observable<RatingDto[]> {
    return this.http.get<RatingDto[]>(`${this.ratings}/myLatest`);
  }

  // Trend history for one target.
  allByApiId(apiId: string): Observable<RatingDto[]> {
    return this.http.get<RatingDto[]>(`${this.ratings}/allByApiId/${apiId}`);
  }

  getById(id: string): Observable<RatingDto> {
    return this.http.get<RatingDto>(`${this.ratings}/${id}`);
  }

  // The findings list for a rating.
  reportsByRatingId(ratingId: string): Observable<ScoreReportDto[]> {
    return this.http.get<ScoreReportDto[]>(
      `${this.reports}/allByRatingId/${ratingId}`
    );
  }
}
