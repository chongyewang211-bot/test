import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProblemService } from '../../services/problem.service';
import { Problem } from '../../models/problem.model';

@Component({
  selector: 'app-problem-detail',
  imports: [CommonModule, RouterModule],
  templateUrl: './problem-detail.html',
  styleUrls: ['./problem-detail.scss']
})
export class ProblemDetailComponent implements OnInit {
  problem: Problem | null = null;
  isLoading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private problemService: ProblemService
  ) {}

  ngOnInit(): void {
    const problemId = this.route.snapshot.paramMap.get('id');
    if (problemId) {
      this.loadProblem(problemId);
    } else {
      this.router.navigate(['/problems']);
    }
  }

  loadProblem(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.problemService.getProblemById(id).subscribe({
      next: (problem) => {
        this.problem = problem;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load problem details. Please try again.';
        this.isLoading = false;
        console.error('Error loading problem:', error);
      }
    });
  }

  getDifficultyClass(difficulty: string): string {
    return `difficulty-${difficulty.toLowerCase()}`;
  }

  goBack(): void {
    this.router.navigate(['/problems']);
  }
}
