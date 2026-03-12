import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environments';
import { TokenService } from './Token.service';

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API = `${environment.apiUrl}/auth`;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) { }

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.API}/login`, { username, password })
      .pipe(tap((res) => this.tokenService.save(res.accessToken, res.refreshToken)));
  }

  logout(): void {
    this.tokenService.clear();
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
