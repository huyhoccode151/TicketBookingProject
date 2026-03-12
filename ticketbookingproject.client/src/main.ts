import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';

import { App } from './app/app';
import { APP_ROUTES } from './app/app.routes';

bootstrapApplication(App, {
  providers: [
    provideRouter(APP_ROUTES),
  ]
});
