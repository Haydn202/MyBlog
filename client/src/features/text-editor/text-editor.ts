import { Component, EventEmitter, Output } from '@angular/core';
import {QuillEditorComponent} from 'ngx-quill';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-text-editor',
  templateUrl: './text-editor.html',
  imports: [
    QuillEditorComponent,
    FormsModule
  ],
  styleUrls: ['./text-editor.css']
})
export class TextEditor {
  @Output() contentChange = new EventEmitter<string>();

  editorContent = '';

  modules = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'],
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      [{ 'list': 'ordered' }, { 'list': 'bullet' }],
      ['blockquote', 'code-block'],
      ['link', 'image', 'video'],
      [{ 'align': [] }],
      [{ 'color': [] }, { 'background': [] }],
      ['clean'],
    ]
  };

  onContentChange(content: string) {
    this.editorContent = content;
    this.contentChange.emit(content);
  }

  publish() {
    console.log('Blog Content:', this.editorContent);
    alert('Published! Check the console for output.');
  }
}
