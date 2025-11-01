import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ProblemService } from '../../services/problem.service';
import { Problem, Category } from '../../models/problem.model';

@Component({
  selector: 'app-problem-list',
  imports: [CommonModule, RouterModule],
  templateUrl: './problem-list.html',
  styleUrls: ['./problem-list.scss']
})
export class ProblemListComponent implements OnInit {
  problems: Problem[] = [];
  categories: Category[] = [];
  selectedCategory = 'all';
  isLoading = true;
  errorMessage = '';

  constructor(
    private problemService: ProblemService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProblems();
  }

  loadCategories(): void {
    this.problemService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadProblems(category: string = 'all'): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.selectedCategory = category;

    this.problemService.getProblems(category).subscribe({
      next: (problems) => {
        this.problems = problems;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load problems. Please try again.';
        this.isLoading = false;
        console.error('Error loading problems:', error);
      }
    });
  }

  filterByCategory(category: string): void {
    this.loadProblems(category);
  }

  getDifficultyClass(difficulty: string): string {
    return `difficulty-${difficulty.toLowerCase()}`;
  }

  viewProblemDetail(problemId: string): void {
    this.router.navigate(['/problems', problemId]);
  }
}
