import { Component, inject, OnInit, signal, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TopicsService } from '../../../core/services/topics.service';
import { TopicDto, TopicCreateDto, TopicUpdateDto } from '../../../Types/TopicManagement';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-topic-form',
  imports: [
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './topic-form.html',
  styleUrl: './topic-form.css'
})
export class TopicForm implements OnInit {
  private fb = inject(FormBuilder);
  private topicsService = inject(TopicsService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  topicForm!: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  isEditMode = signal(false);
  topicId = signal<string | null>(null);

  // Predefined colors for topics
  topicColors = [
    { name: 'Blue', value: 'bg-primary', hex: '#3b82f6' },
    { name: 'Green', value: 'bg-success', hex: '#10b981' },
    { name: 'Purple', value: 'bg-secondary', hex: '#8b5cf6' },
    { name: 'Orange', value: 'bg-warning', hex: '#f59e0b' },
    { name: 'Red', value: 'bg-error', hex: '#ef4444' },
    { name: 'Teal', value: 'bg-info', hex: '#06b6d4' },
    { name: 'Pink', value: 'bg-accent', hex: '#ec4899' },
    { name: 'Gray', value: 'bg-base-300', hex: '#6b7280' }
  ];

  ngOnInit() {
    this.initializeForm();
    this.checkEditMode();
  }

  private initializeForm() {
    this.topicForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      description: ['', [Validators.maxLength(200)]],
      color: ['bg-primary', Validators.required]
    });
  }

  private checkEditMode() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.topicId.set(id);
      this.loadTopic(id);
    }
  }

  private loadTopic(id: string) {
    this.isLoading.set(true);
    this.topicsService.getTopic(id).subscribe({
      next: (topic) => {
        this.topicForm.patchValue({
          name: topic.name,
          description: topic.description || '',
          color: topic.color || 'bg-primary'
        });
        this.isLoading.set(false);
      },
      error: (error) => {
        this.isLoading.set(false);
        console.error('Error loading topic:', error);
        this.errorMessage.set('Failed to load topic');
      }
    });
  }

  onSubmit() {
    if (this.topicForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set(null);

      const formData = this.topicForm.value;

      if (this.isEditMode()) {
        const updateData: TopicUpdateDto = {
          id: this.topicId()!,
          name: formData.name,
          description: formData.description || undefined,
          color: formData.color
        };

        this.topicsService.updateTopic(this.topicId()!, updateData).subscribe({
          next: () => {
            this.isLoading.set(false);
            this.router.navigate(['/admin/manage-topics']);
          },
          error: (error) => {
            this.isLoading.set(false);
            console.error('Error updating topic:', error);
            this.errorMessage.set('Failed to update topic');
          }
        });
      } else {
        const createData: TopicCreateDto = {
          name: formData.name,
          description: formData.description || undefined,
          color: formData.color
        };

        this.topicsService.createTopic(createData).subscribe({
          next: () => {
            this.isLoading.set(false);
            this.router.navigate(['/admin/manage-topics']);
          },
          error: (error) => {
            this.isLoading.set(false);
            console.error('Error creating topic:', error);
            this.errorMessage.set('Failed to create topic');
          }
        });
      }
    } else {
      this.errorMessage.set('Please fill in all required fields correctly.');
    }
  }

  onCancel() {
    this.router.navigate(['/admin/manage-topics']);
  }

  getPageTitle(): string {
    return this.isEditMode() ? 'Edit Topic' : 'Create New Topic';
  }

  getSubmitButtonText(): string {
    return this.isEditMode() ? 'Update Topic' : 'Create Topic';
  }
}
