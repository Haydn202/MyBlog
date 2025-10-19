import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {firstValueFrom, map, Observable, of, tap} from 'rxjs';
import {PaginatedResult} from '../../Types/PaginatedResult';
import {PostSummaryDto} from '../../Types/PostSummary';
import {PostCreateDto, PostDto, PostUpdateDto} from '../../Types/PostCreate';
import {PostFilters} from '../../Types/PostFilters';

@Injectable({
  providedIn: 'root'
})
export class PostsService {
  private http = inject(HttpClient);
  baseUrl = 'http://localhost:5285';
  public posts = signal<PostSummaryDto[]>([]);
  private postCache = new Map<string, PostDto>();

  getPosts(filters?: PostFilters){
    let url = `${this.baseUrl}/posts/All`;
    const params = new URLSearchParams();

    if (filters) {
      if (filters.status) {
        params.append('status', filters.status);
      }
      if (filters.topicId) {
        params.append('topicId', filters.topicId);
      }
      if (filters.searchTerm) {
        params.append('searchTerm', filters.searchTerm);
      }
    }

    if (params.toString()) {
      url += `?${params.toString()}`;
    }

    return this.http.get<PaginatedResult<PostSummaryDto>>(url).pipe(
      map(response => {
        return response.items;
      }),
      tap(posts => {
        this.posts.set(posts);
      })
    );
  }

  getPublicPosts(filters?: PostFilters){
    let url = `${this.baseUrl}/posts`;
    const params = new URLSearchParams();

    if (filters) {
      if (filters.topicId) {
        params.append('topicId', filters.topicId);
      }
      if (filters.searchTerm) {
        params.append('searchTerm', filters.searchTerm);
      }
    }

    if (params.toString()) {
      url += `?${params.toString()}`;
    }

    return this.http.get<PaginatedResult<PostSummaryDto>>(url).pipe(
      map(response => {
        return response.items;
      }),
      tap(posts => {
        this.posts.set(posts);
      })
    );
  }

  createPost(postData: PostCreateDto) {
    return this.http.post<PostSummaryDto>(`${this.baseUrl}/posts`, postData);
  }

  getPost(id: string): Observable<PostDto> {
    const cached = this.postCache.get(id);
    if (cached) {
      return of(cached);
    }

    return this.http.get<PostDto>(`${this.baseUrl}/posts/${id}`).pipe(
      tap(post => this.postCache.set(id, post))
    );
  }

  clearPostCache(id?: string) {
    if (id) {
      this.postCache.delete(id);
    } else {
      this.postCache.clear();
    }
  }

  updatePost(id: string, postData: PostUpdateDto) {
    return this.http.put<PostSummaryDto>(`${this.baseUrl}/posts/${id}`, postData).pipe(
      tap(() => this.clearPostCache(id))
    );
  }

  deletePost(id: string) {
    return this.http.delete(`${this.baseUrl}/posts/${id}`).pipe(
      tap(() => this.clearPostCache(id))
    );
  }
}
