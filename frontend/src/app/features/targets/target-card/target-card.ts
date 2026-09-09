import { Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ScoreDial } from '../../../../Common/score-dial/score-dial';
import { Sparkline } from '../../../../Common/sparkline/sparkline';
import { StatusPill } from '../../../../Common/status-pill/status-pill';
import { apiStatusMeta } from '../../../../Common/status-pill/status-meta';
import type { APIRegistryDTO, RatingDto } from '../../../core/models';

// One card per registered target. `history` is oldest→newest ratings for the
// trend line; `latest` drives the dial. Either may be absent (never scanned).
@Component({
  selector: 'app-target-card',
  standalone: true,
  imports: [RouterLink, ScoreDial, Sparkline, StatusPill],
  templateUrl: './target-card.html',
  styleUrl: './target-card.css',
})
export class TargetCard {
  readonly registry = input.required<APIRegistryDTO>();
  readonly latest = input<RatingDto | null>(null);
  readonly history = input<RatingDto[]>([]);

  readonly statusMeta = computed(() =>
    apiStatusMeta(this.registry().status)
  );

  // score trend, oldest → newest
  readonly trend = computed(() =>
    [...this.history()]
      .sort((a, b) => +new Date(a.createdAt) - +new Date(b.createdAt))
      .map((r) => r.overallScore)
  );

  readonly scanned = computed(() => this.latest() !== null);

  // strip scheme for a tidier display of the target URL
  readonly displayUrl = computed(() =>
    (this.registry().targetURL ?? '').replace(/^https?:\/\//, '')
  );
}
