import {Component, Input} from '@angular/core';
import {PostSummaryDto} from '../../Types/PostSummary';

@Component({
  selector: 'app-summary-card',
  imports: [],
  templateUrl: './summary-card.html',
  styleUrl: './summary-card.css'
})
export class SummaryCard {
  @Input({required: true}) post: PostSummaryDto | undefined;

  onImageError(event: Event) {
    const img = event.target as HTMLImageElement;
    img.style.display = 'none';
    // The fallback placeholder will be shown instead
  }
}
