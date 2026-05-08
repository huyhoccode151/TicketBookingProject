import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, catchError, filter, Observable, switchMap, take, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

//export const authInterceptor: HttpInterceptorFn = (req, next) => {
//  const token = localStorage.getItem('access_token');

//  if (token) {
//    const cloned = req.clone({
//      setHeaders: {
//        Authorization: `Bearer ${token}`
//      }
//    });
//    return next(cloned);
//  }

//  return next(req);
//};

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshToken$ = new BehaviorSubject<string | null>(null);

  private auth = inject(AuthService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const publicUrls = ['/login', '/sign-up'];
    if (publicUrls.some(url => req.url.includes(url))) {
      return next.handle(req);
    }

    const token = localStorage.getItem('access_token');
    const authReq = token ? this.addToken(req, token) : req;

    if (!token) {
      this.auth.logout();
      return throwError(() => new Error('No token'));
    }

    return next.handle(authReq).pipe(
      catchError(err => {
        if ((err instanceof HttpErrorResponse && err.status === 401) || this.isUnauthorizedException(err)) {
          return this.handle401(req, next);
        }
        return throwError(() => err);
      }))
  }

  private handle401(req: HttpRequest<any>, next: HttpHandler) {
    if (this.isRefreshing) {
      return this.refreshToken$.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(token => next.handle(this.addToken(req, token!)))
      );
    }

    this.isRefreshing = true;
    this.refreshToken$.next(null);

    return this.auth.refreshToken().pipe(
      switchMap(({ accessToken, refreshToken }) => {
        this.isRefreshing = false;

        localStorage.setItem('access_token', accessToken);
        localStorage.setItem('refresh_token', refreshToken);

        this.refreshToken$.next(accessToken);
        return next.handle(this.addToken(req, accessToken));
      }),
      catchError(err => {
        this.isRefreshing = false;

        if (err.status === 401 || err.status === 403 || !err.status) {
          this.auth.logout();
        }

        return throwError(() => err);
      })
    )
  }

  private addToken(req: HttpRequest<any>, token: string) {
    return req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  private isUnauthorizedException(error: HttpErrorResponse): boolean {
    return error.status === 500 &&
      error.error?.includes?.('UnauthorizedAccessException');
  }
}
