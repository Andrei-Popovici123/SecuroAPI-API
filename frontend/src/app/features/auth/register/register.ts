import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { extractErrorMessage } from '../../../core/http/http-error';
import { Spinner } from '../../../../Common/spinner/spinner';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, Spinner],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private readonly auth = inject(AuthService);

  readonly email = signal('');
  readonly password = signal('');
  readonly firstName = signal('');
  readonly lastName = signal('');
  readonly companyName = signal('');

  readonly loading = signal(false);
  readonly error = signal('');
  readonly done = signal(false);

  val(e: Event): string {
    return (e.target as HTMLInputElement).value;
  }

  readonly valid = () =>
    !!this.email() &&
    this.password().length >= 12 && // backend policy: 12-char minimum
    !!this.firstName() &&
    !!this.lastName();

  submit(): void {
    if (!this.valid()) return;
    this.loading.set(true);
    this.error.set('');
    this.auth
      .register({
        email: this.email(),
        password: this.password(),
        firstName: this.firstName(),
        lastName: this.lastName(),
        companyName: this.companyName(),
      })
      .subscribe({
        next: () => {
          this.done.set(true);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.loading.set(false);
        },
      });
  }
}
