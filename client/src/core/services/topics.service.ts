import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {map, tap} from 'rxjs';
import {TopicDto, TopicCreateDto, TopicUpdateDto} from '../../Types/TopicManagement';
import {environment} from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TopicsService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  public topics = signal<TopicDto[]>([]);

  getTopics() {
    return this.http.get<TopicDto[]>(`${this.baseUrl}/topics`).pipe(
      tap(topics => {
        this.topics.set(topics);
      })
    );
  }

  createTopic(topicData: TopicCreateDto) {
    return this.http.post<TopicDto>(`${this.baseUrl}/topics`, topicData);
  }

  updateTopic(id: string, topicData: TopicUpdateDto) {
    return this.http.put<TopicDto>(`${this.baseUrl}/topics/${id}`, topicData);
  }

  deleteTopic(id: string) {
    return this.http.delete(`${this.baseUrl}/topics/${id}`);
  }

  getTopic(id: string) {
    return this.http.get<TopicDto>(`${this.baseUrl}/topics/${id}`);
  }
}
