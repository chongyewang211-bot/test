import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { ProblemListComponent } from './components/problem-list/problem-list';
import { ProblemDetailComponent } from './components/problem-detail/problem-detail';
import { PostProblemComponent } from './components/post-problem/post-problem';
import { CommentsComponent } from './components/comments/comments';
import { OnlineUsersComponent } from './components/online-users/online-users';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { 
    path: 'problems', 
    component: ProblemListComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'problems/:id', 
    component: ProblemDetailComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'post-problem', 
    component: PostProblemComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'comments', 
    component: CommentsComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'online-users', 
    component: OnlineUsersComponent,
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/login' }
];
