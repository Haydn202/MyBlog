import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../services/account.service';
import { ToastService } from '../services/toast.service';
import { Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toast = inject(ToastService);
  const router = inject(Router);

  const allowedRoles = route.data?.["roles"] as string[] | undefined;

  const token = accountService.currentUser()?.token;
  if (!token) {
    toast.error('You must be logged in');
    router.navigate(['/login']);
    return false;
  }

  const payload = decodeJwt(token);
  console.log('payload', payload);
  if (!payload) {
    toast.error('Invalid token');
    router.navigate(['/login']);
    return false;
  }

  if (allowedRoles && !allowedRoles.includes(payload.role)) {
    toast.error('You do not have permission to access this page');
    router.navigate(['/']);
    return false;
  }

  return true;
};

function decodeJwt(token: string): any {
  try {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload));
  } catch {
    return null;
  }
}
