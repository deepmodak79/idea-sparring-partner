import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { IdeaService } from '../../core/services/idea.service';
import { ThreadService } from '../../core/services/thread.service';
import { Idea } from '../../core/models/idea.models';
import { ThreadItem } from '../../core/models/thread.models';
import { ThreadPanelComponent } from './thread-panel.component';

@Component({
  selector: 'app-idea-workspace',
  standalone: true,
  imports: [RouterLink, ThreadPanelComponent],
  template: `
    <section class="workspace">
      <div class="top-row">
        <a routerLink="/dashboard">← Back to dashboard</a>
        <div class="actions">
          <a [routerLink]="['/ideas', idea()?.id, 'memories']">Memories</a>
          <button type="button" (click)="generateSynthesis()" [disabled]="synthesisLoading()">Generate synthesis</button>
        </div>
      </div>

      @if (loading()) {
        <p>Loading workspace...</p>
      } @else if (idea()) {
        <header class="idea-header">
          <h2>{{ idea()!.title }}</h2>
          <p>{{ idea()!.description }}</p>
        </header>

        @if (synthesisError()) { <p class="error">{{ synthesisError() }}</p> }
        @if (latestSynthesis()) {
          <section class="synthesis-preview">
            <h3>Latest synthesis (v{{ latestSynthesis()!.version }})</h3>
            <p><strong>Strongest challenges:</strong> {{ latestSynthesis()!.strongestChallenges.join(' | ') }}</p>
          </section>
        }

        <div class="panels">
          @for (thread of threads(); track thread.id) {
            <app-thread-panel [thread]="thread" />
          }
        </div>
      }
    </section>
  `,
  styles: [`
    .workspace { padding: 0.5rem; }
    .top-row { display: flex; justify-content: space-between; align-items: center; gap: 1rem; margin-bottom: 1rem; }
    .actions { display: flex; gap: 0.75rem; align-items: center; }
    .idea-header { background: #fff; border: 1px solid #ddd; border-radius: 8px; padding: 1rem; margin-bottom: 1rem; }
    .panels { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 1rem; }
    .synthesis-preview { background: #eef2ff; border: 1px solid #c7d2fe; border-radius: 8px; padding: 1rem; margin-bottom: 1rem; }
    .error { color: #b00020; }
    @media (max-width: 900px) { .panels { grid-template-columns: 1fr; } }
  `]
})
export class IdeaWorkspaceComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly ideasApi = inject(IdeaService);
  private readonly threadsApi = inject(ThreadService);

  idea = signal<Idea | null>(null);
  threads = signal<ThreadItem[]>([]);
  loading = signal(true);
  synthesisLoading = signal(false);
  synthesisError = signal<string | null>(null);
  latestSynthesis = signal<{ version: number; strongestChallenges: string[] } | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.ideasApi.get(id).subscribe({
      next: (idea) => {
        this.idea.set(idea);
        this.loadThreads(id);
      },
      error: () => this.loading.set(false)
    });
  }

  loadThreads(ideaId: string): void {
    this.threadsApi.listForIdea(ideaId).subscribe({
      next: (res) => {
        this.threads.set(res.items);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  generateSynthesis(): void {
    // wired in commit 16
  }
}
