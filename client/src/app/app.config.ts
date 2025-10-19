import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { routes } from './app.routes';
import { AuthInterceptor } from '../core/interceptors/auth.interceptor';
import { InitService } from '../core/services/init.service';
import { lastValueFrom } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    provideAppInitializer(async () => {
      const initService = inject(InitService);
      setTimeout(async () =>
      {
        try {
          await lastValueFrom(initService.init());
        } catch (error) {
          console.error('Error initializing app', error);
        }
      }, 500);
    })
  ]
};
