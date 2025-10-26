import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {firstValueFrom, map, Observable, of, tap} from 'rxjs';
import {PaginatedResult, PaginationMetadata} from '../../Types/PaginatedResult';
import {PostSummaryDto} from '../../Types/PostSummary';
import {PostCreateDto, PostDto, PostUpdateDto} from '../../Types/PostCreate';
import {PostFilters} from '../../Types/PostFilters';
import {PagingParams} from '../../Types/PagingParams';
import {environment} from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PostsService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  public posts = signal<PostSummaryDto[]>([]);
  public paginationMetadata = signal<PaginationMetadata | null>(null);
  private postCache = new Map<string, PostDto>();

  getPosts(filters?: PostFilters, pagingParams?: PagingParams){
    let url = `${this.baseUrl}/posts/All`;
    const params = new URLSearchParams();

    // Add paging parameters
    if (pagingParams?.pageNumber) {
      params.append('pageNumber', pagingParams.pageNumber.toString());
    }
    if (pagingParams?.pageSize) {
      params.append('pageSize', pagingParams.pageSize.toString());
    }

    // Add filters
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
      tap(response => {
        this.posts.set(response.items);
        this.paginationMetadata.set(response.metadata);
      })
    );
  }

  getPublicPosts(filters?: PostFilters, pagingParams?: PagingParams, appendResults: boolean = false){
    let url = `${this.baseUrl}/posts`;
    const params = new URLSearchParams();

    // Add paging parameters
    if (pagingParams?.pageNumber) {
      params.append('pageNumber', pagingParams.pageNumber.toString());
    }
    if (pagingParams?.pageSize) {
      params.append('pageSize', pagingParams.pageSize.toString());
    }

    // Add filters
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
      tap(response => {
        if (appendResults) {
          // Append new posts to existing ones for infinite scroll
          this.posts.set([...this.posts(), ...response.items]);
        } else {
          // Replace posts (normal behavior)
          this.posts.set(response.items);
        }
        this.paginationMetadata.set(response.metadata);
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
