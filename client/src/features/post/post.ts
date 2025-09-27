import { Component, inject, OnInit, signal, AfterViewInit, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PostsService } from '../../core/services/posts.service';
import { PostDto } from '../../Types/PostCreate';
import { CommonModule } from '@angular/common';
import { TopicPill } from '../../shared/components/topic-pill/topic-pill';
import { TopicColorOptions } from '../../Types/TopicColor';
import { SyntaxHighlightService } from '../../core/services/syntax-highlight.service';

@Component({
  selector: 'app-post',
  imports: [CommonModule, TopicPill],
  templateUrl: './post.html',
  styleUrl: './post.css'
})
export class Post implements OnInit, AfterViewInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private postsService = inject(PostsService);
  private syntaxHighlightService = inject(SyntaxHighlightService);
  private elementRef = inject(ElementRef);

  post = signal<PostDto | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  ngOnInit() {
    const postId = this.route.snapshot.paramMap.get('id');
    if (postId) {
      this.loadPost(postId);
    } else {
      this.errorMessage.set('Post ID not found');
      this.isLoading.set(false);
    }
  }

  private loadPost(postId: string) {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.postsService.getPost(postId).subscribe({
      next: (post) => {
        this.post.set(post);
        this.isLoading.set(false);
        // Highlight syntax after content is loaded
        setTimeout(() => this.highlightSyntax(), 100);
      },
      error: (error) => {
        console.error('Error loading post:', error);
        this.errorMessage.set('Failed to load post. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  ngAfterViewInit() {
    // Highlight syntax when view is ready
    setTimeout(() => this.highlightSyntax(), 100);
  }

  private highlightSyntax() {
    if (this.post()) {
      this.syntaxHighlightService.highlightCode(this.elementRef.nativeElement);
    }
  }

  goBack() {
    this.router.navigate(['/posts']);
  }

  getTopicColorHex(topic: any): string {
    const colorOption = TopicColorOptions.find(c => c.value === topic.color);
    return colorOption?.hex || '#6b7280';
  }
}
