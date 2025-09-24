import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TopicsService } from '../../../core/services/topics.service';
import { TopicDto } from '../../../Types/TopicManagement';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manage-topics',
  imports: [
    CommonModule
  ],
  templateUrl: './manage-topics.html',
  styleUrl: './manage-topics.css'
})
export class ManageTopics implements OnInit {
  private topicsService = inject(TopicsService);
  private router = inject(Router);

  topics = this.topicsService.topics;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  showDeleteModal = signal(false);
  topicToDelete = signal<TopicDto | null>(null);

  ngOnInit() {
    this.loadTopics();
  }

  private loadTopics() {
    this.isLoading.set(true);
    this.topicsService.getTopics().subscribe({
      next: () => {
        this.isLoading.set(false);
        this.errorMessage.set(null);
      },
      error: (error) => {
        this.isLoading.set(false);
        console.error('Error loading topics:', error);
        this.errorMessage.set('Failed to load topics');
      }
    });
  }

  createNewTopic() {
    this.router.navigate(['/admin/create-topic']);
  }

  editTopic(topicId: string) {
    this.router.navigate(['/admin/edit-topic', topicId]);
  }

  confirmDeleteTopic(topic: TopicDto) {
    this.topicToDelete.set(topic);
    this.showDeleteModal.set(true);
  }

  cancelDelete() {
    this.showDeleteModal.set(false);
    this.topicToDelete.set(null);
  }

  deleteTopic() {
    const topic = this.topicToDelete();
    if (!topic) return;

    this.isLoading.set(true);
    this.topicsService.deleteTopic(topic.id).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.showDeleteModal.set(false);
        this.topicToDelete.set(null);
        this.loadTopics(); // Refresh the list
      },
      error: (error) => {
        this.isLoading.set(false);
        console.error('Error deleting topic:', error);
        this.errorMessage.set('Failed to delete topic');
      }
    });
  }

  getTopicColor(topic: TopicDto): string {
    if (topic.color) {
      return topic.color;
    }
    // Generate a color based on the topic name
    const colors = [
      'bg-primary', 'bg-secondary', 'bg-accent', 'bg-info',
      'bg-success', 'bg-warning', 'bg-error'
    ];
    const index = topic.name.length % colors.length;
    return colors[index];
  }
}
