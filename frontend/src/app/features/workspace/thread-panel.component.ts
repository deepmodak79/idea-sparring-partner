import { Component, inject, Input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ThreadService } from '../../core/services/thread.service';
import { Message, ThreadItem } from '../../core/models/thread.models';
import { getApiErrorMessage } from '../../core/utils/api-error.util';
import { timer } from 'rxjs';

@Component({
  selector: 'app-thread-panel',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <section class="panel">
      <header>
        <h3>{{ threadInput()?.persona }}</h3>
      </header>
      <div class="messages">
        @if (loadingMessages()) {
          <p class="status">Loading messages...</p>
        } @else if (generatingOpening()) {
          <p class="status">Generating opening challenge...</p>
        } @else if (messages().length === 0) {
          <p class="status">No messages yet.</p>
        } @else {
          @for (message of messages(); track message.id) {
            <div class="message" [class.user]="message.role === 'User'" [class.assistant]="message.role === 'Assistant'">
              <strong>{{ message.role }}</strong>
              <p>{{ message.content }}</p>
            </div>
          }
        }
      </div>
      <form [formGroup]="form" (ngSubmit)="send()">
        <textarea rows="3" formControlName="content" placeholder="Reply to {{ threadInput()?.persona }}..."></textarea>
        @if (error()) { <p class="error">{{ error() }}</p> }
        <button type="submit" [disabled]="form.invalid || sending() || generatingOpening()">Send</button>
      </form>
    </section>
  `,
  styles: [`
    .panel { border: 1px solid #ddd; border-radius: 8px; background: #fff; display: flex; flex-direction: column; min-height: 420px; }
    header { padding: 0.75rem 1rem; border-bottom: 1px solid #eee; background: #f8fafc; }
    header h3 { margin: 0; font-size: 1rem; }
    .messages { flex: 1; overflow-y: auto; padding: 1rem; display: flex; flex-direction: column; gap: 0.75rem; }
    .message { padding: 0.75rem; border-radius: 8px; background: #f3f4f6; }
    .message.user { background: #dbeafe; }
    .message.assistant { background: #fef3c7; }
    .message p { margin: 0.35rem 0 0; white-space: pre-wrap; }
    .status { color: #666; margin: 0; font-style: italic; }
    form { padding: 1rem; border-top: 1px solid #eee; display: flex; flex-direction: column; gap: 0.5rem; }
    textarea { width: 100%; resize: vertical; padding: 0.5rem; }
    button { align-self: flex-end; padding: 0.45rem 0.9rem; background: #2563eb; color: #fff; border: none; border-radius: 6px; }
    .error { color: #b00020; margin: 0; }
  `]
})
export class ThreadPanelComponent {
  @Input({ required: true }) set thread(value: ThreadItem) {
    this.threadInput.set(value);
    this.loadMessages(value.id);
  }

  /** Stagger opening-challenge generation to avoid Gemini rate limits. */
  @Input() loadOrder = 0;

  private readonly threadsApi = inject(ThreadService);
  private readonly fb = inject(FormBuilder);

  threadInput = signal<ThreadItem | null>(null);
  messages = signal<Message[]>([]);
  loadingMessages = signal(false);
  generatingOpening = signal(false);
  sending = signal(false);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({ content: [''] });

  loadMessages(threadId: string): void {
    this.loadingMessages.set(true);
    this.error.set(null);

    this.threadsApi.getMessages(threadId).subscribe({
      next: (res) => {
        this.loadingMessages.set(false);
        if (res.items.length > 0) {
          this.messages.set(res.items);
          return;
        }

        this.scheduleOpeningChallenge(threadId);
      },
      error: (err) => {
        this.loadingMessages.set(false);
        this.error.set(getApiErrorMessage(err, 'Failed to load messages for this thread.'));
      }
    });
  }

  private scheduleOpeningChallenge(threadId: string): void {
    const delayMs = this.loadOrder * 1500;
    timer(delayMs).subscribe(() => this.ensureOpeningChallenge(threadId));
  }

  private ensureOpeningChallenge(threadId: string): void {
    this.generatingOpening.set(true);
    this.error.set(null);

    this.threadsApi.ensureOpeningChallenge(threadId).subscribe({
      next: (res) => {
        this.messages.set([res.message]);
        this.generatingOpening.set(false);
      },
      error: (err) => {
        this.generatingOpening.set(false);
        this.error.set(getApiErrorMessage(err, 'Failed to generate opening challenge for this persona.'));
      }
    });
  }

  send(): void {
    const content = this.form.controls.content.value.trim();
    if (!content) return;

    this.sending.set(true);
    this.error.set(null);
    this.threadsApi.postMessage(this.threadInput()!.id, content).subscribe({
      next: (res) => {
        this.messages.update((items) => [...items, res.userMessage, res.assistantMessage]);
        this.form.reset();
        this.sending.set(false);
      },
      error: (err) => {
        this.error.set(getApiErrorMessage(err, 'Failed to send message. The AI reply may be unavailable.'));
        this.sending.set(false);
      }
    });
  }
}
