import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BehaviorSubject, catchError, EMPTY, filter, switchMap, take, throwError } from 'rxjs';
import {environment} from '@environments/environment';
import {Authentication} from '@services/authentication';
import {AuthService} from 'fakeoverflow-angular-services';

let isRefreshing = false;
const refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

export const refreshTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const apiBaseUrl = [
    environment.apiBaseUrl,
  ];
  if (!apiBaseUrl.some(url => req.url.startsWith(url)) || !req.headers.has('Authorization')) {
    return next(req);
  }

  const authService = inject(Authentication);
  const authApiService = inject(AuthService);

  return next(req).pipe(
    catchError(err => {
      if(!(err instanceof HttpErrorResponse) || !req.headers.has("Authorization"))
        return throwError(() => err);

      if(err.status !== 401)
        return throwError(() =>  err);

      if(!authService.currentIdentityAsValue?.secrets?.refreshToken || req.url.endsWith("/auth/refresh/"))
      {
        authService.logout({
          redirectToLogout: true,
          toastMessage: 'Failed to refresh your session, please try login again',
        });
        return throwError(() =>  err);
      }


      if(isRefreshing){
        return refreshTokenSubject.pipe(
          filter(token => token !== null),
          take(1),
          switchMap(() => next(req.clone({
            setHeaders: {
              Authorization: `Bearer ${authService.currentIdentityAsValue?.secrets?.accessToken}`
            }
          })))
        )
      }

      isRefreshing = true;
      refreshTokenSubject.next(null);

      return authApiService.refresh().pipe(
        switchMap((res) => {
          if(!res)
          {
            authService.logout({
              redirectToLogout: true,
              toastMessage: 'Failed to refresh your session, please try login again',
            });
            return throwError(() =>  err);
          }
          const value = res.value;
          isRefreshing = false;
          refreshTokenSubject.next(value!.accessToken);
          authService.updateSecrets({
            accessToken: value!.accessToken!,
            refreshToken: value!.refreshToken!,
          });
          return next(req.clone({
            setHeaders: {
              Authorization: `Bearer ${value!.accessToken}`
            }
          }))
        }),
        catchError((err) => {
          console.error("Error refreshing token", err);
          isRefreshing = false;
          authService.logout({
            redirectToLogout: true,
            toastMessage: 'Failed to refresh your session, please try login again',
          });
          return EMPTY;
        })
      )
    })
  );
};
