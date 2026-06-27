import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginRequest,
  SignupRequest,
  TokenRefreshResponse,
  UserDto
} from '../models/auth.models';
import { TokenStorage } from './token-storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly tokenStorage = new TokenStorage();
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  readonly currentUser = signal<UserDto | null>(null);

  isAuthenticated(): boolean {
    return !!this.tokenStorage.getAccessToken();
  }

  getAccessToken(): string | null {
    return this.tokenStorage.getAccessToken();
  }

  getRefreshToken(): string | null {
    return this.tokenStorage.getRefreshToken();
  }

  signup(request: SignupRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/signup`, request).pipe(
      tap((res) => this.handleAuthSuccess(res))
    );
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request).pipe(
      tap((res) => this.handleAuthSuccess(res))
    );
  }

  refresh(): Observable<TokenRefreshResponse> {
    const refreshToken = this.tokenStorage.getRefreshToken();
    return this.http.post<TokenRefreshResponse>(`${this.baseUrl}/refresh`, { refreshToken }).pipe(
      tap((res) => this.tokenStorage.saveTokens(res.accessToken, res.refreshToken))
    );
  }

  loadMe(): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.baseUrl}/me`).pipe(
      tap((user) => this.currentUser.set(user))
    );
  }

  logout(): void {
    const refreshToken = this.tokenStorage.getRefreshToken();
    if (refreshToken) {
      this.http.post(`${this.baseUrl}/logout`, { refreshToken }).subscribe({ error: () => undefined });
    }
    this.tokenStorage.clear();
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  private handleAuthSuccess(response: AuthResponse): void {
    this.tokenStorage.saveTokens(response.accessToken, response.refreshToken);
    this.currentUser.set(response.user);
  }
}
