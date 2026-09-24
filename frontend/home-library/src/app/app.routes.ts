import { Routes } from '@angular/router';
import { Edit } from './edit/edit';
import { App } from './app';
import { Start } from './start/start';

export const routes: Routes = [
    { path: '', component: App },
    { path: 'list', component: App },
    { path: 'edit', component: Edit },
    { path: 'edit/:id', component: Edit },
];
