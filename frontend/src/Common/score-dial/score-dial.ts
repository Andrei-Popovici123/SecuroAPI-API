import { Component, computed, input } from '@angular/core';

// score is the OverallScore (1..100); higher = safer, so the colour
// runs green→amber→red as it drops.
@Component({
  selector: 'app-score-dial',
  standalone: true,
  templateUrl: './score-dial.html',
  styleUrl: './score-dial.css',
})
export class ScoreDial {
  readonly score = input.required<number>();
  readonly size = input(160); // px
  readonly label = input('Security score');

  private readonly R = 52;
  readonly circumference = 2 * Math.PI * this.R;

  private readonly clamped = computed(() =>
    Math.max(0, Math.min(100, this.score()))
  );

  readonly dashoffset = computed(
    () => this.circumference * (1 - this.clamped() / 100)
  );

  readonly color = computed(() => {
    const s = this.clamped();
    if (s >= 80) return 'var(--color-ok)';
    if (s >= 50) return 'var(--color-warn)';
    return 'var(--color-danger)';
  });
}
