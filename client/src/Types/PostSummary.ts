export interface TopicDto {
  id: string;       // adjust type depending on your C# TopicDto definition
  name: string;
}

export interface PostSummaryDto {
  id: string;                      // Guid → string
  title: string;
  description: string;
  topics: TopicDto[];
  createdOn: string;               // DateTime → ISO string in JSON
  thumbnailUrl?: string | null;    // nullable in C#
}
