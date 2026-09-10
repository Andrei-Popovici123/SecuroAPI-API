import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthStore } from '../../app/core/auth/auth.store';
import { AuthService } from '../../app/core/auth/auth.service';
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})
export class Sidebar {
  readonly store = inject(AuthStore);
  private readonly auth = inject(AuthService);

  logout(): void {
    this.auth.logout();
  }
}
