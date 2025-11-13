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
  { path: 'home', loadComponent: () => import('./pages/home/home/home').then(m => m.Home), },
  { path: 'post', loadComponent: () => import('./pages/home/home/CreatePost/post').then(m => m.PostComponent) },
  { path: 'update', loadComponent: () => import('./pages/home/home/UpdatePost/update').then(m => m.UpdateComponent) },
   { path: 'detail', loadComponent: () => import('./pages/home/home/PostDetails/details').then(m => m.DetailsComponent) },
    { path: 'my-questions', loadComponent: () => import('./pages/home/home/MyQuestions/myQuestions').then(m => m.MyQuestions) },

  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home' }

];
