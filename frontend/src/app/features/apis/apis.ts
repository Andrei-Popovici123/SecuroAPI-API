import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, forkJoin, map, of, switchMap } from 'rxjs';
import { ApiRegistryApi } from '../../core/api/api-registry.api';
import { RatingApi } from '../../core/api/rating.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { TargetCard } from '../targets/target-card/target-card';
import { EmptyState } from '../../../Common/empty-state/empty-state';
import { Spinner } from '../../../Common/spinner/spinner';
import type { APIRegistryDTO, RatingDto } from '../../core/models';

interface Card {
  registry: APIRegistryDTO;
  latest: RatingDto | null;
  history: RatingDto[];
}

@Component({
  selector: 'app-apis',
  standalone: true,
  imports: [RouterLink, TargetCard, EmptyState, Spinner],
  templateUrl: './apis.html',
  styleUrl: './apis.css',
})
export class Apis {
  private readonly registry = inject(ApiRegistryApi);
  private readonly rating = inject(RatingApi);

  readonly loading = signal(true);
  readonly error = signal('');
  readonly cards = signal<Card[]>([]);

  readonly scannedCount = computed(() => this.cards().filter((c) => c.latest).length);
  readonly avgScore = computed(() => {
    const scored = this.cards().filter((c) => c.latest);
    if (!scored.length) return null;
    const sum = scored.reduce((a, c) => a + (c.latest!.overallScore ?? 0), 0);
    return Math.round(sum / scored.length);
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    forkJoin({
      apis: this.registry.myApis(),
      latest: this.rating.myLatest(),
    })
      .pipe(
        switchMap(({ apis, latest }) => {
          if (!apis.length) return of([] as Card[]);
          // NOTE: N+1 (one history call per target). Fine at thesis scale; a
          // batch endpoint or GROUP BY is the scale answer (backend open item).
          return forkJoin(
            apis.map((api) =>
              this.rating.allByApiId(api.apiid).pipe(
                map((history) => this.toCard(api, latest, history)),
                catchError(() => of(this.toCard(api, latest, []))),
              ),
            ),
          );
        }),
      )
      .subscribe({
        next: (cards) => {
          this.cards.set(cards);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.loading.set(false);
        },
      });
  }

  private toCard(api: APIRegistryDTO, latest: RatingDto[], history: RatingDto[]): Card {
    return {
      registry: api,
      latest: latest.find((r) => r.apiid === api.apiid) ?? null,
      history,
    };
  }
}
