import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {map, tap} from 'rxjs';
import {TopicDto} from '../../Types/PostCreate';

@Injectable({
  providedIn: 'root'
})
export class TopicsService {
  private http = inject(HttpClient);
  baseUrl = 'http://localhost:5285';
  public topics = signal<TopicDto[]>([]);

  getTopics() {
    return this.http.get<TopicDto[]>(`${this.baseUrl}/topics`).pipe(
      tap(topics => {
        this.topics.set(topics);
      })
    );
  }
}
