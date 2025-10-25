import {Component, input} from '@angular/core';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-post-skeleton',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './post-skeleton.html',
  styleUrl: './post-skeleton.css'
})
export class PostSkeleton {
  count = input<number>(1);

  get skeletonArray(): number[] {
    return Array(this.count()).fill(0);
  }
}

