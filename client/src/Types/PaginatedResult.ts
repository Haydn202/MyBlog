export interface PaginationMetadata {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
}

export interface PaginatedResult<T> {
  metadata: PaginationMetadata;
  items: T[];
}
