import { Routes } from '@angular/router';
import {Home} from '../features/home/home';
import {Post} from '../features/post/post';
import {AdminPage} from '../features/admin/admin-page/admin-page';
import {CreatePost} from '../features/admin/create-post/create-post';
import {ManageTopics} from '../features/admin/manage-topics/manage-topics';
import {TopicForm} from '../features/admin/topic-form/topic-form';
import {About} from '../features/about/about';
import {authGuard} from '../core/guards/auth-guard';

export const routes: Routes = [
  {path: 'posts', component: Home},
  {path: 'post/:id', component: Post},
  {path: 'admin', component: AdminPage, canActivate: [authGuard], data: { roles: ['Admin'] }},
  {path: 'admin/create-post', component: CreatePost, canActivate: [authGuard], data: { roles: ['Admin'] }},
  {path: 'admin/manage-topics', component: ManageTopics, canActivate: [authGuard], data: { roles: ['Admin'] }},
  {path: 'admin/create-topic', component: TopicForm, canActivate: [authGuard], data: { roles: ['Admin'] }},
  {path: 'admin/edit-topic/:id', component: TopicForm, canActivate: [authGuard], data: { roles: ['Admin'] }},
  {path: 'about', component: About},
  {path: '**', component: Home}
];
