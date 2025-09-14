import {Component, inject} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {PostsService} from '../../core/services/posts.service';
import {SummaryCard} from '../../layout/summary-card/summary-card';
import {TextEditor} from '../text-editor/text-editor';

@Component({
  selector: 'app-home',
  imports: [
    SummaryCard,
    TextEditor
  ],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  protected postService = inject(PostsService);

  async ngOnInit(){
    await this.postService.getPosts().subscribe();
  }

}
