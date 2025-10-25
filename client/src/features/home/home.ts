import {Component, inject, OnInit, signal, HostListener, OnDestroy} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {PostsService} from '../../core/services/posts.service';
import {SummaryCard} from '../../layout/summary-card/summary-card';
import {TextEditor} from '../text-editor/text-editor';
import {PostSummaryDto} from '../../Types/PostSummary';
import {PagingParams} from '../../Types/PagingParams';
import {PostFilters} from '../../Types/PostFilters';
import {CommonModule} from '@angular/common';
import {PostSkeleton} from '../../shared/components/post-skeleton/post-skeleton';

@Component({
  selector: 'app-home',
  imports: [
    SummaryCard,
    CommonModule,
    PostSkeleton
  ],
  templateUrl: './home.html',
  styleUrl: './home.css',
  standalone: true
})
export class Home implements OnInit, OnDestroy {
  public postService = inject(PostsService);
  paginationMetadata = this.postService.paginationMetadata;

  // Pagination properties
  currentPage = signal<number>(1);
  pageSize = signal<number>(10);
  
  // Infinite scroll properties
  isLoading = signal<boolean>(false);
  isLoadingMore = signal<boolean>(false);

  ngOnInit() {
    this.loadPosts();
  }

  ngOnDestroy() {
    // Reset posts when leaving the page
    this.postService.posts.set([]);
  }

  @HostListener('window:scroll')
  onScroll() {
    // Check if user is near the bottom of the page
    const scrollPosition = window.innerHeight + window.scrollY;
    const pageHeight = document.documentElement.scrollHeight;
    const threshold = 300; // pixels from bottom to trigger load

    if (scrollPosition >= pageHeight - threshold && !this.isLoadingMore() && this.hasMorePosts()) {
      this.loadMorePosts();
    }
  }

  private loadPosts(filters?: PostFilters) {
    this.isLoading.set(true);
    const pagingParams: PagingParams = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize()
    };
    this.postService.getPublicPosts(filters, pagingParams, false).subscribe({
      next: () => {
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  private loadMorePosts() {
    if (this.isLoadingMore()) return;

    this.isLoadingMore.set(true);
    this.currentPage.set(this.currentPage() + 1);

    const pagingParams: PagingParams = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize()
    };

    this.postService.getPublicPosts(undefined, pagingParams, true).subscribe({
      next: () => {
        this.isLoadingMore.set(false);
      },
      error: () => {
        this.isLoadingMore.set(false);
        // Revert page number on error
        this.currentPage.set(this.currentPage() - 1);
      }
    });
  }

  hasMorePosts(): boolean {
    const metadata = this.paginationMetadata();
    if (!metadata) return false;

    const totalLoaded = this.postService.posts().length;
    return totalLoaded < metadata.totalCount;
  }

  trackById(index: number, post: PostSummaryDto) {
    return post.id;
  }
}
