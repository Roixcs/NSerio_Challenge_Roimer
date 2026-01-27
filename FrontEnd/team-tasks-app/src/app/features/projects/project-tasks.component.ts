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
import { NgxChartsModule, LegendPosition } from '@swimlane/ngx-charts';

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
  templateUrl: './project-tasks.component.html',
  styleUrl: './project-tasks.component.css'
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

  // Chart
  chartData: { name: string; value: number }[] = [];
  legendPosition = LegendPosition.Right;

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
    const dialogRef = this.dialog.open(TaskDetailDialogComponent, {
      width: '500px',
      data: { taskId: task.taskId }
    });

    dialogRef.afterClosed().subscribe(updated => {
      if (updated) {
        this.loadTasks();
      }
    });
  }

  openNewTask(): void {
    this.router.navigate(['/tasks', 'new'], { queryParams: { projectId: this.projectId } });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }
}
