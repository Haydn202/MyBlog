import { Routes } from '@angular/router';
import {Home} from '../features/home/home';
import {Post} from '../features/post/post';
import {AdminPage} from '../features/admin/admin-page/admin-page';
import {About} from '../features/about/about';
import {authGuard} from '../core/guards/auth-guard';

export const routes: Routes = [
  {path: 'posts', component: Home},
  {path: 'post/:id', component: Post},
  {path: 'admin', component: AdminPage, canActivate: [authGuard], data: { roles: ['Admin'] }},
  {path: 'about', component: About},
  {path: '**', component: Home}
];
