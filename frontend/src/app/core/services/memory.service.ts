import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MemoryItem } from '../models/memory.models';

@Injectable({ providedIn: 'root' })
export class MemoryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/memories`;

  list(ideaId?: string): Observable<{ items: MemoryItem[] }> {
    const query = ideaId ? `?ideaId=${ideaId}` : '';
    return this.http.get<{ items: MemoryItem[] }>(`${this.baseUrl}${query}`);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
