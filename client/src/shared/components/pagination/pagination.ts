import {Component, input, output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PaginationMetadata} from '../../../Types/PaginatedResult';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css'
})
export class Pagination {
  metadata = input.required<PaginationMetadata | null>();
  pageChange = output<number>();
  
  // Expose Math for template use
  protected readonly Math = Math;

  onPageChange(page: number) {
    if (this.metadata() && page >= 1 && page <= this.metadata()!.totalPages) {
      this.pageChange.emit(page);
    }
  }

  getPageNumbers(): number[] {
    if (!this.metadata()) return [];
    
    const metadata = this.metadata()!;
    const pages: number[] = [];
    const totalPages = metadata.totalPages;
    const currentPage = metadata.currentPage;
    
    // Show max 7 page numbers
    const maxPagesToShow = 7;
    
    if (totalPages <= maxPagesToShow) {
      // Show all pages if total pages is less than max
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Always show first page
      pages.push(1);
      
      // Calculate start and end of middle range
      let start = Math.max(2, currentPage - 2);
      let end = Math.min(totalPages - 1, currentPage + 2);
      
      // Add ellipsis after first page if needed
      if (start > 2) {
        pages.push(-1); // -1 represents ellipsis
      }
      
      // Add middle pages
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
      
      // Add ellipsis before last page if needed
      if (end < totalPages - 1) {
        pages.push(-1); // -1 represents ellipsis
      }
      
      // Always show last page
      pages.push(totalPages);
    }
    
    return pages;
  }
}

