import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginRequest, RegisterRequest, LoginResponse } from '../models/user.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private tokenKey = 'auth_token';
  private userSubject = new BehaviorSubject<LoginResponse | null>(null);
  public user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadUserFromStorage();
  }

  private loadUserFromStorage(): void {
    const token = this.getToken();
    const username = localStorage.getItem('username');
    const email = localStorage.getItem('email');
    const expiresAt = localStorage.getItem('expiresAt');

    if (token && username && email && expiresAt) {
      const user: LoginResponse = {
        token,
        username,
        email,
        expiresAt: new Date(expiresAt)
      };
      this.userSubject.next(user);
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        localStorage.setItem(this.tokenKey, response.token);
        localStorage.setItem('username', response.username);
        localStorage.setItem('email', response.email);
        localStorage.setItem('expiresAt', response.expiresAt.toString());
        this.userSubject.next(response);
      })
    );
  }

  register(registerData: RegisterRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/register`, registerData).pipe(
      tap(response => {
        localStorage.setItem(this.tokenKey, response.token);
        localStorage.setItem('username', response.username);
        localStorage.setItem('email', response.email);
        localStorage.setItem('expiresAt', response.expiresAt.toString());
        this.userSubject.next(response);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem('username');
    localStorage.removeItem('email');
    localStorage.removeItem('expiresAt');
    this.userSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    const expiresAt = localStorage.getItem('expiresAt');
    
    if (!token || !expiresAt) {
      return false;
    }

    const expirationDate = new Date(expiresAt);
    return new Date() < expirationDate;
  }

  getCurrentUser(): LoginResponse | null {
    return this.userSubject.value;
  }
}

