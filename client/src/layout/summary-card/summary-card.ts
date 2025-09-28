import {Component, Input} from '@angular/core';
import {PostSummaryDto} from '../../Types/PostSummary';
import {Thumbnail} from '../../shared/components/thumbnail/thumbnail';

@Component({
  selector: 'app-summary-card',
  imports: [Thumbnail],
  templateUrl: './summary-card.html',
  styleUrl: './summary-card.css'
})
export class SummaryCard {
  @Input({required: true}) post: PostSummaryDto | undefined;
}
