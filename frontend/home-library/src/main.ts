import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { Start } from './app/start/start';

bootstrapApplication(Start, appConfig)
  .catch((err) => console.error(err));
