import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
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
export class TextEditor implements OnChanges {
  @Input() editorContent: string = '';
  @Output() contentChange = new EventEmitter<string>();

  internalContent = '';

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

  ngOnChanges(changes: SimpleChanges) {
    if (changes['editorContent'] && changes['editorContent'].currentValue !== this.internalContent) {
      this.internalContent = changes['editorContent'].currentValue;
    }
  }

  onContentChange(content: string) {
    this.internalContent = content;
    this.contentChange.emit(content);
  }

  publish() {
    console.log('Blog Content:', this.internalContent);
    alert('Published! Check the console for output.');
  }
}
