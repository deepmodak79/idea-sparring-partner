import { Component, inject, Input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ThreadService } from '../../core/services/thread.service';
import { Message, ThreadItem } from '../../core/models/thread.models';
import { getApiErrorMessage } from '../../core/utils/api-error.util';

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
        @for (message of messages(); track message.id) {
          <div class="message" [class.user]="message.role === 'User'" [class.assistant]="message.role === 'Assistant'">
            <strong>{{ message.role }}</strong>
            <p>{{ message.content }}</p>
          </div>
        }
      </div>
      <form [formGroup]="form" (ngSubmit)="send()">
        <textarea rows="3" formControlName="content" placeholder="Reply to {{ threadInput()?.persona }}..."></textarea>
        @if (error()) { <p class="error">{{ error() }}</p> }
        <button type="submit" [disabled]="form.invalid || sending()">Send</button>
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

  private readonly threadsApi = inject(ThreadService);
  private readonly fb = inject(FormBuilder);

  threadInput = signal<ThreadItem | null>(null);
  messages = signal<Message[]>([]);
  sending = signal(false);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({ content: [''] });

  loadMessages(threadId: string): void {
    this.threadsApi.getMessages(threadId).subscribe({
      next: (res) => this.messages.set(res.items),
      error: (err) => this.error.set(getApiErrorMessage(err, 'Failed to load messages for this thread.'))
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
