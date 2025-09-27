import { Component, inject, OnInit, signal, Output, EventEmitter } from '@angular/core';
import { PostsService } from '../../../core/services/posts.service';
import { PostSummaryDto } from '../../../Types/PostSummary';
import { PostFilters } from '../../../Types/PostFilters';
import { TopicsService } from '../../../core/services/topics.service';
import { TopicDto } from '../../../Types/TopicManagement';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TopicPill } from '../../../shared/components/topic-pill/topic-pill';
import { TopicColorOptions } from '../../../Types/TopicColor';

@Component({
  selector: 'app-posts-list',
  imports: [
    CommonModule,
    FormsModule,
    TopicPill
  ],
  templateUrl: './posts-list.html',
  styleUrl: './posts-list.css'
})
export class PostsList implements OnInit {
  private postsService = inject(PostsService);
  private topicsService = inject(TopicsService);

  posts = this.postsService.posts;
  topics = this.topicsService.topics;

  // Filter properties
  currentFilters = signal<PostFilters>({});
  statusFilter = signal<string>('');
  topicFilter = signal<string>('');
  searchFilter = signal<string>('');

  // Output events
  @Output() postSelected = new EventEmitter<PostSummaryDto>();
  @Output() createNewPost = new EventEmitter<void>();

  ngOnInit() {
    this.loadPosts();
    this.loadTopics();
  }

  private loadPosts() {
    this.postsService.getPosts(this.currentFilters()).subscribe();
  }

  private loadTopics() {
    this.topicsService.getTopics().subscribe();
  }

  selectPost(post: PostSummaryDto) {
    this.postSelected.emit(post);
  }

  createNew() {
    this.createNewPost.emit();
  }

  applyFilters() {
    const filters: PostFilters = {};

    if (this.statusFilter()) {
      filters.status = this.statusFilter() as 'Draft' | 'Published' | 'Deleted';
    }

    if (this.topicFilter()) {
      filters.topicId = this.topicFilter();
    }

    if (this.searchFilter()) {
      filters.searchTerm = this.searchFilter();
    }

    this.currentFilters.set(filters);
    this.loadPosts();
  }

  clearFilters() {
    this.statusFilter.set('');
    this.topicFilter.set('');
    this.searchFilter.set('');
    this.currentFilters.set({});
    this.loadPosts();
  }

  onStatusFilterChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.statusFilter.set(target.value);
    this.applyFilters();
  }

  onTopicFilterChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.topicFilter.set(target.value);
    this.applyFilters();
  }

  onSearchFilterChange(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchFilter.set(target.value);
  }

  getTopicColorHex(topic: TopicDto): string {
    const colorOption = TopicColorOptions.find(c => c.value === topic.color);
    return colorOption?.hex || '#6b7280';
  }
}
