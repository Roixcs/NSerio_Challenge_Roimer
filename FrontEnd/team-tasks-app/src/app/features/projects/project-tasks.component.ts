import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { NgxChartsModule } from '@swimlane/ngx-charts';

import { ProjectService, DeveloperService } from '../../core/services';
import { Task, Developer, PaginatedResult, TASK_STATUSES } from '../../models';
import { StatusBadgePipe } from '../../shared/pipes/status-badge.pipe';
import { TaskDetailDialogComponent } from '../tasks/task-detail-dialog.component';

@Component({
  selector: 'app-project-tasks',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatPaginatorModule,
    MatSelectModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    NgxChartsModule,
    StatusBadgePipe
  ],
  template: `
    <div class="project-tasks-container">
      <header class="page-header">
        <button mat-icon-button (click)="goBack()">
          <mat-icon>arrow_back</mat-icon>
        </button>
        <h1>Project Tasks</h1>
        <button mat-raised-button color="primary" (click)="openNewTask()">
          <mat-icon>add</mat-icon>
          New Task
        </button>
      </header>

      <div class="content-grid">
        <!-- Filters -->
        <mat-card class="filters-card">
          <mat-card-content>
            <div class="filters-row">
              <mat-form-field appearance="outline">
                <mat-label>Status</mat-label>
                <mat-select [(ngModel)]="selectedStatus" (selectionChange)="applyFilters()">
                  <mat-option [value]="null">All</mat-option>
                  @for (status of statuses; track status) {
                    <mat-option [value]="status">{{ status }}</mat-option>
                  }
                </mat-select>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Developer</mat-label>
                <mat-select [(ngModel)]="selectedDeveloper" (selectionChange)="applyFilters()">
                  <mat-option [value]="null">All</mat-option>
                  @for (dev of developers; track dev.developerId) {
                    <mat-option [value]="dev.developerId">{{ dev.fullName }}</mat-option>
                  }
                </mat-select>
              </mat-form-field>
            </div>
          </mat-card-content>
        </mat-card>

        <!-- Tasks Table -->
        <mat-card class="tasks-card">
          <mat-card-content>
            @if (loading) {
              <div class="loading-container">
                <mat-spinner diameter="40"></mat-spinner>
              </div>
            } @else {
              <table mat-table [dataSource]="tasks">
                <ng-container matColumnDef="title">
                  <th mat-header-cell *matHeaderCellDef>Title</th>
                  <td mat-cell *matCellDef="let row">{{ row.title }}</td>
                </ng-container>
                <ng-container matColumnDef="assigneeName">
                  <th mat-header-cell *matHeaderCellDef>Assigned To</th>
                  <td mat-cell *matCellDef="let row">{{ row.assigneeName || 'Unassigned' }}</td>
                </ng-container>
                <ng-container matColumnDef="status">
                  <th mat-header-cell *matHeaderCellDef>Status</th>
                  <td mat-cell *matCellDef="let row">
                    <span [style]="row.status | statusBadge">{{ row.status }}</span>
                  </td>
                </ng-container>
                <ng-container matColumnDef="priority">
                  <th mat-header-cell *matHeaderCellDef>Priority</th>
                  <td mat-cell *matCellDef="let row">
                    <span [style]="row.priority | statusBadge">{{ row.priority }}</span>
                  </td>
                </ng-container>
                <ng-container matColumnDef="estimatedComplexity">
                  <th mat-header-cell *matHeaderCellDef>Complexity</th>
                  <td mat-cell *matCellDef="let row">{{ row.estimatedComplexity }}</td>
                </ng-container>
                <ng-container matColumnDef="createdAt">
                  <th mat-header-cell *matHeaderCellDef>Created</th>
                  <td mat-cell *matCellDef="let row">{{ row.createdAt | date:'shortDate' }}</td>
                </ng-container>
                <ng-container matColumnDef="dueDate">
                  <th mat-header-cell *matHeaderCellDef>Due Date</th>
                  <td mat-cell *matCellDef="let row">{{ row.dueDate | date:'shortDate' }}</td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: displayedColumns;"
                    class="clickable-row"
                    (click)="openTaskDetail(row)"></tr>
              </table>

              <mat-paginator
                [length]="totalCount"
                [pageSize]="pageSize"
                [pageIndex]="pageIndex"
                [pageSizeOptions]="[5, 10, 25]"
                (page)="onPageChange($event)"
                showFirstLastButtons>
              </mat-paginator>
            }
          </mat-card-content>
        </mat-card>

        <!-- Chart -->
        <mat-card class="chart-card">
          <mat-card-header>
            <mat-card-title>Tasks by Status</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            @if (!loading && chartData.length > 0) {
              <ngx-charts-pie-chart
                [results]="chartData"
                [view]="[350, 250]"
                [legend]="true"
                [labels]="true"
                [doughnut]="true">
              </ngx-charts-pie-chart>
            }
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .project-tasks-container {
      padding: 24px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .page-header {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 24px;
    }

    .page-header h1 {
      flex: 1;
      margin: 0;
      font-size: 24px;
      font-weight: 400;
    }

    .content-grid {
      display: grid;
      grid-template-columns: 1fr 350px;
      gap: 24px;
    }

    .filters-card {
      grid-column: 1 / -1;
    }

    .filters-row {
      display: flex;
      gap: 16px;
      flex-wrap: wrap;
    }

    .filters-row mat-form-field {
      min-width: 200px;
    }

    .tasks-card {
      min-height: 400px;
    }

    table {
      width: 100%;
    }

    .clickable-row {
      cursor: pointer;
    }

    .clickable-row:hover {
      background-color: #f5f5f5;
    }

    .loading-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 300px;
    }

    .chart-card {
      height: fit-content;
    }

    @media (max-width: 960px) {
      .content-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class ProjectTasksComponent implements OnInit {
  projectId!: number;
  tasks: Task[] = [];
  developers: Developer[] = [];

  // Filters
  selectedStatus: string | null = null;
  selectedDeveloper: number | null = null;
  statuses = TASK_STATUSES;

  // Pagination
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;

  // State
  loading = true;

  // Table columns
  displayedColumns = ['title', 'assigneeName', 'status', 'priority', 'estimatedComplexity', 'createdAt', 'dueDate'];

  // Chart data
  chartData: { name: string; value: number }[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService,
    private developerService: DeveloperService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.projectId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDevelopers();
    this.loadTasks();
  }

  loadDevelopers(): void {
    this.developerService.getActiveDevelopers().subscribe({
      next: (data) => this.developers = data,
      error: (err) => console.error('Error loading developers:', err)
    });
  }

  loadTasks(): void {
    this.loading = true;
    this.projectService.getProjectTasks(
      this.projectId,
      this.pageIndex + 1,
      this.pageSize,
      this.selectedStatus || undefined,
      this.selectedDeveloper || undefined
    ).subscribe({
      next: (result: PaginatedResult<Task>) => {
        this.tasks = result.items;
        this.totalCount = result.totalCount;
        this.updateChartData();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading tasks:', err);
        this.loading = false;
      }
    });
  }

  updateChartData(): void {
    const statusCounts = new Map<string, number>();
    TASK_STATUSES.forEach(s => statusCounts.set(s, 0));

    this.tasks.forEach(task => {
      const count = statusCounts.get(task.status) || 0;
      statusCounts.set(task.status, count + 1);
    });

    this.chartData = Array.from(statusCounts.entries())
      .filter(([_, value]) => value > 0)
      .map(([name, value]) => ({ name, value }));
  }

  applyFilters(): void {
    this.pageIndex = 0;
    this.loadTasks();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadTasks();
  }

  openTaskDetail(task: Task): void {
    this.dialog.open(TaskDetailDialogComponent, {
      width: '500px',
      data: { taskId: task.taskId }
    });
  }

  openNewTask(): void {
    this.router.navigate(['/tasks', 'new'], { queryParams: { projectId: this.projectId } });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }
}
