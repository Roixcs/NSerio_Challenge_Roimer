import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { TaskService } from '../../core/services';
import { TaskDetail, UpdateTaskStatus, TASK_STATUSES, TASK_PRIORITIES } from '../../models';
import { StatusBadgePipe } from '../../shared/pipes/status-badge.pipe';

@Component({
  selector: 'app-task-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatSelectModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    StatusBadgePipe
  ],
  templateUrl: './task-detail-dialog.component.html',
  styleUrl: './task-detail-dialog.component.css'
})
export class TaskDetailDialogComponent implements OnInit {
  task: TaskDetail | null = null;
  loading = true;
  updating = false;

  newStatus: string = '';
  newPriority: string = '';

  statuses = TASK_STATUSES;
  priorities = TASK_PRIORITIES;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { taskId: number },
    private dialogRef: MatDialogRef<TaskDetailDialogComponent>,
    private taskService: TaskService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadTask();
  }

  loadTask(): void {
    this.taskService.getTaskById(this.data.taskId).subscribe({
      next: (task) => {
        this.task = task;
        this.newStatus = task.status;
        this.newPriority = task.priority;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading task:', err);
        this.loading = false;
        this.snackBar.open('Error loading task details', 'Close', { duration: 3000 });
      }
    });
  }

  updateTask(): void {
    if (!this.task) return;

    this.updating = true;
    const update: UpdateTaskStatus = {
      status: this.newStatus as any,
      priority: this.newPriority as any
    };

    this.taskService.updateTaskStatus(this.task.taskId, update).subscribe({
      next: () => {
        this.snackBar.open('Task updated successfully', 'Close', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err) => {
        console.error('Error updating task:', err);
        this.snackBar.open('Error updating task', 'Close', { duration: 3000 });
        this.updating = false;
      }
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
