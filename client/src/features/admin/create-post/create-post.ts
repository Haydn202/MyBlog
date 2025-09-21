import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TextEditor } from '../../text-editor/text-editor';
import { PostsService } from '../../../core/services/posts.service';
import { TopicsService } from '../../../core/services/topics.service';
import { PostCreateDto, PostStatus, TopicDto } from '../../../Types/PostCreate';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-post',
  imports: [
    TextEditor,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './create-post.html',
  styleUrl: './create-post.css'
})
export class CreatePost implements OnInit {
  private fb = inject(FormBuilder);
  private postsService = inject(PostsService);
  private topicsService = inject(TopicsService);
  private router = inject(Router);

  postForm!: FormGroup;
  topics = signal<TopicDto[]>([]);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  postStatuses: PostStatus[] = ['Draft', 'Published', 'Deleted'];
  selectedTopics: string[] = [];
  editorContent = '';

  ngOnInit() {
    this.initializeForm();
    this.loadTopics();
  }

  private initializeForm() {
    this.postForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(1000)]],
      thumbnailUrl: [''],
      status: ['Draft', Validators.required],
      content: ['', Validators.required]
    });
  }

  private loadTopics() {
    this.topicsService.getTopics().subscribe({
      next: (topics) => {
        this.topics.set(topics);
      },
      error: (error) => {
        console.error('Error loading topics:', error);
        this.errorMessage.set('Failed to load topics');
      }
    });
  }

  onContentChange(content: string) {
    this.editorContent = content;
    this.postForm.patchValue({ content });
  }

  onTopicChange(topicId: string, isChecked: boolean) {
    if (isChecked) {
      this.selectedTopics.push(topicId);
    } else {
      this.selectedTopics = this.selectedTopics.filter(id => id !== topicId);
    }
  }

  onSubmit() {
    if (this.postForm.valid && this.selectedTopics.length > 0) {
      this.isLoading.set(true);
      this.errorMessage.set(null);

      const formData = this.postForm.value;
      const postData: PostCreateDto = {
        title: formData.title,
        description: formData.description,
        thumbnailUrl: formData.thumbnailUrl || null,
        content: formData.content,
        topicIds: this.selectedTopics,
        status: formData.status
      };

      this.postsService.createPost(postData).subscribe({
        next: (createdPost) => {
          this.isLoading.set(false);
          this.router.navigate(['/admin/manage-posts']);
        },
        error: (error) => {
          this.isLoading.set(false);
          console.error('Error creating post:', error);
          this.errorMessage.set('Failed to create post. Please try again.');
        }
      });
    } else {
      this.errorMessage.set('Please fill in all required fields and select at least one topic.');
    }
  }

  onCancel() {
    this.router.navigate(['/admin/manage-posts']);
  }
}
