import { Component } from '@angular/core';
import {ManagePosts} from '../manage-posts/manage-posts';

@Component({
  selector: 'app-admin-page',
  imports: [
    ManagePosts
  ],
  templateUrl: './admin-page.html',
  styleUrl: './admin-page.css'
})
export class AdminPage {

}
