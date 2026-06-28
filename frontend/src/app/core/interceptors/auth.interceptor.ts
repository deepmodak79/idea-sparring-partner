import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, finalize, shareReplay, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { TokenRefreshResponse } from '../models/auth.models';
import { Observable } from 'rxjs';

let refreshInFlight: Observable<TokenRefreshResponse> | null = null;

function refreshOnce(auth: AuthService): Observable<TokenRefreshResponse> {
  if (!refreshInFlight) {
    refreshInFlight = auth.refresh().pipe(
      finalize(() => {
        refreshInFlight = null;
      }),
      shareReplay(1)
    );
  }

  return refreshInFlight;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getAccessToken();

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error) => {
      if (error.status !== 401 || req.url.includes('/auth/refresh') || req.url.includes('/auth/login')) {
        return throwError(() => error);
      }

      return refreshOnce(auth).pipe(
        switchMap((res) => {
          const retryReq = req.clone({
            setHeaders: { Authorization: `Bearer ${res.accessToken}` }
          });
          return next(retryReq);
        }),
        catchError((refreshError) => {
          auth.logout();
          return throwError(() => refreshError);
        })
      );
    })
  );
};
