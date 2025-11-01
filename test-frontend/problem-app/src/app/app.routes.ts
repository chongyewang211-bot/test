import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { ProblemListComponent } from './components/problem-list/problem-list';
import { ProblemDetailComponent } from './components/problem-detail/problem-detail';
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
  { path: '**', redirectTo: '/login' }
];
