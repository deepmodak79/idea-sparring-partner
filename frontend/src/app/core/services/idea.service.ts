import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateIdeaRequest, Idea } from '../models/idea.models';

@Injectable({ providedIn: 'root' })
export class IdeaService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/ideas`;

  list(): Observable<{ items: Idea[] }> {
    return this.http.get<{ items: Idea[] }>(this.baseUrl);
  }

  get(id: string): Observable<Idea> {
    return this.http.get<Idea>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateIdeaRequest): Observable<Idea> {
    return this.http.post<Idea>(this.baseUrl, request);
  }
}
