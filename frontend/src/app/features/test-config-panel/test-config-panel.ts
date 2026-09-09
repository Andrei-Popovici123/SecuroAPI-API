import { Component, OnInit, computed, inject, input, signal } from '@angular/core';
import { catchError, forkJoin, of } from 'rxjs';
import { TestConfigApi } from '../../core/api/test-config.api';
import { extractErrorMessage } from '../../core/http/http-error';
import { SeverityBadge } from '../../../Common/severity-badge/severity-badge';
import { Spinner } from '../../../Common/spinner/spinner';
import { OwaspCategoryLabel } from '../../core/models';
import type { CatalogEntry, OwaspCategory } from '../../core/models';

@Component({
  selector: 'app-test-config-panel',
  standalone: true,
  imports: [SeverityBadge, Spinner],
  templateUrl: './test-config-panel.html',
  styleUrl: './test-config-panel.css',
})
export class TestConfigPanel implements OnInit {
  private readonly api = inject(TestConfigApi);

  readonly apiid = input.required<string>();

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal('');
  readonly savedOk = signal(false);

  readonly catalog = signal<CatalogEntry[]>([]);
  readonly selected = signal<Set<string>>(new Set());
  private readonly configId = signal<string | null>(null);

  readonly count = computed(() => this.selected().size);
  readonly canSave = computed(() => this.count() >= 1 && this.count() <= 10);

  // group catalog entries by OWASP category for a tidier picker
  readonly groups = computed(() => {
    const byCat = new Map<number, CatalogEntry[]>();
    for (const e of this.catalog()) {
      const list = byCat.get(e.category) ?? [];
      list.push(e);
      byCat.set(e.category, list);
    }
    return [...byCat.entries()].map(([category, entries]) => ({
      category,
      label: OwaspCategoryLabel[category as OwaspCategory] ?? 'Other',
      entries,
    }));
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    forkJoin({
      catalog: this.api.catalog(),
      // no config yet → 404; treat as an empty selection, not an error
      config: this.api.byApiId(this.apiid()).pipe(catchError(() => of(null))),
    }).subscribe({
      next: ({ catalog, config }) => {
        this.catalog.set(catalog);
        if (config) {
          this.configId.set(config.configId);
          this.selected.set(new Set(config.enabledTestIds ?? []));
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.loading.set(false);
      },
    });
  }

  isSelected(id: string): boolean {
    return this.selected().has(id);
  }

  toggle(id: string): void {
    const next = new Set(this.selected());
    next.has(id) ? next.delete(id) : next.add(id);
    this.selected.set(next);
    this.savedOk.set(false);
  }

  save(): void {
    if (!this.canSave()) return;
    this.saving.set(true);
    this.error.set('');
    this.savedOk.set(false);
    const enabledTestIds = [...this.selected()];
    const id = this.configId();

    const req$ = id
      ? this.api.update(id, { enabledTestIds })
      : this.api.create({ apiid: this.apiid(), enabledTestIds });

    req$.subscribe({
      next: (cfg) => {
        this.configId.set(cfg.configId);
        this.saving.set(false);
        this.savedOk.set(true);
      },
      error: (err) => {
        this.error.set(extractErrorMessage(err));
        this.saving.set(false);
      },
    });
  }
}
