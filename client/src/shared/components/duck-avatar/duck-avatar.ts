import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-duck-avatar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './duck-avatar.html',
  styleUrls: ['./duck-avatar.css']
})
export class DuckAvatar {
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() rounded: boolean = true;

  getSizeClasses(): string {
    switch (this.size) {
      case 'sm':
        return 'w-6 h-6';
      case 'md':
        return 'w-8 h-8';
      case 'lg':
        return 'w-12 h-12';
      default:
        return 'w-8 h-8';
    }
  }

  getRoundedClasses(): string {
    return this.rounded ? 'rounded-full' : '';
  }
}
