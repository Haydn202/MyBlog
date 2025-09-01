import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {firstValueFrom, map, tap} from 'rxjs';
import {PaginatedResult} from '../../Types/PaginatedResult';
import {PostSummaryDto} from '../../Types/PostSummary';

@Injectable({
  providedIn: 'root'
})
export class PostsService {
  private http = inject(HttpClient);
  baseUrl = 'http://localhost:5285';
  public posts = signal<PostSummaryDto[]>([]);

  getPosts(){
    return this.http.get<PaginatedResult<PostSummaryDto>>(`${this.baseUrl}/posts`).pipe(
      map(response => {
        return response.items;
      }),
      tap(posts => {
        this.posts.set(posts);
      })
    );
  }
}
