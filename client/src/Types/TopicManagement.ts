import { TopicColor } from './TopicColor';

export interface TopicDto {
  id: string;
  name: string;
  description?: string;
  color: TopicColor;
  postCount?: number;
}

export interface TopicCreateDto {
  name: string;
  description?: string;
  color: TopicColor;
}

export interface TopicUpdateDto {
  id: string;
  name: string;
  description?: string;
  color: TopicColor;
}

export interface TopicDeleteDto {
  id: string;
}
