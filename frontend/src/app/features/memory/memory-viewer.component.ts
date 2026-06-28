import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MemoryService } from '../../core/services/memory.service';
import { MemoryItem } from '../../core/models/memory.models';

@Component({
  selector: 'app-memory-viewer',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="memory-viewer">
      <a [routerLink]="['/ideas', ideaId()]">← Back to workspace</a>
      <h2>Memories</h2>
      @if (loading()) { <p>Loading memories...</p> }
      @else if (memories().length === 0) { <p>No memories stored yet.</p> }
      @else {
        <ul>
          @for (memory of memories(); track memory.id) {
            <li>
              <div class="meta">{{ memory.scope }} · {{ memory.type }}</div>
              <p>{{ memory.content }}</p>
              <button type="button" (click)="remove(memory.id)">Delete</button>
            </li>
          }
        </ul>
      }
    </section>
  `,
  styles: [`
    .memory-viewer { background: #fff; border: 1px solid #ddd; border-radius: 8px; padding: 1.5rem; }
    ul { list-style: none; padding: 0; }
    li { border: 1px solid #eee; border-radius: 8px; padding: 1rem; margin-bottom: 0.75rem; }
    .meta { font-size: 0.85rem; color: #666; margin-bottom: 0.35rem; }
    button { margin-top: 0.5rem; }
  `]
})
export class MemoryViewerComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly memoryApi = inject(MemoryService);

  ideaId = signal('');
  memories = signal<MemoryItem[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.ideaId.set(id);
    this.memoryApi.list(id).subscribe({
      next: (res) => {
        this.memories.set(res.items);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  remove(id: string): void {
    this.memoryApi.delete(id).subscribe({
      next: () => this.memories.update((items) => items.filter((m) => m.id !== id))
    });
  }
}
