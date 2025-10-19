import { Component, Input, Output, EventEmitter, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommentDto, CreateReplyDto, UpdateCommentDto } from '../../../Types/Comment';
import { CommentsService } from '../../../core/services/comments.service';
import { ToastService } from '../../../core/services/toast.service';
import { AccountService } from '../../../core/services/account.service';
import { ConfirmationService } from '../../../core/services/confirmation.service';


@Component({
  selector: 'app-comment',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './comment.html',
  styleUrls: ['./comment.css']
})
export class Comment {
  @Input({ required: true }) comment!: CommentDto;
  @Input() postId!: string;
  @Input() isReply: boolean = false;
  @Input() parentCommentId?: string; // For nested replies - the top-level comment ID
  @Input() directParentCommentId?: string; // The immediate parent comment ID for API calls
  @Output() commentUpdated = new EventEmitter<void>();
  @Output() commentDeleted = new EventEmitter<void>();
  
  private commentsService = inject(CommentsService);
  private toastService = inject(ToastService);
  private accountService = inject(AccountService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);
  
  isReplying = signal(false);
  isEditing = signal(false);
  isLoading = signal(false);
  
  replyForm!: FormGroup;
  editForm!: FormGroup;
  
  ngOnInit() {
    this.initializeForms();
  }
  
  isCurrentUserAuthor(): boolean {
    const currentUser = this.accountService.currentUser();
    return currentUser?.id === this.comment.userId;
  }
  
  private initializeForms() {
    this.replyForm = this.fb.group({
      message: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(1000)]]
    });
    
    this.editForm = this.fb.group({
      message: [this.comment.message, [Validators.required, Validators.minLength(1), Validators.maxLength(1000)]]
    });
  }
  
  startReply() {
    this.isReplying.set(true);
    this.replyForm.reset();
  }
  
  cancelReply() {
    this.isReplying.set(false);
    this.replyForm.reset();
  }
  
  startEdit() {
    this.isEditing.set(true);
    this.editForm.patchValue({ message: this.comment.message });
  }
  
  cancelEdit() {
    this.isEditing.set(false);
    this.editForm.patchValue({ message: this.comment.message });
  }
  
  submitReply() {
    if (this.replyForm.valid) {
      this.isLoading.set(true);
      
      const currentUser = this.accountService.currentUser();
      if (!currentUser) {
        this.toastService.error('You must be logged in to reply.');
        this.isLoading.set(false);
        return;
      }
      
      // Use parent comment ID if this is a reply to a reply, otherwise use current comment ID
      const targetCommentId = this.parentCommentId || this.comment.id;
      
      const replyData: CreateReplyDto = {
        message: this.replyForm.value.message,
        commentId: targetCommentId,
        userId: currentUser.id
      };
      
      this.commentsService.createReply(this.postId, targetCommentId, replyData).subscribe({
        next: (newReply) => {
          this.isReplying.set(false);
          this.replyForm.reset();
          this.isLoading.set(false);
          this.commentUpdated.emit();
        },
        error: (error) => {
          console.error('Error posting reply:', error);
          this.toastService.error('Failed to post reply. Please try again.');
          this.isLoading.set(false);
        }
      });
    }
  }
  
  submitEdit() {
    if (this.editForm.valid) {
      this.isLoading.set(true);
      
      const updateData = {
        message: this.editForm.value.message
      };
      
      // If this is a reply, use updateReply, otherwise use updateComment
      if (this.isReply && this.directParentCommentId) {
        this.commentsService.updateReply(this.postId, this.directParentCommentId, this.comment.id, updateData).subscribe({
          next: (updatedReply) => {
            this.comment.message = updatedReply.message;
            this.isEditing.set(false);
            this.isLoading.set(false);
            this.commentUpdated.emit();
          },
          error: (error) => {
            console.error('Error updating reply:', error);
            this.toastService.error('Failed to update reply. Please try again.');
            this.isLoading.set(false);
          }
        });
      } else {
        this.commentsService.updateComment(this.postId, this.comment.id, updateData).subscribe({
          next: (updatedComment) => {
            this.comment.message = updatedComment.message;
            this.isEditing.set(false);
            this.isLoading.set(false);
            this.commentUpdated.emit();
          },
          error: (error) => {
            console.error('Error updating comment:', error);
            this.toastService.error('Failed to update comment. Please try again.');
            this.isLoading.set(false);
          }
        });
      }
    }
  }
  
  async deleteComment() {
    const itemType = this.isReply ? 'reply' : 'comment';
    
    const confirmed = await this.confirmationService.confirm({
      title: `Delete ${itemType.charAt(0).toUpperCase() + itemType.slice(1)}`,
      message: `Are you sure you want to delete this ${itemType}? This action cannot be undone.`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      confirmButtonClass: 'btn-error'
    });
    
    if (confirmed) {
      this.isLoading.set(true);
      
      // If this is a reply, use deleteReply, otherwise use deleteComment
      if (this.isReply && this.directParentCommentId) {
        this.commentsService.deleteReply(this.postId, this.directParentCommentId, this.comment.id).subscribe({
          next: () => {
            this.isLoading.set(false);
            this.commentDeleted.emit();
          },
          error: (error) => {
            console.error('Error deleting reply:', error);
            this.toastService.error('Failed to delete reply. Please try again.');
            this.isLoading.set(false);
          }
        });
      } else {
        this.commentsService.deleteComment(this.postId, this.comment.id).subscribe({
          next: () => {
            this.isLoading.set(false);
            this.commentDeleted.emit();
          },
          error: (error) => {
            console.error('Error deleting comment:', error);
            this.toastService.error('Failed to delete comment. Please try again.');
            this.isLoading.set(false);
          }
        });
      }
    }
  }
  
  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
