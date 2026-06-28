import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class KeepAliveService {
  private timerId?: ReturnType<typeof setInterval>;

  constructor(private readonly http: HttpClient) {}

  start(): void {
    const intervalMs = environment.keepAliveIntervalMs;
    if (!environment.production || !intervalMs || intervalMs <= 0) {
      return;
    }

    const url = `${environment.apiBaseUrl}/health`;
    const ping = () => {
      this.http.get(url).subscribe({
        next: () => {
          // Keep-alive ping succeeded; no UI needed.
        },
        error: () => {
          // Ignore errors — service may be cold-starting on free tier.
        }
      });
    };

    ping();
    this.timerId = setInterval(ping, intervalMs);
  }

  stop(): void {
    if (this.timerId !== undefined) {
      clearInterval(this.timerId);
      this.timerId = undefined;
    }
  }
}
