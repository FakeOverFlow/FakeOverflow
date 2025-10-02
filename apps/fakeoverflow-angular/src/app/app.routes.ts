import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./pages/auth/login/login').then(m => m.Login),
        title: 'Login | FakeOverflow'
      },
      {
        path: 'sign-up',
        loadComponent: () => import('./pages/auth/sign-up/sign-up').then(m => m.SignUp),
        title: 'Sign Up | FakeOverflow'
      },
      {
        path: 'verify',
        loadComponent: () => import('./pages/auth/verify/verify').then(m => m.Verify),
        title: 'Verify Account | FakeOverflow'
      },
      {
        path: 'verify/:verify-id',
        loadComponent: () => import('./pages/auth/verify/verify').then(m => m.Verify),
        title: 'Verifying your account | FakeOverflow'
      }
    ]
  },
  {
    path: 'home',
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/home/home/home').then(m => m.Home),
        title: 'Home | FakeOverflow'
      }
    ]
  },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  }
];
