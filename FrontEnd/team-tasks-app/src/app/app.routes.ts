import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'projects/:id',
    loadComponent: () => import('./features/projects/project-tasks.component').then(m => m.ProjectTasksComponent)
  },
  {
    path: 'tasks/new',
    loadComponent: () => import('./features/tasks/task-form.component').then(m => m.TaskFormComponent)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
