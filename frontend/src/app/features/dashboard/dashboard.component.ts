import { Component, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <section class="dashboard">
      <div class="header-row">
        <div>
          <h2>Your Ideas</h2>
          <p>Welcome, {{ userName() }}</p>
        </div>
        <button type="button" (click)="logout()">Log out</button>
      </div>
      <p class="placeholder">Idea dashboard will load here.</p>
    </section>
  `,
  styles: [`
    .dashboard { background: #fff; border: 1px solid #ddd; border-radius: 8px; padding: 1.5rem; }
    .header-row { display: flex; justify-content: space-between; align-items: start; gap: 1rem; }
    button { padding: 0.5rem 1rem; border: 1px solid #ccc; border-radius: 6px; background: #fff; }
    .placeholder { color: #666; }
  `]
})
export class DashboardComponent implements OnInit {
  private readonly auth = inject(AuthService);
  userName = signal('User');

  ngOnInit(): void {
    if (this.auth.currentUser()) {
      this.userName.set(this.auth.currentUser()!.displayName);
    } else {
      this.auth.loadMe().subscribe({
        next: (user) => this.userName.set(user.displayName),
        error: () => this.auth.logout()
      });
    }
  }

  logout(): void {
    this.auth.logout();
  }
}
