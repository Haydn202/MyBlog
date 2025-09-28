import {Component, Input, signal, inject} from '@angular/core';
import {PostSummaryDto} from '../../Types/PostSummary';
import {Thumbnail} from '../../shared/components/thumbnail/thumbnail';
import {PostsService} from '../../core/services/posts.service';
import {PostDto} from '../../Types/PostCreate';
import {CommonModule} from '@angular/common';
import {DomSanitizer, SafeHtml} from '@angular/platform-browser';
import {TopicPill} from '../../shared/components/topic-pill/topic-pill';
import Prism from 'prismjs';
import 'prismjs/components/prism-javascript';
import 'prismjs/components/prism-typescript';
import 'prismjs/components/prism-csharp';
import 'prismjs/components/prism-go';
import 'prismjs/components/prism-python';

import 'prismjs/components/prism-json';
import 'prismjs/themes/prism-tomorrow.css';

@Component({
  selector: 'app-summary-card',
  imports: [Thumbnail, CommonModule, TopicPill],
  templateUrl: './summary-card.html',
  styleUrl: './summary-card.css'
})
export class SummaryCard {
  @Input({required: true}) post: PostSummaryDto | undefined;
  
  private postsService = inject(PostsService);
  private sanitizer = inject(DomSanitizer);
  
  isExpanded = signal(false);
  isLoading = signal(false);
  fullPost = signal<PostDto | null>(null);
  postContentSafe: SafeHtml | null = null;
  
  toggleExpansion() {
    if (this.isExpanded()) {
      this.isExpanded.set(false);
    } else {
      this.isExpanded.set(true);
      this.loadFullPost();
    }
  }
  
  private loadFullPost() {
    if (!this.post?.id) return;
    
    this.isLoading.set(true);
    this.postsService.getPost(this.post.id).subscribe({
      next: (post) => {
        this.fullPost.set(post);
        this.postContentSafe = this.sanitizer.bypassSecurityTrustHtml(post.content);
        this.isLoading.set(false);
        
        // Highlight syntax after content is loaded
        setTimeout(() => this.highlightCode(), 100);
      },
      error: (error) => {
        console.error('Error loading post:', error);
        this.isLoading.set(false);
      }
    });
  }
  
  private highlightCode() {
    // Look for code blocks and highlight them
    const codeBlocks = document.querySelectorAll('pre[data-language]');
    codeBlocks.forEach((preElement) => {
      const htmlElement = preElement as HTMLElement;
      const language = htmlElement.getAttribute('data-language');
      
      if (language && language !== 'plain') {
        htmlElement.classList.add(`language-${language}`);
        Prism.highlightElement(htmlElement);
      }
    });
  }
}
