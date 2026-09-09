import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ApiRegistryApi } from '../../../core/api/api-registry.api';
import { extractErrorMessage } from '../../../core/http/http-error';
import { Spinner } from '../../../../Common/spinner/spinner';

@Component({
  selector: 'app-target-register',
  standalone: true,
  imports: [RouterLink, Spinner],
  templateUrl: './target-register.html',
  styleUrl: './target-register.css',
})
export class TargetRegister {
  private readonly api = inject(ApiRegistryApi);
  private readonly router = inject(Router);

  readonly targetURL = signal('');
  readonly authType = signal('None');
  readonly loading = signal(false);
  readonly error = signal('');

  readonly authOptions = ['None', 'Bearer', 'API Key', 'Basic'];

  val(e: Event): string {
    return (e.target as HTMLInputElement).value;
  }

  // light client check; the backend is authoritative (rejects bare IPs, non-http)
  readonly valid = () => /^https?:\/\/.+/i.test(this.targetURL().trim());

  submit(): void {
    if (!this.valid()) return;
    this.loading.set(true);
    this.error.set('');
    this.api
      .create({
        targetURL: this.targetURL().trim(),
        authType: this.authType(),
      })
      .subscribe({
        next: (created) => {
          // straight to detail — that's where the DNS verification lives
          this.router.navigate(['/targets', created.apiid]);
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.loading.set(false);
        },
      });
  }
}
