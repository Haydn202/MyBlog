import { Component } from '@angular/core';
import {CreatePost} from '../create-post/create-post';

@Component({
  selector: 'app-manage-posts',
  imports: [
    CreatePost
  ],
  templateUrl: './manage-posts.html',
  styleUrl: './manage-posts.css'
})
export class ManagePosts {

}
