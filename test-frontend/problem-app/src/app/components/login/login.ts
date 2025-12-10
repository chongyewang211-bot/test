import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest, RegisterRequest } from '../../models/user.model';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent {
  isRegisterMode = false;
  
  credentials: LoginRequest = {
    username: '',
    password: ''
  };
  
  registerData: RegisterRequest = {
    username: '',
    email: '',
    password: ''
  };
  
  errorMessage = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  toggleMode(): void {
    this.isRegisterMode = !this.isRegisterMode;
    this.errorMessage = '';
    this.credentials = { username: '', password: '' };
    this.registerData = { username: '', email: '', password: '' };
  }

  onSubmit(): void {
    if (this.isRegisterMode) {
      this.onRegister();
    } else {
      this.onLogin();
    }
  }

  onLogin(): void {
    if (!this.credentials.username || !this.credentials.password) {
      this.errorMessage = 'Please enter both username and password';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.credentials).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/problems';
        this.router.navigate([returnUrl]);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Login failed. Please check your credentials.';
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  onRegister(): void {
    if (!this.registerData.username || !this.registerData.email || !this.registerData.password) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    if (this.registerData.password.length < 3) {
      this.errorMessage = 'Password must be at least 3 characters';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.register(this.registerData).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/problems';
        this.router.navigate([returnUrl]);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Registration failed. Please try again.';
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
}
