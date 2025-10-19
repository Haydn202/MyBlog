import { inject, Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginCreds, RegisterCreds, User } from '../../Types/user';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);
  baseUrl = 'http://localhost:5285';

  role = computed(() => {
    const user = this.currentUser();
    if (!user?.token) return null;

    const payload = this.decodeJwt(user.token);
    return payload?.role || null;
  });

  roles = computed(() => {
    const user = this.currentUser();
    if (!user?.token) return [];
    const payload = this.decodeJwt(user.token);
    if (!payload) return [];
    return Array.isArray(payload.roles) ? payload.roles : [payload.role];
  });

  login(creds: LoginCreds) {
    return this.http.post<User>(`${this.baseUrl}/accounts/login`, creds, { withCredentials: true }).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
          this.startRefreshTokenTimer();
        }
      })
    );
  }

  signup(creds: RegisterCreds) {
    return this.http.post<User>(`${this.baseUrl}/accounts/register`, creds, { withCredentials: true }).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
          this.startRefreshTokenTimer();
        }
      })
    );
  }

  refreshToken() {
    return this.http.post<User>(`${this.baseUrl}/accounts/refresh-token`, {}, { withCredentials: true }).pipe(
      tap(user => {
        if (user) this.setCurrentUser(user);
      })
    );
  }

  startRefreshTokenTimer() {
    setInterval(() => {
      this.refreshToken().subscribe({
        next: (user) => {
          if (user) this.setCurrentUser(user);
        },
        error: (error) => {
          this.logout().subscribe();
          console.error('Error refreshing token', error);
        },
      });
    }, 5 * 60 * 1000);
  }

  setCurrentUser(user: User) {
    this.currentUser.set(user);
  }

  logout() {
    return this.http.post(`${this.baseUrl}/accounts/logout`, {}, { withCredentials: true }).pipe(
      tap(() => {
        this.currentUser.set(null);
      })
    );
  }

  hasRole(...allowedRoles: string[]) {
    return this.roles().some((r: string) => allowedRoles.includes(r));
  }

  private decodeJwt(token: string): any {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch {
      return null;
    }
  }
}
