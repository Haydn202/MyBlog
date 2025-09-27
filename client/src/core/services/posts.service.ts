import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {firstValueFrom, map, tap} from 'rxjs';
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

  getPost(id: string) {
    return this.http.get<PostDto>(`${this.baseUrl}/posts/${id}`);
  }

  updatePost(id: string, postData: PostUpdateDto) {
    return this.http.put<PostSummaryDto>(`${this.baseUrl}/posts/${id}`, postData);
  }

  deletePost(id: string) {
    return this.http.delete(`${this.baseUrl}/posts/${id}`);
  }
}
