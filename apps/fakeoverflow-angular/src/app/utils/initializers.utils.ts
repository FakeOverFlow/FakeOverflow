import {environment} from '@environments/environment';
import {inject} from '@angular/core';
import {Authentication} from '@services/authentication';

export function overrideLoggers(){
  const isLogIgnored = environment.ignoreLogs;
  if(isLogIgnored){
    console.trace = () => {};
    console.info = () => {};
  }
}

export function initializeAuthentications(){
  const authenticationService = inject(Authentication);
  authenticationService.initAuthentication();
}
