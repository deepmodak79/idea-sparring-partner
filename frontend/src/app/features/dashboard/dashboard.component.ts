import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { IdeaService } from '../../core/services/idea.service';
import { Idea } from '../../core/models/idea.models';
import { getApiErrorMessage } from '../../core/utils/api-error.util';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="dashboard">
      <div class="header-row">
        <div>
          <h2>Your Ideas</h2>
          <p>Welcome, {{ userName() }}</p>
        </div>
        <button type="button" (click)="logout()">Log out</button>
      </div>

      <form class="create-form" [formGroup]="form" (ngSubmit)="createIdea()">
        <h3>Create a new idea</h3>
        <label>Title<input formControlName="title" placeholder="AI meal planner for busy parents" /></label>
        <label>Description<textarea rows="4" formControlName="description"></textarea></label>
        @if (error()) { <p class="error">{{ error() }}</p> }
        <button type="submit" [disabled]="form.invalid || creating()">Create idea</button>
      </form>

      @if (loadError()) { <p class="error">{{ loadError() }}</p> }
      @if (loading()) {
        <p>Loading ideas...</p>
      } @else if (ideas().length === 0) {
        <p class="empty">No ideas yet. Create your first one above.</p>
      } @else {
        <ul class="idea-list">
          @for (idea of ideas(); track idea.id) {
            <li>
              <a [routerLink]="['/ideas', idea.id]">
                <strong>{{ idea.title }}</strong>
                <span>{{ idea.threads.length }} threads</span>
              </a>
            </li>
          }
        </ul>
      }
    </section>
  `,
  styles: [`
    .dashboard { background: #fff; border: 1px solid #ddd; border-radius: 8px; padding: 1.5rem; }
    .header-row { display: flex; justify-content: space-between; align-items: start; gap: 1rem; margin-bottom: 1.5rem; }
    .create-form { border: 1px solid #eee; border-radius: 8px; padding: 1rem; margin-bottom: 1.5rem; background: #fafafa; }
    label { display: block; margin-bottom: 0.75rem; }
    input, textarea { width: 100%; padding: 0.5rem; margin-top: 0.25rem; }
    button { padding: 0.5rem 1rem; border: 1px solid #ccc; border-radius: 6px; background: #2563eb; color: #fff; border-color: #2563eb; }
    .header-row button { background: #fff; color: #333; }
    .idea-list { list-style: none; padding: 0; margin: 0; }
    .idea-list li { border: 1px solid #eee; border-radius: 6px; margin-bottom: 0.5rem; }
    .idea-list a { display: flex; justify-content: space-between; padding: 0.75rem 1rem; text-decoration: none; color: inherit; }
    .error { color: #b00020; }
    .empty { color: #666; }
  `]
})
export class DashboardComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly ideasApi = inject(IdeaService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  userName = signal('User');
  ideas = signal<Idea[]>([]);
  loading = signal(true);
  creating = signal(false);
  loadError = signal<string | null>(null);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', Validators.required]
  });

  ngOnInit(): void {
    if (this.auth.currentUser()) {
      this.userName.set(this.auth.currentUser()!.displayName);
    } else {
      this.auth.loadMe().subscribe({
        next: (user) => this.userName.set(user.displayName),
        error: () => this.auth.logout()
      });
    }
    this.loadIdeas();
  }

  loadIdeas(): void {
    this.loadError.set(null);
    this.ideasApi.list().subscribe({
      next: (res) => {
        this.ideas.set(res.items);
        this.loading.set(false);
      },
      error: (err) => {
        this.loadError.set(getApiErrorMessage(err, 'Failed to load your ideas.'));
        this.loading.set(false);
      }
    });
  }

  createIdea(): void {
    if (this.form.invalid) return;
    this.creating.set(true);
    this.error.set(null);
    this.ideasApi.create(this.form.getRawValue()).subscribe({
      next: (idea) => this.router.navigate(['/ideas', idea.id]),
      error: (err) => {
        this.error.set(getApiErrorMessage(err, 'Failed to create idea. AI opening challenges may be unavailable.'));
        this.creating.set(false);
      },
      complete: () => this.creating.set(false)
    });
  }

  logout(): void {
    this.auth.logout();
  }
}
