import { Component, inject, OnInit, signal } from '@angular/core';
import { PostsService } from '../../../core/services/posts.service';
import { PostSummaryDto } from '../../../Types/PostSummary';
import { PostDto } from '../../../Types/PostCreate';
import { PostsList } from '../posts-list/posts-list';
import { PostEditor } from '../post-editor/post-editor';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manage-posts',
  imports: [
    CommonModule,
    PostsList,
    PostEditor
  ],
  templateUrl: './manage-posts.html',
  styleUrl: './manage-posts.css'
})
export class ManagePosts implements OnInit {
  private postsService = inject(PostsService);

  posts = this.postsService.posts;
  selectedPost = signal<PostDto | null>(null);
  isCreatingNew = signal(false);

  ngOnInit() {
    this.loadPosts();
  }

  private loadPosts() {
    this.postsService.getPosts({}).subscribe();
  }

  onPostSelected(post: PostSummaryDto) {
    this.isCreatingNew.set(false);
    this.postsService.getPost(post.id).subscribe({
      next: (fullPost) => {
        this.selectedPost.set(fullPost);
      },
      error: (error) => {
        console.error('Error loading post:', error);
      }
    });
  }

  onCreateNewPost() {
    this.isCreatingNew.set(true);
    this.selectedPost.set(null);
  }

  onPostSaved() {
    this.loadPosts(); // Refresh the list
  }

  onPostDeleted() {
    this.selectedPost.set(null);
    this.isCreatingNew.set(false);
    this.loadPosts(); // Refresh the list
  }

  onEditCancelled() {
    this.selectedPost.set(null);
    this.isCreatingNew.set(false);
  }
}