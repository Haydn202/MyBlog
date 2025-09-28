import { TopicColor } from './TopicColor';

export interface TopicDto {
  id: string;       // adjust type depending on your C# TopicDto definition
  name: string;
  color: TopicColor;
}

export interface PostSummaryDto {
  id: string;                      // Guid → string
  title: string;
  description: string;
  topics: TopicDto[];
  createdOn: string;               // DateTime → ISO string in JSON
  thumbnail?: string | null;    // nullable in C#
  status: 'Draft' | 'Published' | 'Deleted';
}
