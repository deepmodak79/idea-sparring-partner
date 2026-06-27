import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface HealthResponse {
  status: string;
  service: string;
  timestamp: string;
}

export interface DatabaseHealthResponse {
  status: string;
  database: string;
  timestamp: string;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  readonly baseUrl = environment.apiBaseUrl;

  getHealth(): Observable<HealthResponse> {
    return this.http.get<HealthResponse>(`${this.baseUrl}/health`);
  }

  getDatabaseHealth(): Observable<DatabaseHealthResponse> {
    return this.http.get<DatabaseHealthResponse>(`${this.baseUrl}/health/database`);
  }
}
