import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { extractErrorMessage } from '../../core/http/http-error';

// PLACEHOLDER — unstyled, exists to prove the plumbing works end-to-end.
// Replace with the real login screen in the auth feature pass.
@Component({
  selector: 'app-login',
  standalone: true,
  template: `
    <div style="max-width:320px;margin:4rem auto;display:grid;gap:.5rem">
      <h1>Sign in</h1>
      <input
        placeholder="email"
        [value]="email()"
        (input)="email.set(val($event))"
      />
      <input
        placeholder="password"
        type="password"
        [value]="password()"
        (input)="password.set(val($event))"
      />
      <button (click)="submit()" [disabled]="loading()">
        {{ loading() ? '…' : 'Sign in' }}
      </button>
      @if (error()) {
        <p style="color:#DC2626">{{ error() }}</p>
      }
    </div>
  `,
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly email = signal('');
  readonly password = signal('');
  readonly loading = signal(false);
  readonly error = signal('');

  val(e: Event): string {
    return (e.target as HTMLInputElement).value;
  }

  submit(): void {
    this.loading.set(true);
    this.error.set('');
    this.auth
      .login({ email: this.email(), password: this.password() })
      .subscribe({
        next: (user) => {
          const returnUrl =
            this.route.snapshot.queryParamMap.get('returnUrl') ?? '/dashboard';
          this.router.navigateByUrl(
            user.status === 'Approved' ? returnUrl : '/pending'
          );
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.loading.set(false);
        },
      });
  }
}
