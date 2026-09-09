import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { AdminApi } from '../../core/api/admin.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { StatusPill } from '../../../Common/status-pill/status-pill';
import { Spinner } from '../../../Common/spinner/spinner';
import { apiStatusMeta, userStatusMeta } from '../../../Common/status-pill/status-meta';
import type { APIRegistryDTO, GetRegisteredUserDTO } from '../../core/models';

type Tab = 'users' | 'apis';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [DatePipe, StatusPill, Spinner],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class Admin implements OnInit {
  private readonly api = inject(AdminApi);

  readonly tab = signal<Tab>('users');
  readonly users = signal<GetRegisteredUserDTO[]>([]);
  readonly apis = signal<APIRegistryDTO[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly busy = signal<Record<string, boolean>>({});

  readonly userMeta = userStatusMeta;
  readonly apiMeta = apiStatusMeta;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    forkJoin({
      users: this.api.pendingUsers(),
      apis: this.api.pendingApis(),
    }).subscribe({
      next: ({ users, apis }) => {
        this.users.set(users);
        this.apis.set(apis);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loading.set(false);
      },
    });
  }

  displayUrl(u?: string): string {
    return (u ?? '').replace(/^https?:\/\//, '');
  }

  approveUser(u: GetRegisteredUserDTO): void {
    this.act(u.id!, this.api.approveUser(u.id!), () => this.dropUser(u.id!));
  }
  rejectUser(u: GetRegisteredUserDTO): void {
    this.act(u.id!, this.api.rejectUser(u.id!), () => this.dropUser(u.id!));
  }
  approveApi(a: APIRegistryDTO): void {
    this.act(a.apiid, this.api.approveApi(a.apiid), () => this.dropApi(a.apiid));
  }
  rejectApi(a: APIRegistryDTO): void {
    this.act(a.apiid, this.api.rejectApi(a.apiid), () => this.dropApi(a.apiid));
  }

  private act(id: string, req$: import('rxjs').Observable<void>, ok: () => void): void {
    this.setBusy(id, true);
    this.error.set('');
    req$.subscribe({
      next: () => ok(),
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.setBusy(id, false);
      },
    });
  }

  private dropUser(id: string): void {
    this.users.set(this.users().filter((u) => u.id !== id));
  }
  private dropApi(id: string): void {
    this.apis.set(this.apis().filter((a) => a.apiid !== id));
  }
  private setBusy(id: string, v: boolean): void {
    this.busy.set({ ...this.busy(), [id]: v });
  }
}
