import { Component, computed, input } from '@angular/core';
import type { Tone } from './status-meta';

// Dumb pill: caller passes a label + tone. Map enums via status-meta.ts helpers.
@Component({
  selector: 'app-status-pill',
  standalone: true,
  templateUrl: './status-pill.html',
  styleUrl: './status-pill.css',
})
export class StatusPill {
  readonly label = input.required<string>();
  readonly tone = input<Tone>('neutral');

  // dot + text share the tone colour; bg is a faint tint of it
  readonly cls = computed(() => `tone-${this.tone()}`);
}
