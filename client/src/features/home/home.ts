import {Component, inject, OnInit} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {PostsService} from '../../core/services/posts.service';
import {SummaryCard} from '../../layout/summary-card/summary-card';
import {TextEditor} from '../text-editor/text-editor';
import {PostSummaryDto} from '../../Types/PostSummary';

@Component({
  selector: 'app-home',
  imports: [
    SummaryCard
  ],
  templateUrl: './home.html',
  styleUrl: './home.css',
  standalone: true
})
export class Home implements OnInit {
  constructor(public postService: PostsService) {}

  ngOnInit() {
    this.postService.getPublicPosts().subscribe();
  }

  trackById(index: number, post: PostSummaryDto) {
    return post.id;
  }
}
