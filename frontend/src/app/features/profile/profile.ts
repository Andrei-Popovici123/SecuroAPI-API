import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { AuthStore } from '../../core/auth/auth.store';
import { AuthService } from '../../core/auth/auth.service';
import { extractErrorMessage } from '../../core/http/http-error';
import { StatusPill } from '../../../Common/status-pill/status-pill';
import { Spinner } from '../../../Common/spinner/spinner';
import { userStatusMeta } from '../../../Common/status-pill/status-meta';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [StatusPill, Spinner],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile implements OnInit {
  readonly store = inject(AuthStore);
  private readonly auth = inject(AuthService);

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal('');
  readonly savedOk = signal(false);

  // editable fields
  readonly firstName = signal('');
  readonly lastName = signal('');
  readonly companyName = signal('');

  readonly meta = computed(() => userStatusMeta(this.store.status() ?? undefined));
  readonly initials = computed(() => {
    const parts = `${this.firstName()} ${this.lastName()}`.trim().split(/\s+/);
    return ((parts[0]?.[0] ?? '') + (parts[1]?.[0] ?? '')).toUpperCase() || 'U';
  });

  readonly valid = computed(() => !!this.firstName().trim() && !!this.lastName().trim());
  readonly dirty = computed(() => {
    const u = this.store.user();
    return (
      this.firstName() !== (u?.firstName ?? '') ||
      this.lastName() !== (u?.lastName ?? '') ||
      this.companyName() !== (u?.companyName ?? '')
    );
  });

  val(e: Event): string {
    return (e.target as HTMLInputElement).value;
  }

  ngOnInit(): void {
    this.auth.refreshMe().subscribe({
      next: () => {
        this.seed();
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loading.set(false);
      },
    });
  }

  private seed(): void {
    const u = this.store.user();
    this.firstName.set(u?.firstName ?? '');
    this.lastName.set(u?.lastName ?? '');
    this.companyName.set(u?.companyName ?? '');
  }

  reset(): void {
    this.seed();
    this.savedOk.set(false);
    this.error.set('');
  }

  save(): void {
    if (!this.valid() || !this.dirty()) return;
    this.saving.set(true);
    this.error.set('');
    this.savedOk.set(false);
    this.auth
      .updateProfile({
        firstName: this.firstName().trim(),
        lastName: this.lastName().trim(),
        companyName: this.companyName().trim(),
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.savedOk.set(true);
        },
        error: (err) => {
          this.error.set(extractErrorMessage(err));
          this.saving.set(false);
        },
      });
  }

  logout(): void {
    this.auth.logout();
  }
}
