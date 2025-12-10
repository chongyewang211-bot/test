import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { OnlineUser } from '../../models/user.model';

@Component({
  selector: 'app-online-users',
  imports: [CommonModule],
  templateUrl: './online-users.html',
  styleUrls: ['./online-users.scss']
})
export class OnlineUsersComponent implements OnInit {
  users: OnlineUser[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.userService.getOnlineUsers().subscribe({
      next: (users) => {
        this.users = users.filter(u => u.isActive);
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load users';
        this.isLoading = false;
      }
    });
  }

  getInitials(username: string): string {
    return username.charAt(0).toUpperCase();
  }
}

