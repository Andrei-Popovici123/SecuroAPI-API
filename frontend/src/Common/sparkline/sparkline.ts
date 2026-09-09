import { Component, computed, input } from '@angular/core';

// Dumb trend line. Give it the values in chronological order (oldest → newest).
@Component({
  selector: 'app-sparkline',
  standalone: true,
  templateUrl: './sparkline.html',
  styleUrl: './sparkline.css',
})
export class Sparkline {
  readonly values = input.required<number[]>();
  readonly width = input(120);
  readonly height = input(32);
  readonly color = input('var(--color-info)');

  // Build an SVG polyline scaled to the data's own min/max, with a little padding.
  readonly points = computed(() => {
    const v = this.values();
    if (v.length === 0) return '';
    const w = this.width();
    const h = this.height();
    const pad = 2;
    const min = Math.min(...v);
    const max = Math.max(...v);
    const span = max - min || 1;
    const stepX = v.length > 1 ? (w - pad * 2) / (v.length - 1) : 0;
    return v
      .map((n, i) => {
        const x = pad + i * stepX;
        const y = pad + (h - pad * 2) * (1 - (n - min) / span);
        return `${x.toFixed(1)},${y.toFixed(1)}`;
      })
      .join(' ');
  });

  readonly hasData = computed(() => this.values().length > 1);
}
