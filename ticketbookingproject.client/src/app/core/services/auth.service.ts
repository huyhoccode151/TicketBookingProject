import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, from, Observable, switchMap, tap } from 'rxjs';

import { environment } from '../../../environments/environments';
import { TokenService } from './Token.service';
import { UserCreate } from '../../features/user/models/user';
import { Router } from '@angular/router';
import { ApiResponse } from '../../features/user/models/paged-result';
import { PermissionService } from './permission-service';

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
  private userSubject = new BehaviorSubject<any>(null);
  public user$ = this.userSubject.asObservable();

  private router = inject(Router);
  private permissionService = inject(PermissionService);
  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) { }

  login(UsernameOrEmail: string, Password: string): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${this.API}/login`, { UsernameOrEmail, Password })
      .pipe(tap((res) => this.tokenService.save(res.data.accessToken, res.data.refreshToken)));
  }

  signup(Username: string, Email: string, Password: string, Firstname: string, Lastname: string, ConfirmPassword: string) {
    return this.http.post<UserCreate>(`${this.api}/sign-up`, { Username, Email, Password, Firstname, Lastname, ConfirmPassword });
  }

  refreshToken() {
    const refreshToken = localStorage.getItem('refresh_token');
    return this.http.post<TokenResponse>(`${this.API}/refresh`, { refreshToken });
  }

  logout(): void {
    this.tokenService.clear();
    this.permissionService.clear();
    this.router.navigate(['/login']);
  }

  loginWithGoogle(idToken: string): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.API}/google`, { idToken })
      .pipe(tap((res) => this.tokenService.save(res.data.accessToken, res.data.refreshToken)));
  }

  loginWithFacebook(): void {
    window.location.href = `${this.API}/facebook`;
  }

  isAuthenticated(): boolean {
    return !!this.tokenService.getAccessToken();
  }

  //loginWithGoogle() {
  //  return from(this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID)).pipe(
  //    switchMap((socialUser: SocialUser) => {
  //      // Bước 3: Gửi idToken nhận được từ Google lên Backend của mình
  //      return this.verifyWithBackend(socialUser.idToken ?? "");
  //    }),
  //    tap(response => {
  //      // Bước 5: Lưu JWT của hệ thống mình vào LocalStorage
  //      localStorage.setItem('access_token', response.data.accessToken);
  //      //this.userSubject.next(response.data.user);
  //    })
  //  );
  //}

  //private verifyWithBackend(idToken: string): Observable<ApiResponse<LoginResponse>> {
  //  return this.http.post<ApiResponse<LoginResponse>>(`${this.API}/google`, { idToken });
  //}
}
