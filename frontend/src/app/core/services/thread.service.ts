import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Message, PostMessageResponse, ThreadItem } from '../models/thread.models';

@Injectable({ providedIn: 'root' })
export class ThreadService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  listForIdea(ideaId: string): Observable<{ items: ThreadItem[] }> {
    return this.http.get<{ items: ThreadItem[] }>(`${this.baseUrl}/ideas/${ideaId}/threads`);
  }

  getMessages(threadId: string): Observable<{ items: Message[] }> {
    return this.http.get<{ items: Message[] }>(`${this.baseUrl}/threads/${threadId}/messages`);
  }

  postMessage(threadId: string, content: string): Observable<PostMessageResponse> {
    return this.http.post<PostMessageResponse>(`${this.baseUrl}/threads/${threadId}/messages`, { content });
  }
}
