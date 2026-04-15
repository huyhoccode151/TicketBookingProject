import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';

import { App } from './app/app';
import { APP_ROUTES } from './app/app.routes';
import { TOAST_CONFIG } from './app/shared/ui/toast/toast.service';
import { AuthInterceptor, authInterceptor } from './app/core/interceptors/authInterceptor';
import { inject, provideAppInitializer } from '@angular/core';
import { PermissionService } from './app/core/services/permission-service';

bootstrapApplication(App, {
  providers: [
    provideHttpClient(withInterceptors([
      authInterceptor
    ])),
    provideRouter(APP_ROUTES),
    provideAppInitializer(() => {
      inject(PermissionService).load();
    }),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    provideHttpClient(),
    {
      provide: TOAST_CONFIG,
      useValue: {
        defaultDuration: 4000,
        defaultPosition: 'top-right',
        maxToasts: 5,
      },
    },
  ]
});
