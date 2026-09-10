import {
  afterNextRender,
  Component,
  computed,
  ElementRef,
  inject,
  input,
  signal,
} from '@angular/core';

export interface TrendPoint {
  x: number; // index or timestamp-ms; only order matters
  y: number;
  label?: string; // optional x-axis tick label (e.g. a date)
}

@Component({
  selector: 'app-trend-chart',
  standalone: true,
  templateUrl: './trend-chart.html',
  styleUrl: './trend-chart.css',
})
export class TrendChart {
  readonly points = input.required<TrendPoint[]>();
  readonly color = input('var(--color-info)');
  readonly yUnit = input(''); // 'ms', '', etc.
  readonly height = input(160);
  readonly yMin = input<number | null>(null); // pin (e.g. 0) or auto
  readonly yMax = input<number | null>(null); // pin (e.g. 100) or auto
  readonly compact = input(false); // small mode for latency minis

  private readonly host = inject(ElementRef<HTMLElement>);
  private readonly _w = signal(300);
  private get W() { return this._w(); }

// viewBox width (scales via CSS)
  private readonly padL = 34; // room for y labels
  private readonly padR = 8;
  private readonly padT = 8;
  private readonly padB = 14; // room for x labels

  readonly H = computed(() => this.height());
  constructor() {
    afterNextRender(() => {
      const ro = new ResizeObserver(([e]) => {
        const w = Math.round(e.contentRect.width);
        if (w > 0) this._w.set(w);
      });
      ro.observe(this.host.nativeElement);
    });
  }

  private readonly plotW = computed(() => this.W - this.padL - this.padR);
  private readonly plotH = computed(() => this.H() - this.padT - this.padB);

  readonly hasData = computed(() => this.points().length > 0);
  readonly single = computed(() => this.points().length === 1);

  // y-domain: pinned or padded-auto
  private readonly domain = computed(() => {
    const ys = this.points().map((p) => p.y);
    let lo = this.yMin() ?? Math.min(...ys);
    let hi = this.yMax() ?? Math.max(...ys);
    if (lo === hi) {
      lo -= 1;
      hi += 1;
    } // avoid /0
    if (this.yMin() === null) lo = Math.floor(lo - (hi - lo) * 0.1);
    if (this.yMax() === null) hi = Math.ceil(hi + (hi - lo) * 0.1);
    return { lo, hi };
  });

  private sx(i: number): number {
    const n = this.points().length;
    const step = n > 1 ? this.plotW() / (n - 1) : 0;
    return this.padL + i * step;
  }
  private sy(v: number): number {
    const { lo, hi } = this.domain();
    return this.padT + this.plotH() * (1 - (v - lo) / (hi - lo));
  }

  // the line (flat across for a single point)
  readonly path = computed(() => {
    const pts = this.points();
    if (!pts.length) return '';
    if (pts.length === 1) {
      const y = this.sy(pts[0].y);
      return `M ${this.padL} ${y} L ${this.padL + this.plotW()} ${y}`;
    }
    return pts
      .map((p, i) => `${i ? 'L' : 'M'} ${this.sx(i).toFixed(1)} ${this.sy(p.y).toFixed(1)}`)
      .join(' ');
  });

  readonly dots = computed(() =>
    this.single()
      ? [{ cx: this.padL + this.plotW() / 2, cy: this.sy(this.points()[0].y) }]
      : this.points().map((p, i) => ({ cx: this.sx(i), cy: this.sy(p.y) })),
  );

  // y gridlines + labels (3 in compact, 4 otherwise)
  readonly yTicks = computed(() => {
    const { lo, hi } = this.domain();
    const n = this.compact() ? 3 : 4;
    return Array.from({ length: n }, (_, i) => {
      const v = lo + ((hi - lo) * i) / (n - 1);
      return { y: this.sy(v), label: this.fmt(v) };
    });
  });

  readonly xTicks = computed(() => {
    const pts = this.points();
    const withLabels = pts.filter((p) => p.label);
    if (withLabels.length < 2 || this.compact()) return [];
    const idxs = [0, Math.floor((pts.length - 1) / 2), pts.length - 1];
    return [...new Set(idxs)].map((i) => ({
      x: this.sx(i),
      label: pts[i].label!,
    }));
  });

  readonly viewBox = computed(() => `0 0 ${this.W} ${this.H()}`);
  readonly plotLeft = this.padL;
  readonly plotRight = computed(() => this.padL + this.plotW());
  readonly plotTop = this.padT;
  readonly plotBottom = computed(() => this.padT + this.plotH());

  private fmt(v: number): string {
    const r = Math.abs(v) >= 100 ? Math.round(v) : Math.round(v * 10) / 10;
    return `${r}${this.yUnit()}`;
  }
}
