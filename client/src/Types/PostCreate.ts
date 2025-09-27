import { TopicColor } from './TopicColor';

export interface PostCreateDto {
  title: string;
  description: string;
  thumbnailUrl?: string | null;
  content: string;
  topicIds: string[];
  status: PostStatus;
}

export interface PostUpdateDto {
  id: string;
  title: string;
  description: string;
  thumbnailUrl?: string | null;
  content: string;
  topicIds: string[];
  status: PostStatus;
}

export interface PostDto {
  id: string;
  title: string;
  description: string;
  topics: TopicDto[];
  createdOn: string;
  content: string;
  mainComments?: any[] | null;
  thumbnailUrl?: string | null;
  status: PostStatus;
}

export type PostStatus = 'Draft' | 'Published' | 'Deleted';

export interface TopicDto {
  id: string;
  name: string;
  color: TopicColor;
}
