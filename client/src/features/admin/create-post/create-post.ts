import { Component } from '@angular/core';
import {TextEditor} from '../../text-editor/text-editor';

@Component({
  selector: 'app-create-post',
  imports: [
    TextEditor
  ],
  templateUrl: './create-post.html',
  styleUrl: './create-post.css'
})
export class CreatePost {

}
