import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environments';
import { TokenService } from './Token.service';
import { UserCreate } from '../../features/user/models/user';
import { Router } from '@angular/router';

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API = `${environment.apiUrl}/auth`;
  private readonly api = `${environment.apiUrl}` + '/User';

  private router = inject(Router);
  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) { }

  login(UsernameOrEmail: string, Password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.API}/login`, { UsernameOrEmail, Password })
      .pipe(tap((res) => this.tokenService.save(res.accessToken, res.refreshToken)));
  }

  signup(Username: string, Email: string, Password: string, Firstname: string, Lastname: string) {
    return this.http.post<UserCreate>(`${this.api}/sign-up`, { Username, Email, Password, Firstname, Lastname });
  }

  refreshToken() {
    const refreshToken = localStorage.getItem('refresh_token');
    return this.http.post<TokenResponse>(`${this.API}/refresh`, { refreshToken });
  }

  logout(): void {
    this.tokenService.clear();
    this.router.navigate(['/login']);
  }

  loginWithGoogle(): void {
    window.location.href = `${this.API}/google`;
  }

  loginWithFacebook(): void {
    window.location.href = `${this.API}/facebook`;
  }

  isAuthenticated(): boolean {
    return !!this.tokenService.getAccessToken();
  }
}
