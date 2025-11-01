import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Problem, Category } from '../models/problem.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProblemService {
  private apiUrl = `${environment.apiUrl}/problems`;

  constructor(private http: HttpClient) {}

  getProblems(category?: string): Observable<Problem[]> {
    let params = new HttpParams();
    if (category && category !== 'all') {
      params = params.set('category', category);
    }
    return this.http.get<Problem[]>(this.apiUrl, { params });
  }

  getProblemById(id: string): Observable<Problem> {
    return this.http.get<Problem>(`${this.apiUrl}/${id}`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
  }

  createProblem(problem: Problem): Observable<Problem> {
    return this.http.post<Problem>(this.apiUrl, problem);
  }

  updateProblem(id: string, problem: Problem): Observable<Problem> {
    return this.http.put<Problem>(`${this.apiUrl}/${id}`, problem);
  }

  deleteProblem(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  createTestProblem(): Observable<Problem> {
    return this.http.post<Problem>(`${this.apiUrl}/test-create`, {});
  }
}

