export interface CommentDto {
  id: string;
  message: string;
  userName: string;
  createdOn: string;
  replies?: ReplyDto[];
}

export interface ReplyDto {
  id: string;
  message: string;
  userName: string;
  createdOn: string;
}

export interface CreateCommentDto {
  message: string;
  userId: string;
}

export interface CreateReplyDto {
  message: string;
  commentId: string;
  userId: string;
}

export interface UpdateCommentDto {
  message: string;
}

export interface UpdateReplyDto {
  message: string;
}
