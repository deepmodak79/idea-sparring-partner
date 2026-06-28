import { Component, inject, OnInit, signal } from '@angular/core';
import { ApiService } from '../../core/services/api.service';
import { getApiErrorMessage } from '../../core/utils/api-error.util';

@Component({
  selector: 'app-health',
  standalone: true,
  template: `
    <section class="card">
      <h2>API Health</h2>
      @if (loading()) {
        <p>Checking backend...</p>
      } @else if (error()) {
        <p class="error">{{ error() }}</p>
      } @else {
        <p><strong>API:</strong> {{ apiStatus() }}</p>
        <p><strong>Database:</strong> {{ dbStatus() }}</p>
      }
    </section>
  `,
  styles: [`
    .card { padding: 1rem; border: 1px solid #ddd; border-radius: 8px; margin-top: 1rem; }
    .error { color: #b00020; }
  `]
})
export class HealthComponent implements OnInit {
  private readonly api = inject(ApiService);
  loading = signal(true);
  error = signal<string | null>(null);
  apiStatus = signal('unknown');
  dbStatus = signal('unknown');

  ngOnInit(): void {
    this.api.getHealth().subscribe({
      next: (res) => this.apiStatus.set(res.status),
      error: (err) => {
        this.error.set(getApiErrorMessage(err, 'Backend API is not reachable.'));
        this.loading.set(false);
      }
    });

    this.api.getDatabaseHealth().subscribe({
      next: (res) => {
        this.dbStatus.set(res.database);
        this.loading.set(false);
      },
      error: (err) => {
        this.dbStatus.set(getApiErrorMessage(err, 'Database health check failed.'));
        this.loading.set(false);
      }
    });
  }
}
