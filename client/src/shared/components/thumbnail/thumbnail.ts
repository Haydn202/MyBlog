import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-thumbnail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './thumbnail.html',
  styleUrls: ['./thumbnail.css']
})
export class Thumbnail {
  @Input() thumbnail?: string | null;
  @Input() alt: string = 'Thumbnail';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() rounded: boolean = true;

  getImageClasses(): string {
    const baseClasses = 'object-cover shadow-sm flex-shrink-0';
    const sizeClasses = this.getSizeClasses();
    const roundedClasses = this.rounded ? 'rounded-box' : '';
    
    return `${baseClasses} ${sizeClasses} ${roundedClasses}`.trim();
  }

  getPlaceholderClasses(): string {
    const baseClasses = 'bg-base-200 flex items-center justify-center flex-shrink-0';
    const sizeClasses = this.getSizeClasses();
    const roundedClasses = this.rounded ? 'rounded-box' : '';
    
    return `${baseClasses} ${sizeClasses} ${roundedClasses}`.trim();
  }

  private getSizeClasses(): string {
    switch (this.size) {
      case 'sm':
        return 'w-16 h-16';
      case 'md':
        return 'w-32 h-32';
      case 'lg':
        return 'w-48 h-48';
      default:
        return 'w-32 h-32';
    }
  }
}
