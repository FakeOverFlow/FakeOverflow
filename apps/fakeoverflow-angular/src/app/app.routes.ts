import { Routes } from '@angular/router';
import { authGuard } from '@guards/auth-guard';
import { DetailsComponent } from './pages/home/home/PostDetails/details';
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
  { 
    path: 'post',
    children: [
      {
        path: 'create',
        loadComponent: () => import('./pages/home/home/create-post/post').then(m => m.PostComponent),
        canActivate: [authGuard],
        title: 'Create Post | FakeOverflow'
      }
    ]     
  },
  { path: 'update', loadComponent: () => import('./pages/home/home/UpdatePost/update').then(m => m.UpdateComponent) },
   { path: 'detail', loadComponent: () => import('./pages/home/home/PostDetails/details').then(m => m.DetailsComponent) },
    { path: 'personalhome', loadComponent: () => import('./pages/home/home/PersonalQuestionsHome/personalHome').then(m => m.PersonalHome) },

  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: '**', redirectTo: 'home' },
{ path: 'post/:id', component: DetailsComponent }
];
