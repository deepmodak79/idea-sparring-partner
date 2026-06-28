import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { IdeaService } from '../../core/services/idea.service';
import { ThreadService } from '../../core/services/thread.service';
import { Idea } from '../../core/models/idea.models';
import { ThreadItem } from '../../core/models/thread.models';
import { ThreadPanelComponent } from './thread-panel.component';
import { SynthesisService } from '../../core/services/synthesis.service';
import { Synthesis } from '../../core/models/synthesis.models';
import { getApiErrorMessage } from '../../core/utils/api-error.util';

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

      @if (loadError()) { <p class="error">{{ loadError() }}</p> }
      @else if (loading()) {
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
            <p><strong>Strongest challenges:</strong></p>
            <ul>
              @for (item of latestSynthesis()!.strongestChallenges; track item) {
                <li>{{ item }}</li>
              }
            </ul>
            <p><strong>Weakest reasoning:</strong></p>
            <ul>
              @for (item of latestSynthesis()!.weakestReasoning; track item) {
                <li>{{ item }}</li>
              }
            </ul>
            <p><strong>Unresolved tensions:</strong></p>
            <ul>
              @for (item of latestSynthesis()!.unresolvedTensions; track item) {
                <li>{{ item }}</li>
              }
            </ul>
          </section>
        }

        <div class="panels">
          @for (thread of threads(); track thread.id; let i = $index) {
            <app-thread-panel [thread]="thread" [loadOrder]="i" />
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
  private readonly synthesisApi = inject(SynthesisService);

  idea = signal<Idea | null>(null);
  threads = signal<ThreadItem[]>([]);
  loadError = signal<string | null>(null);
  loading = signal(true);
  synthesisLoading = signal(false);
  synthesisError = signal<string | null>(null);
  latestSynthesis = signal<Synthesis | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.ideasApi.get(id).subscribe({
      next: (idea) => {
        this.idea.set(idea);
        this.loadThreads(id);
        this.loadSyntheses(id);
      },
      error: (err) => {
        this.loadError.set(getApiErrorMessage(err, 'Failed to load this idea.'));
        this.loading.set(false);
      }
    });
  }

  loadThreads(ideaId: string): void {
    this.threadsApi.listForIdea(ideaId).subscribe({
      next: (res) => {
        this.threads.set(res.items);
        this.loading.set(false);
      },
      error: (err) => {
        this.loadError.set(getApiErrorMessage(err, 'Failed to load threads for this idea.'));
        this.loading.set(false);
      }
    });
  }

  loadSyntheses(ideaId: string): void {
    this.synthesisApi.list(ideaId).subscribe({
      next: (res) => {
        const latest = res.items.at(-1) ?? null;
        this.latestSynthesis.set(latest);
      }
    });
  }

  generateSynthesis(): void {
    const ideaId = this.idea()?.id;
    if (!ideaId) return;

    this.synthesisLoading.set(true);
    this.synthesisError.set(null);
    this.synthesisApi.create(ideaId).subscribe({
      next: (synthesis) => {
        this.latestSynthesis.set(synthesis);
        this.synthesisLoading.set(false);
      },
      error: (err) => {
        this.synthesisError.set(getApiErrorMessage(err, 'Failed to generate synthesis.'));
        this.synthesisLoading.set(false);
      }
    });
  }
}
