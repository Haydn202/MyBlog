import { Component, inject, OnInit, signal, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { Router } from '@angular/router';
import { PostsService } from '../../../core/services/posts.service';
import { PostSummaryDto } from '../../../Types/PostSummary';
import { TopicsService } from '../../../core/services/topics.service';
import { TopicDto } from '../../../Types/TopicManagement';
import { PostDto, PostUpdateDto } from '../../../Types/PostCreate';
import { TextEditor } from '../../text-editor/text-editor';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastService } from '../../../core/services/toast.service';
import { TopicPill } from '../../../shared/components/topic-pill/topic-pill';
import { TopicColorOptions } from '../../../Types/TopicColor';
import { ConfirmationService } from '../../../core/services/confirmation.service';

@Component({
  selector: 'app-post-editor',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TextEditor
  ],
  templateUrl: './post-editor.html',
  styleUrl: './post-editor.css'
})
export class PostEditor implements OnInit, OnChanges {
  @Input() selectedPost = signal<PostDto | null>(null);
  @Input() isCreatingNew = signal(false);
  @Output() postSaved = new EventEmitter<void>();
  @Output() postDeleted = new EventEmitter<void>();
  @Output() editCancelled = new EventEmitter<void>();

  private postsService = inject(PostsService);
  private topicsService = inject(TopicsService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private toast = inject(ToastService);
  private confirmationService = inject(ConfirmationService);

  topics = this.topicsService.topics;
  postForm!: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  editorContent = signal<string>('');
  isPreviewMode = signal(false);

  ngOnInit() {
    this.initializeForm();
    this.loadTopics();
    
    // Check if we have a selected post after form initialization
    if (this.selectedPost()) {
      this.populateForm(this.selectedPost()!);
    } else if (this.isCreatingNew()) {
      this.resetForm();
    }
  }

  private initializeForm() {
    this.postForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(1000)]],
      topicIds: [[], Validators.required]
    });
  }

  private loadTopics() {
    this.topicsService.getTopics().subscribe();
  }

  ngOnChanges() {
    // Only populate form if it's been initialized
    if (this.postForm) {
      if (this.selectedPost()) {
        this.populateForm(this.selectedPost()!);
      } else if (this.isCreatingNew()) {
        this.resetForm();
      }
    }
  }

  private populateForm(post: PostDto) {
    this.postForm.patchValue({
      title: post.title,
      description: post.description,
      topicIds: post.topics.map(t => t.id)
    });
    this.editorContent.set(post.content);
  }

  private resetForm() {
    this.editorContent.set('');
    this.postForm.reset({
      title: '',
      description: '',
      topicIds: []
    });
    this.errorMessage.set(null);
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
            this.postSaved.emit();
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
            // Fetch the full post details to keep editing
            this.postsService.getPost(updatedPost.id).subscribe(fullPost => {
              this.selectedPost.set(fullPost);
            });
            this.postSaved.emit();
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
    this.editCancelled.emit();
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

  async deletePost(postId: string) {
    const confirmed = await this.confirmationService.confirm({
      title: 'Delete Post',
      message: 'Are you sure you want to delete this post? This action cannot be undone.',
      confirmText: 'Delete',
      cancelText: 'Cancel',
      confirmButtonClass: 'btn-error'
    });

    if (confirmed) {
      this.isLoading.set(true);
      
      this.postsService.deletePost(postId).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.postDeleted.emit();
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

  getTopicColorHex(topic: TopicDto): string {
    const colorOption = TopicColorOptions.find(c => c.value === topic.color);
    return colorOption?.hex || '#6b7280';
  }
}
