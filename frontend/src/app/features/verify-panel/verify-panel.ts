import { Component, computed, inject, input, output, signal } from '@angular/core';
import { ApiRegistryApi } from '../../core/api/api-registry.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { Spinner } from '../../../Common/spinner/spinner';
import type { APIRegistryDTO, VerificationStatusDTO } from '../../core/models';

@Component({
  selector: 'app-verify-panel',
  standalone: true,
  imports: [Spinner],
  templateUrl: './verify-panel.html',
  styleUrl: './verify-panel.css',
})
export class VerifyPanel {
  private readonly api = inject(ApiRegistryApi);

  readonly registry = input.required<APIRegistryDTO>();
  readonly verified = output<void>(); // tell the parent to reload the registry

  readonly loading = signal(false);
  readonly error = signal('');
  readonly result = signal<VerificationStatusDTO | null>(null);
  readonly copied = signal<'name' | 'value' | null>(null);

  readonly isVerified = computed(() => !!this.registry().verifiedAt);

  readonly host = computed(() => {
    try {
      return new URL(this.registry().targetURL ?? '').host;
    } catch {
      return '';
    }
  });

  // The record the user must add. Backend also echoes these on a failed check.
  readonly recordName = computed(() =>
    this.host() ? `_securoapi.${this.host()}` : ''
  );
  readonly recordValue = computed(
    () => `securoapi-verify=${this.registry().verificationToken ?? ''}`
  );

  async copy(which: 'name' | 'value'): Promise<void> {
    const text = which === 'name' ? this.recordName() : this.recordValue();
    try {
      await navigator.clipboard.writeText(text);
      this.copied.set(which);
      setTimeout(() => this.copied.set(null), 1500);
    } catch {
      /* clipboard blocked — user can select manually */
    }
  }

  verify(): void {
    this.loading.set(true);
    this.error.set('');
    this.result.set(null);
    this.api.verify(this.registry().apiid).subscribe({
      next: (res) => {
        this.result.set(res);
        this.loading.set(false);
        if (res.verified) this.verified.emit();
      },
      // real errors (30s cooldown, not-found) come back 4xx-with-string
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loading.set(false);
      },
    });
  }
}
