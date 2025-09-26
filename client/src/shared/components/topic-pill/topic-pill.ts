import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TopicDto } from '../../../Types/TopicManagement';
import { TopicColorOptions } from '../../../Types/TopicColor';

@Component({
  selector: 'app-topic-pill',
  imports: [CommonModule],
  template: `
    <div 
      class="badge badge-xs text-white" 
      [style.background-color]="getTopicColorHex(topic)">
      {{ topic.name }}
    </div>
  `,
  styleUrl: './topic-pill.css'
})
export class TopicPill {
  @Input({ required: true }) topic!: TopicDto;

  getTopicColorHex(topic: TopicDto): string {
    const colorOption = TopicColorOptions.find(c => c.value === topic.color);
    return colorOption?.hex || '#6b7280';
  }
}
