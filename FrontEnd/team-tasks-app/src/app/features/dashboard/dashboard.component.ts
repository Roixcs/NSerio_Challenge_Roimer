import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { DashboardService, ProjectService } from '../../core/services';
import { DeveloperWorkload, DeveloperDelayRisk, Project } from '../../models';
import { StatusBadgePipe } from '../../shared/pipes/status-badge.pipe';
import { HighlightRiskDirective } from '../../shared/directives/highlight-risk.directive';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatSortModule,
    MatProgressSpinnerModule,
    StatusBadgePipe,
    HighlightRiskDirective
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  // Data
  developerWorkload: DeveloperWorkload[] = [];
  sortedWorkload: DeveloperWorkload[] = [];
  projects: Project[] = [];
  delayRisk: DeveloperDelayRisk[] = [];

  // Loading states
  loadingWorkload = true;
  loadingProjects = true;
  loadingRisk = true;

  // Column definitions
  workloadColumns = ['developerName', 'openTasksCount', 'averageEstimatedComplexity'];
  projectColumns = ['name', 'clientName', 'totalTasks', 'openTasks', 'completedTasks'];
  riskColumns = ['developerName', 'openTasksCount', 'avgDelayDays', 'nearestDueDate', 'latestDueDate', 'predictedCompletionDate', 'highRiskFlag'];

  constructor(
    private dashboardService: DashboardService,
    private projectService: ProjectService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDeveloperWorkload();
    this.loadProjects();
    this.loadDelayRisk();
  }

  loadDeveloperWorkload(): void {
    this.dashboardService.getDeveloperWorkload().subscribe({
      next: (data) => {
        this.developerWorkload = data;
        this.sortedWorkload = [...data];
        this.loadingWorkload = false;
      },
      error: (err) => {
        console.error('Error loading developer workload:', err);
        this.loadingWorkload = false;
      }
    });
  }

  loadProjects(): void {
    this.projectService.getProjects().subscribe({
      next: (data) => {
        this.projects = data;
        this.loadingProjects = false;
      },
      error: (err) => {
        console.error('Error loading projects:', err);
        this.loadingProjects = false;
      }
    });
  }

  loadDelayRisk(): void {
    this.dashboardService.getDeveloperDelayRisk().subscribe({
      next: (data) => {
        this.delayRisk = data;
        this.loadingRisk = false;
      },
      error: (err) => {
        console.error('Error loading delay risk:', err);
        this.loadingRisk = false;
      }
    });
  }

  sortWorkload(sort: Sort): void {
    if (!sort.active || sort.direction === '') {
      this.sortedWorkload = [...this.developerWorkload];
      return;
    }

    this.sortedWorkload = [...this.developerWorkload].sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'developerName':
          return this.compare(a.developerName, b.developerName, isAsc);
        case 'openTasksCount':
          return this.compare(a.openTasksCount, b.openTasksCount, isAsc);
        case 'averageEstimatedComplexity':
          return this.compare(a.averageEstimatedComplexity, b.averageEstimatedComplexity, isAsc);
        default:
          return 0;
      }
    });
  }

  compare(a: string | number, b: string | number, isAsc: boolean): number {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  goToProject(projectId: number): void {
    this.router.navigate(['/projects', projectId]);
  }

  openNewTaskDialog(): void {
    this.router.navigate(['/tasks', 'new']);
  }
}
