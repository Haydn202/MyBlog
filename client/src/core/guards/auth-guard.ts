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

  if (!accountService.currentUser()) {
    toast.error('You must be logged in');
    router.navigate(['/login']);
    return false;
  }

  if (allowedRoles && !accountService.hasRole(...allowedRoles)) {
    toast.error('You do not have permission to access this page');
    router.navigate(['/']);
    return false;
  }

  return true;
};
