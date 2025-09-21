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

  // Computed property for role
  role = computed(() => {
    const user = this.currentUser();
    if (!user?.token) return null;

    const payload = this.decodeJwt(user.token);
    return payload?.role || null;
  });

  // Computed for multiple roles (if JWT has array)
  roles = computed(() => {
    const user = this.currentUser();
    if (!user?.token) return [];
    const payload = this.decodeJwt(user.token);
    if (!payload) return [];
    return Array.isArray(payload.roles) ? payload.roles : [payload.role];
  });

  login(creds: LoginCreds) {
    return this.http.post<User>(`${this.baseUrl}/accounts/login`, creds).pipe(
      tap(user => {
        if (user) this.setCurrentUser(user);
      })
    );
  }

  signup(creds: RegisterCreds) {
    return this.http.post<User>(`${this.baseUrl}/accounts/register`, creds).pipe(
      tap(user => {
        if (user) this.setCurrentUser(user);
      })
    );
  }

  private setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  /** Check if current user has at least one of the allowed roles */
  hasRole(...allowedRoles: string[]) {
    return this.roles().some((r: string) => allowedRoles.includes(r));
  }

  /** Decode JWT payload */
  private decodeJwt(token: string): any {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch {
      return null;
    }
  }
}
