import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CommentService } from '../../services/comment.service';
import { AuthService } from '../../services/auth.service';
import { Comment } from '../../models/comment.model';

@Component({
  selector: 'app-comments',
  imports: [CommonModule, FormsModule],
  templateUrl: './comments.html',
  styleUrls: ['./comments.scss']
})
export class CommentsComponent implements OnInit {
  comments: Comment[] = [];
  newComment = '';
  isLoading = false;
  isSubmitting = false;
  errorMessage = '';

  constructor(
    private commentService: CommentService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadComments();
  }

  loadComments(): void {
    this.isLoading = true;
    this.commentService.getComments().subscribe({
      next: (comments) => {
        this.comments = comments;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load comments';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (!this.newComment.trim()) {
      this.errorMessage = 'Please enter a comment';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    this.commentService.createComment(this.newComment.trim()).subscribe({
      next: (comment) => {
        this.comments.unshift(comment);
        this.newComment = '';
        this.isSubmitting = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to post comment';
        this.isSubmitting = false;
      }
    });
  }

  formatDate(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleString();
  }
}

