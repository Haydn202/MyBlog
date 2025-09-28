import { Component, Input, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Comment } from '../comment/comment';
import { CommentsService } from '../../../core/services/comments.service';
import { ToastService } from '../../../core/services/toast.service';
import { CommentDto, CreateCommentDto } from '../../../Types/Comment';
import { AccountService } from '../../../core/services/account.service';

@Component({
  selector: 'app-comments-section',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, Comment],
  templateUrl: './comments-section.html',
  styleUrls: ['./comments-section.css']
})
export class CommentsSection implements OnInit {
  @Input({ required: true }) postId!: string;
  
  private commentsService = inject(CommentsService);
  private toastService = inject(ToastService);
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  
  comments = signal<CommentDto[]>([]);
  isLoading = signal(false);
  isSubmitting = signal(false);
  
  commentForm!: FormGroup;
  
  ngOnInit() {
    this.initializeForm();
    this.loadComments();
  }
  
  private initializeForm() {
    this.commentForm = this.fb.group({
      message: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(1000)]]
    });
  }
  
  private loadComments() {
    this.isLoading.set(true);
    
    this.commentsService.getCommentsByPostId(this.postId).subscribe({
      next: (result) => {
        this.comments.set(result.items);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading comments:', error);
        this.toastService.error('Failed to load comments. Please try again.');
        this.isLoading.set(false);
      }
    });
  }
  
  submitComment() {
    if (this.commentForm.valid) {
      this.isSubmitting.set(true);
      
      const currentUser = this.accountService.currentUser();
      if (!currentUser) {
        this.toastService.error('You must be logged in to comment.');
        this.isSubmitting.set(false);
        return;
      }
      
      const commentData: CreateCommentDto = {
        message: this.commentForm.value.message,
        userId: currentUser.id
      };
      
      this.commentsService.createComment(this.postId, commentData).subscribe({
        next: (newComment) => {
          this.toastService.success('Comment posted successfully!');
          this.commentForm.reset();
          this.loadComments(); // Reload all comments
          this.isSubmitting.set(false);
        },
        error: (error) => {
          console.error('Error posting comment:', error);
          this.toastService.error('Failed to post comment. Please try again.');
          this.isSubmitting.set(false);
        }
      });
    }
  }
  
  onCommentUpdated() {
    // Reload comments when a comment is updated
    this.loadComments();
  }
  
  onCommentDeleted() {
    // Reload comments when a comment is deleted
    this.loadComments();
  }
}
