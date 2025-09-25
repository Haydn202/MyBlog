export interface PostFilters {
  status?: 'Draft' | 'Published' | 'Deleted';
  topicId?: string;
  searchTerm?: string;
}
