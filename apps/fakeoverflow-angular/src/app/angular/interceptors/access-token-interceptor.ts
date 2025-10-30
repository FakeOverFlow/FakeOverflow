import { HttpInterceptorFn } from '@angular/common/http';
import {environment} from '@environments/environment';
import {Authentication} from '@services/authentication';
import {inject} from '@angular/core';

const BLACKLISTED_API_URLS : string[] = [
  "/accounts/auth/login/",
]

export const accessTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const apiBaseUrl = [
    environment.apiBaseUrl,
  ];

  if(!apiBaseUrl.some(url => req.url.startsWith(url))){
    console.trace("Skipping appending token for API requests since the api is not defined")
    return next(req);
  }

  if(isBlacklisted(req.url)){
    console.trace("URL is blacklisted, Skipping appending token")
    return next(req);
  }

  const authenticationService: Authentication = inject(Authentication);
  if (!authenticationService.isAuthenticated) {
    console.trace("No authentication identity found, Skipping token")
    return next(req);
  }

  const currentUserValue = authenticationService.currentIdentityAsValue;
  if(!currentUserValue){
    console.trace("No user identity found, Skipping token")
    return next(req);
  }

  const accessToken = currentUserValue.secrets.accessToken;
  if(!accessToken){
    console.trace("No access token found, Skipping token")
    return next(req);
  }

  req = req.clone({
    setHeaders: {
      Authorization: `Bearer ${accessToken}`
    },
  })

  return next(req);
};

/**
 * Checks if the provided URL is in the blacklist.
 *
 * @param {string} url - The URL to check against the blacklist.
 * @return {boolean} Returns true if the URL is blacklisted, otherwise false.
 */
function isBlacklisted(url: string): boolean {
  const requestUrl = new URL(url);
  return BLACKLISTED_API_URLS.includes(requestUrl.pathname);
}
