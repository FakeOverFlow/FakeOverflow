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
      }
    ]
  },
];
