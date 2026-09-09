import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

// PLACEHOLDER — the real landing page is its own feature pass.
@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div style="margin:4rem auto;max-width:520px;display:grid;gap:1rem">
      <h1>SecuroAPI</h1>
      <a routerLink="/login">Sign in</a>
    </div>
  `,
})
export class Landing {}
