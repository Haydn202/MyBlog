import {Component, inject, OnInit} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {PostsService} from '../../core/services/posts.service';
import {SummaryCard} from '../../layout/summary-card/summary-card';
import {TextEditor} from '../text-editor/text-editor';
import {RouterLink} from '@angular/router';
import {PostSummaryDto} from '../../Types/PostSummary';
import {NgForOf} from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [
    SummaryCard,
    RouterLink,
    NgForOf
  ],
  templateUrl: './home.html',
  styleUrl: './home.css',
  standalone: true
})
export class Home implements OnInit {
  constructor(public postService: PostsService) {}

  ngOnInit() {
    this.postService.getPosts().subscribe();
  }

  trackById(index: number, post: PostSummaryDto) {
    return post.id;
  }
}
