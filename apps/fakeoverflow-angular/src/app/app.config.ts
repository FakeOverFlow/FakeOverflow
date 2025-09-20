import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import Aura from '@primeuix/themes/aura';
import { routes } from './app.routes';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import {provideHotToastConfig} from '@ngxpert/hot-toast';
import {providePrimeNG} from 'primeng/config';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
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
    provideHotToastConfig({
      position: 'top-right',
      duration: 3000,
      dismissible: true,
    })
  ]
};
