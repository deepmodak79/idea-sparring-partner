import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { IdeaService } from '../../core/services/idea.service';
import { Idea } from '../../core/models/idea.models';

@Component({
  selector: 'app-idea-workspace',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="workspace">
      <a routerLink="/dashboard">← Back to dashboard</a>
      @if (loading()) {
        <p>Loading workspace...</p>
      } @else if (idea()) {
        <h2>{{ idea()!.title }}</h2>
        <p class="description">{{ idea()!.description }}</p>
        <p class="placeholder">Four-panel sparring workspace loads in the next step.</p>
      }
    </section>
  `,
  styles: [`
    .workspace { background: #fff; border: 1px solid #ddd; border-radius: 8px; padding: 1.5rem; }
    .description { color: #555; }
    .placeholder { color: #666; font-style: italic; }
  `]
})
export class IdeaWorkspaceComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly ideasApi = inject(IdeaService);

  idea = signal<Idea | null>(null);
  loading = signal(true);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.ideasApi.get(id).subscribe({
      next: (idea) => {
        this.idea.set(idea);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
