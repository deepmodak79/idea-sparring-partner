import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div class="app-shell">
      <header>
        <h1>Idea Sparring Partner</h1>
      </header>
      <main>
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .app-shell { max-width: 960px; margin: 0 auto; padding: 1.5rem; font-family: system-ui, sans-serif; }
    header h1 { margin: 0 0 0.25rem; }
    header p { margin: 0; color: #555; }
  `]
})
export class AppComponent {}
