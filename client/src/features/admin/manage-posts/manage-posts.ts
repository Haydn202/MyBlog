import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { PostsService } from '../../../core/services/posts.service';
import { PostSummaryDto } from '../../../Types/PostSummary';
import { PostFilters } from '../../../Types/PostFilters';
import { TopicsService } from '../../../core/services/topics.service';
import { TopicDto } from '../../../Types/TopicManagement';
import { PostDto, PostUpdateDto } from '../../../Types/PostCreate';
import { TextEditor } from '../../text-editor/text-editor';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastService } from '../../../core/services/toast.service';
import { TopicPill } from '../../../shared/components/topic-pill/topic-pill';
import { TopicColorOptions } from '../../../Types/TopicColor';

@Component({
  selector: 'app-manage-posts',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TextEditor,
    TopicPill
  ],
  templateUrl: './manage-posts.html',
  styleUrl: './manage-posts.css'
})
export class ManagePosts implements OnInit {
  private postsService = inject(PostsService);
  private topicsService = inject(TopicsService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);

  posts = this.postsService.posts;
  topics = this.topicsService.topics;

  // Filter properties
  currentFilters = signal<PostFilters>({});
  statusFilter = signal<string>('');
  topicFilter = signal<string>('');
  searchFilter = signal<string>('');

  // Selected post and form
  selectedPost = signal<PostDto | null>(null);
  postForm!: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  editorContent = signal<string>('');
  isCreatingNew = signal(false);
  isPreviewMode = signal(false);

  ngOnInit() {
    this.initializeForm();
    this.loadPosts();
    this.loadTopics();
  }

  private initializeForm() {
    this.postForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(1000)]],
      topicIds: [[], Validators.required]
    });
  }

  private loadPosts() {
    this.postsService.getPosts(this.currentFilters()).subscribe();
  }

  private loadTopics() {
    this.topicsService.getTopics().subscribe();
  }

  createNewPost() {
    this.isCreatingNew.set(true);
    this.selectedPost.set(null);
    this.editorContent.set('');
    this.postForm.reset({
      title: '',
      description: '',
      topicIds: []
    });
    this.errorMessage.set(null);
  }

  selectPost(post: PostSummaryDto) {
    this.isCreatingNew.set(false);
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.postsService.getPost(post.id).subscribe({
      next: (fullPost) => {
        this.selectedPost.set(fullPost);
        this.populateForm(fullPost);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading post:', error);
        this.errorMessage.set('Failed to load post details.');
        this.isLoading.set(false);
      }
    });
  }

  private populateForm(post: PostDto) {
    this.postForm.patchValue({
      title: post.title,
      description: post.description,
      topicIds: post.topics.map(t => t.id)
    });
    this.editorContent.set(post.content);
  }

  saveAsDraft() {
    this.savePost('Draft');
  }

  publishPost() {
    this.savePost('Published');
  }

  private savePost(status: 'Draft' | 'Published') {
    if (this.postForm.valid && this.editorContent()) {
      this.isLoading.set(true);
      this.errorMessage.set(null);

      const formData = this.postForm.value;
      const postData = { ...formData, content: this.editorContent(), status };

      if (this.isCreatingNew()) {
        // Create new post
        this.postsService.createPost(postData).subscribe({
          next: (createdPost) => {
            this.isLoading.set(false);
            this.isCreatingNew.set(false);
            // Fetch the full post details to keep editing
            this.postsService.getPost(createdPost.id).subscribe(fullPost => {
              this.selectedPost.set(fullPost);
            });
            this.loadPosts(); // Refresh the list
            this.toast.success(`Post ${status === 'Draft' ? 'saved as draft' : 'published'} successfully!`);
          },
          error: (error) => {
            console.error('Error creating post:', error);
            this.errorMessage.set('Failed to create post. Please try again.');
            this.isLoading.set(false);
          }
        });
      } else if (this.selectedPost()) {
        // Update existing post
        const postId = this.selectedPost()!.id;
        const updateData: PostUpdateDto = {
          id: postId,
          ...postData
        };

        this.postsService.updatePost(postId, updateData).subscribe({
          next: (updatedPost) => {
            this.isLoading.set(false);
            this.postsService.getPost(updatedPost.id).subscribe(fullPost => {
              this.selectedPost.set(fullPost);
            });
            this.loadPosts(); // Refresh the list
            this.toast.success(`Post ${status === 'Draft' ? 'saved as draft' : 'published'} successfully!`);
          },
          error: (error) => {
            console.error('Error updating post:', error);
            this.errorMessage.set('Failed to update post. Please try again.');
            this.isLoading.set(false);
          }
        });
      }
    } else {
      this.errorMessage.set('Please fill in all required fields and add content.');
    }
  }

  cancelEdit() {
    this.isCreatingNew.set(false);
    this.selectedPost.set(null);
    this.editorContent.set('');
    this.postForm.reset({
      title: '',
      description: '',
      topicIds: []
    });
    this.errorMessage.set(null);
  }

  onContentChange(content: string) {
    this.editorContent.set(content);
  }

  togglePreview() {
    this.isPreviewMode.set(!this.isPreviewMode());
  }

  editContent() {
    this.isPreviewMode.set(false);
  }

  deletePost(postId: string) {
    if (confirm('Are you sure you want to delete this post?')) {
      this.isLoading.set(true);
      
      this.postsService.deletePost(postId).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.selectedPost.set(null); // Clear the selected post
          this.loadPosts(); // Refresh the list
          this.toast.success('Post deleted successfully!');
        },
        error: (error) => {
          console.error('Error deleting post:', error);
          this.isLoading.set(false);
          this.toast.error('Failed to delete post. Please try again.');
        }
      });
    }
  }

  onTopicChange(topicId: string, event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;
    const currentTopics = this.postForm.get('topicIds')?.value || [];

    if (isChecked) {
      this.postForm.patchValue({
        topicIds: [...currentTopics, topicId]
      });
    } else {
      this.postForm.patchValue({
        topicIds: currentTopics.filter((id: string) => id !== topicId)
      });
    }
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

  getTopicColorHex(topic: TopicDto): string {
    const colorOption = TopicColorOptions.find(c => c.value === topic.color);
    return colorOption?.hex || '#6b7280';
  }
}
