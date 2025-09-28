import {
  ApplicationConfig,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';
import {provideRouter, withHashLocation} from '@angular/router';
import Aura from '@primeuix/themes/aura';
import { routes } from './app.routes';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import {provideHotToastConfig} from '@ngxpert/hot-toast';
import {providePrimeNG} from 'primeng/config';
import {initializeAuthentications, overrideLoggers} from '@utils/initializers.utils';
import {environment} from '@environments/environment';
import { provideApi } from "fakeoverflow-angular-services";

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    environment.useHashBasedRouting ?
      provideRouter(routes, withHashLocation()) :
      provideRouter(routes),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        options: {
          darkModeSelector: '.fof-dark-mode-toggle',
          cssLayer: {
            name: 'primeng',
            order: 'theme, base, primeng'
          }
        },
        preset: Aura
      }
    }),
    provideHttpClient(
      withInterceptors([

      ])
    ),
    provideApi(environment.apiBaseUrl),
    provideHotToastConfig({
      position: 'top-right',
      duration: 3000,
      dismissible: true,
    }),
    provideAppInitializer(overrideLoggers),
    provideAppInitializer(initializeAuthentications)
  ]
};
