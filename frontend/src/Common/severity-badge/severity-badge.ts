import { Component, computed, input } from '@angular/core';
import { Severity, SeverityLabel } from '../../app/core/models';

// severity is the Severity enum INT (1..4) as it arrives on the wire.
@Component({
  selector: 'app-severity-badge',
  standalone: true,
  templateUrl: './severity-badge.html',
  styleUrl: './severity-badge.css',
})
export class SeverityBadge {
  readonly severity = input.required<number>();

  readonly label = computed(
    () => SeverityLabel[this.severity() as Severity] ?? 'Unknown'
  );
  readonly cls = computed(() => `sev-${this.severity()}`);
}
