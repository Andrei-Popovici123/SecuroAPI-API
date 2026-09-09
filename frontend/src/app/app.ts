import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from '../Common/navbar/navbar';
import { Sidebar } from '../Common/sidebar/sidebar';
import { AuthStore } from './core/auth/auth.store';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, Navbar, Sidebar],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  private readonly store = inject(AuthStore);

  // bare pages before login; navbar-only while pending; full console when approved
  readonly showChrome = computed(() => this.store.isAuthenticated());
  readonly showSidebar = computed(() => this.store.isApproved());
}
