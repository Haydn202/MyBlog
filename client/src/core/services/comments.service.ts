import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CommentDto, CreateCommentDto, UpdateCommentDto, CreateReplyDto, UpdateReplyDto, ReplyDto } from '../../Types/Comment';
import { PaginatedResult } from '../../Types/PaginatedResult';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Posts`;

  getCommentsByPostId(postId: string, pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedResult<CommentDto>> {
    return this.http.get<PaginatedResult<CommentDto>>(`${this.baseUrl}/${postId}/comments?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  createComment(postId: string, comment: CreateCommentDto): Observable<CommentDto> {
    return this.http.post<CommentDto>(`${this.baseUrl}/${postId}/comments`, comment);
  }

  updateComment(postId: string, commentId: string, updateData: UpdateCommentDto): Observable<CommentDto> {
    return this.http.put<CommentDto>(`${this.baseUrl}/${postId}/comments/${commentId}`, updateData);
  }

  deleteComment(postId: string, commentId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${postId}/comments/${commentId}`);
  }

  createReply(postId: string, commentId: string, reply: CreateReplyDto): Observable<ReplyDto> {
    return this.http.post<ReplyDto>(`${this.baseUrl}/${postId}/comments/${commentId}/replies`, reply);
  }

  updateReply(postId: string, commentId: string, replyId: string, updateData: UpdateReplyDto): Observable<ReplyDto> {
    return this.http.put<ReplyDto>(`${this.baseUrl}/${postId}/comments/${commentId}/replies/${replyId}`, updateData);
  }

  deleteReply(postId: string, commentId: string, replyId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${postId}/comments/${commentId}/replies/${replyId}`);
  }
}
