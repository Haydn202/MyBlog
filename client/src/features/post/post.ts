import { Component, inject, OnInit, signal, ElementRef, AfterViewInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PostsService } from '../../core/services/posts.service';
import { PostDto } from '../../Types/PostCreate';
import { CommonModule } from '@angular/common';
import { TopicPill } from '../../shared/components/topic-pill/topic-pill';
import { TopicColorOptions } from '../../Types/TopicColor';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import Prism from 'prismjs';
import 'prismjs/components/prism-javascript';
import 'prismjs/components/prism-json';
import 'prismjs/themes/prism-tomorrow.css';

@Component({
  selector: 'app-post',
  imports: [CommonModule, TopicPill],
  templateUrl: './post.html',
  styleUrls: ['./post.css'] // note: fixed typo from styleUrl -> styleUrls
})
export class Post implements OnInit, AfterViewInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private postsService = inject(PostsService);
  private sanitizer = inject(DomSanitizer);

  post = signal<PostDto | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  postContentSafe: SafeHtml | null = null;

  private highlighted = false;

  @ViewChild('postContent') postContentRef!: ElementRef;

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
        // Render safe HTML
        this.postContentSafe = this.sanitizer.bypassSecurityTrustHtml(post.content);
        this.isLoading.set(false);
        this.highlighted = false; // reset highlighting flag
        
        // Highlight syntax after content is loaded
        setTimeout(() => this.highlightCode(), 100);
      },
      error: (error) => {
        console.error('Error loading post:', error);
        this.errorMessage.set('Failed to load post. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  ngAfterViewInit() {
    setTimeout(() => this.highlightCode(), 200);
  }

  private highlightCode() {
    if (!this.postContentRef) {
      console.log('postContentRef not available yet');
      return;
    }
    
    const element = this.postContentRef.nativeElement as HTMLElement;
    console.log('Starting syntax highlighting...');
    console.log('Element:', element);
    
    const codeBlocks = element.querySelectorAll('pre[data-language]');
    console.log('Found code blocks:', codeBlocks.length);

    codeBlocks.forEach((preElement, index) => {
      const htmlElement = preElement as HTMLElement;
      const language = htmlElement.getAttribute('data-language');
      
      console.log(`Code block ${index}:`, {
        language: language,
        textContent: htmlElement.textContent?.substring(0, 100)
      });
      
      if (language && language !== 'plain') {
        htmlElement.classList.add(`language-${language}`);
        
        Prism.highlightElement(htmlElement);
        console.log(`Highlighted as ${language}`);
      }
    });
    
    const codeElements = element.querySelectorAll('pre code, code');
    console.log('Found code elements:', codeElements.length);
    
    codeElements.forEach((codeElement, index) => {
      const htmlElement = codeElement as HTMLElement;
      const parentPre = htmlElement.closest('pre');
      const language = parentPre?.getAttribute('data-language') || 'javascript';
      
      console.log(`Code element ${index}:`, {
        language: language,
        textContent: htmlElement.textContent?.substring(0, 100)
      });
      
      if (language && language !== 'plain') {
        htmlElement.classList.add(`language-${language}`);
        Prism.highlightElement(htmlElement);
        console.log(`Highlighted code element as ${language}`);
      }
    });
  }

  goBack() {
    this.router.navigate(['/posts']);
  }

  getTopicColorHex(topic: any): string {
    const colorOption = TopicColorOptions.find(c => c.value === topic.color);
    return colorOption?.hex || '#6b7280';
  }
}
