import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Synthesis } from '../models/synthesis.models';

@Injectable({ providedIn: 'root' })
export class SynthesisService {
  private readonly http = inject(HttpClient);

  list(ideaId: string): Observable<{ items: Synthesis[] }> {
    return this.http.get<{ items: Synthesis[] }>(`${environment.apiBaseUrl}/ideas/${ideaId}/syntheses`);
  }

  create(ideaId: string): Observable<Synthesis> {
    return this.http.post<Synthesis>(`${environment.apiBaseUrl}/ideas/${ideaId}/syntheses`, {});
  }
}
