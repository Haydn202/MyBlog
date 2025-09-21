import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PostsService } from '../../../core/services/posts.service';
import { PostSummaryDto } from '../../../Types/PostSummary';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manage-posts',
  imports: [
    CommonModule
  ],
  templateUrl: './manage-posts.html',
  styleUrl: './manage-posts.css'
})
export class ManagePosts implements OnInit {
  private postsService = inject(PostsService);
  private router = inject(Router);

  posts = this.postsService.posts;

  ngOnInit() {
    this.postsService.getPosts().subscribe();
  }

  createNewPost() {
    this.router.navigate(['/admin/create-post']);
  }

  editPost(postId: string) {
    // TODO: Implement edit functionality
    console.log('Edit post:', postId);
  }

  deletePost(postId: string) {
    // TODO: Implement delete functionality
    console.log('Delete post:', postId);
  }
}
