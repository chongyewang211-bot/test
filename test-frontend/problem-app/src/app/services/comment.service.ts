import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment, CreateCommentRequest } from '../models/comment.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl = `${environment.apiUrl}/comments`;

  constructor(private http: HttpClient) {}

  getComments(): Observable<Comment[]> {
    return this.http.get<Comment[]>(this.apiUrl);
  }

  createComment(content: string): Observable<Comment> {
    const request: CreateCommentRequest = { content };
    return this.http.post<Comment>(this.apiUrl, request);
  }

  deleteComment(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

