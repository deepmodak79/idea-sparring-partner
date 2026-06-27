import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="auth-card">
      <h2>Sign up</h2>
      <form [formGroup]="form" (ngSubmit)="submit()">
        <label>Display name<input formControlName="displayName" /></label>
        <label>Email<input type="email" formControlName="email" /></label>
        <label>Password<input type="password" formControlName="password" /></label>
        @if (error()) { <p class="error">{{ error() }}</p> }
        <button type="submit" [disabled]="form.invalid || loading()">Create account</button>
      </form>
      <p>Already have an account? <a routerLink="/login">Log in</a></p>
    </section>
  `,
  styles: [`
    .auth-card { max-width: 400px; margin: 2rem auto; padding: 1.5rem; background: #fff; border-radius: 8px; border: 1px solid #ddd; }
    label { display: block; margin-bottom: 1rem; }
    input { width: 100%; padding: 0.5rem; margin-top: 0.25rem; }
    button { width: 100%; padding: 0.6rem; background: #2563eb; color: #fff; border: none; border-radius: 6px; }
    .error { color: #b00020; }
  `]
})
export class SignupComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  loading = signal(false);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    displayName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  submit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.error.set(null);
    this.auth.signup(this.form.getRawValue()).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.error.set(err.error?.error ?? 'Signup failed.');
        this.loading.set(false);
      },
      complete: () => this.loading.set(false)
    });
  }
}
