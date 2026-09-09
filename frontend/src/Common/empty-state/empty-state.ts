import { Component, input } from '@angular/core';

// An empty screen is an invitation to act — title + message + optional CTA slot.
@Component({
  selector: 'app-empty-state',
  standalone: true,
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.css',
})
export class EmptyState {
  readonly title = input.required<string>();
  readonly message = input<string>('');
}
