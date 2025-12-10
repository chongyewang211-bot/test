import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProblemService } from '../../services/problem.service';
import { Problem, Category } from '../../models/problem.model';

@Component({
  selector: 'app-post-problem',
  imports: [CommonModule, FormsModule],
  templateUrl: './post-problem.html',
  styleUrls: ['./post-problem.scss']
})
export class PostProblemComponent implements OnInit {
  categories: Category[] = [];
  problem: Partial<Problem> = {
    title: '',
    description: '',
    difficulty: 'Easy',
    category: '',
    tags: []
  };
  tagsInput = '';
  errorMessage = '';
  isLoading = false;
  isSubmitting = false;

  constructor(
    private problemService: ProblemService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.problemService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        if (categories.length > 0 && !this.problem.category) {
          this.problem.category = categories[0].name;
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load categories';
        this.isLoading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/problems']);
  }

  onSubmit(): void {
    if (!this.problem.title || !this.problem.description || !this.problem.category) {
      this.errorMessage = 'Please fill in all required fields';
      return;
    }

    if (this.tagsInput.trim()) {
      this.problem.tags = this.tagsInput.split(',').map(tag => tag.trim()).filter(tag => tag.length > 0);
    } else {
      this.problem.tags = [];
    }

    const problemToSubmit: Problem = {
      id: '',
      problemNumber: 0,
      title: this.problem.title,
      description: this.problem.description,
      difficulty: this.problem.difficulty as 'Easy' | 'Medium' | 'Hard',
      category: this.problem.category,
      tags: this.problem.tags,
      acceptanceRate: 0,
      likes: 0,
      isActive: true,
      createdAt: new Date(),
      updatedAt: new Date()
    };

    this.isSubmitting = true;
    this.errorMessage = '';

    this.problemService.createProblem(problemToSubmit).subscribe({
      next: (createdProblem) => {
        this.router.navigate(['/problems', createdProblem.id]);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to create problem. Please try again.';
        this.isSubmitting = false;
      }
    });
  }
}

