export interface TopicDto {
  id: string;
  name: string;
  description?: string;
  color?: string;
  postCount?: number;
}

export interface TopicCreateDto {
  name: string;
  description?: string;
  color?: string;
}

export interface TopicUpdateDto {
  id: string;
  name: string;
  description?: string;
  color?: string;
}

export interface TopicDeleteDto {
  id: string;
}
