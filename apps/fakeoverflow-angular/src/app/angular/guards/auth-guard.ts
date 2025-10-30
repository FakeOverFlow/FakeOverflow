import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Authentication } from '@services/authentication';

export const authGuard: CanActivateFn = (route, state) => {
  const authenticationService = inject(Authentication);
  const router = inject(Router);

  const isAuthenticated = authenticationService.isAuthenticated;
  console.trace('Auth Guard - isAuthenticated:', isAuthenticated);
  if(!isAuthenticated) {
    const isPublic = state.url.startsWith('/public') || state.url.startsWith('/auth');
    if(!isPublic) {
      return router.parseUrl('/auth/login');
    }
  }

  return true;
};
