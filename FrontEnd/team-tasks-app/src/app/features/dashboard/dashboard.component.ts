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
  template: `
    <div class="dashboard-container">
      <header class="dashboard-header">
        <h1>Team Tasks Dashboard</h1>
        <button mat-raised-button color="primary" (click)="openNewTaskDialog()">
          <mat-icon>add</mat-icon>
          New Task
        </button>
      </header>

      <div class="dashboard-grid">
        <!-- Developer Workload Table -->
        <mat-card class="dashboard-card">
          <mat-card-header>
            <mat-card-title>Developer Workload</mat-card-title>
            <mat-card-subtitle>Open tasks per active developer</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            @if (loadingWorkload) {
              <div class="loading-container">
                <mat-spinner diameter="40"></mat-spinner>
              </div>
            } @else {
              <table mat-table [dataSource]="sortedWorkload" matSort (matSortChange)="sortWorkload($event)">
                <ng-container matColumnDef="developerName">
                  <th mat-header-cell *matHeaderCellDef mat-sort-header>Developer</th>
                  <td mat-cell *matCellDef="let row">{{ row.developerName }}</td>
                </ng-container>
                <ng-container matColumnDef="openTasksCount">
                  <th mat-header-cell *matHeaderCellDef mat-sort-header>Open Tasks</th>
                  <td mat-cell *matCellDef="let row">{{ row.openTasksCount }}</td>
                </ng-container>
                <ng-container matColumnDef="averageEstimatedComplexity">
                  <th mat-header-cell *matHeaderCellDef mat-sort-header>Avg Complexity</th>
                  <td mat-cell *matCellDef="let row">{{ row.averageEstimatedComplexity | number:'1.1-1' }}</td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="workloadColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: workloadColumns;"></tr>
              </table>
            }
          </mat-card-content>
        </mat-card>

        <!-- Project Health Table -->
        <mat-card class="dashboard-card">
          <mat-card-header>
            <mat-card-title>Project Health</mat-card-title>
            <mat-card-subtitle>Task status per project</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            @if (loadingProjects) {
              <div class="loading-container">
                <mat-spinner diameter="40"></mat-spinner>
              </div>
            } @else {
              <table mat-table [dataSource]="projects">
                <ng-container matColumnDef="name">
                  <th mat-header-cell *matHeaderCellDef>Project</th>
                  <td mat-cell *matCellDef="let row">
                    <a class="project-link" (click)="goToProject(row.projectId)">{{ row.name }}</a>
                  </td>
                </ng-container>
                <ng-container matColumnDef="clientName">
                  <th mat-header-cell *matHeaderCellDef>Client</th>
                  <td mat-cell *matCellDef="let row">{{ row.clientName }}</td>
                </ng-container>
                <ng-container matColumnDef="totalTasks">
                  <th mat-header-cell *matHeaderCellDef>Total</th>
                  <td mat-cell *matCellDef="let row">{{ row.totalTasks }}</td>
                </ng-container>
                <ng-container matColumnDef="openTasks">
                  <th mat-header-cell *matHeaderCellDef>Open</th>
                  <td mat-cell *matCellDef="let row">{{ row.openTasks }}</td>
                </ng-container>
                <ng-container matColumnDef="completedTasks">
                  <th mat-header-cell *matHeaderCellDef>Completed</th>
                  <td mat-cell *matCellDef="let row">{{ row.completedTasks }}</td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="projectColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: projectColumns;"
                    [class.warning-row]="row.openTasks > row.completedTasks"></tr>
              </table>
            }
          </mat-card-content>
        </mat-card>

        <!-- Developer Delay Risk Table -->
        <mat-card class="dashboard-card full-width">
          <mat-card-header>
            <mat-card-title>Developer Delay Risk</mat-card-title>
            <mat-card-subtitle>Prediction based on historical data</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            @if (loadingRisk) {
              <div class="loading-container">
                <mat-spinner diameter="40"></mat-spinner>
              </div>
            } @else {
              <table mat-table [dataSource]="delayRisk">
                <ng-container matColumnDef="developerName">
                  <th mat-header-cell *matHeaderCellDef>Developer</th>
                  <td mat-cell *matCellDef="let row">{{ row.developerName }}</td>
                </ng-container>
                <ng-container matColumnDef="openTasksCount">
                  <th mat-header-cell *matHeaderCellDef>Open Tasks</th>
                  <td mat-cell *matCellDef="let row">{{ row.openTasksCount }}</td>
                </ng-container>
                <ng-container matColumnDef="avgDelayDays">
                  <th mat-header-cell *matHeaderCellDef>Avg Delay (days)</th>
                  <td mat-cell *matCellDef="let row">{{ row.avgDelayDays | number:'1.1-1' }}</td>
                </ng-container>
                <ng-container matColumnDef="nearestDueDate">
                  <th mat-header-cell *matHeaderCellDef>Nearest Due</th>
                  <td mat-cell *matCellDef="let row">{{ row.nearestDueDate | date:'shortDate' }}</td>
                </ng-container>
                <ng-container matColumnDef="latestDueDate">
                  <th mat-header-cell *matHeaderCellDef>Latest Due</th>
                  <td mat-cell *matCellDef="let row">{{ row.latestDueDate | date:'shortDate' }}</td>
                </ng-container>
                <ng-container matColumnDef="predictedCompletionDate">
                  <th mat-header-cell *matHeaderCellDef>Predicted Completion</th>
                  <td mat-cell *matCellDef="let row">{{ row.predictedCompletionDate | date:'shortDate' }}</td>
                </ng-container>
                <ng-container matColumnDef="highRiskFlag">
                  <th mat-header-cell *matHeaderCellDef>Risk</th>
                  <td mat-cell *matCellDef="let row">
                    <span [style]="row.highRiskFlag === 1 ? ('High' | statusBadge) : ('Low' | statusBadge)">
                      {{ row.highRiskFlag === 1 ? 'HIGH' : 'LOW' }}
                    </span>
                  </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="riskColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: riskColumns;"
                    [appHighlightRisk]="row.highRiskFlag === 1"></tr>
              </table>
            }
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 24px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .dashboard-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
    }

    .dashboard-header h1 {
      margin: 0;
      font-size: 28px;
      font-weight: 400;
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 24px;
    }

    .dashboard-card {
      min-height: 300px;
    }

    .full-width {
      grid-column: 1 / -1;
    }

    table {
      width: 100%;
    }

    .loading-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 200px;
    }

    .warning-row {
      background-color: #fff3e0 !important;
    }

    .project-link {
      color: #1976d2;
      cursor: pointer;
      text-decoration: none;
    }

    .project-link:hover {
      text-decoration: underline;
    }

    @media (max-width: 960px) {
      .dashboard-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
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
