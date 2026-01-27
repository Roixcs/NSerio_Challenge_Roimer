import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { TaskService, ProjectService, DeveloperService } from '../../core/services';
import { Project, Developer, CreateTask, TASK_STATUSES, TASK_PRIORITIES } from '../../models';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css'
})
export class TaskFormComponent implements OnInit {
  taskForm: FormGroup;
  projects: Project[] = [];
  developers: Developer[] = [];

  statuses = TASK_STATUSES;
  priorities = TASK_PRIORITIES;

  submitting = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private taskService: TaskService,
    private projectService: ProjectService,
    private developerService: DeveloperService,
    private snackBar: MatSnackBar
  ) {
    this.taskForm = this.fb.group({
      projectId: [null, Validators.required],
      title: ['', [Validators.required, Validators.maxLength(300)]],
      description: [''],
      assigneeId: [null],
      status: ['ToDo', Validators.required],
      priority: ['Medium', Validators.required],
      estimatedComplexity: [3, Validators.required],
      dueDate: [null]
    });
  }

  ngOnInit(): void {
    this.loadProjects();
    this.loadDevelopers();

    // Pre-select project if passed in query params
    const projectId = this.route.snapshot.queryParamMap.get('projectId');
    if (projectId) {
      this.taskForm.patchValue({ projectId: Number(projectId) });
    }
  }

  loadProjects(): void {
    this.projectService.getProjects().subscribe({
      next: (data) => this.projects = data,
      error: (err) => console.error('Error loading projects:', err)
    });
  }

  loadDevelopers(): void {
    this.developerService.getActiveDevelopers().subscribe({
      next: (data) => this.developers = data,
      error: (err) => console.error('Error loading developers:', err)
    });
  }

  onSubmit(): void {
    if (this.taskForm.invalid) return;

    this.submitting = true;
    this.errorMessage = '';

    const formValue = this.taskForm.value;
    const task: CreateTask = {
      projectId: formValue.projectId,
      title: formValue.title.trim(),
      description: formValue.description?.trim() || null,
      assigneeId: formValue.assigneeId,
      status: formValue.status,
      priority: formValue.priority,
      estimatedComplexity: formValue.estimatedComplexity,
      dueDate: formValue.dueDate ? new Date(formValue.dueDate).toISOString() : null
    };

    this.taskService.createTask(task).subscribe({
      next: (created) => {
        this.snackBar.open('Task created successfully!', 'Close', { duration: 3000 });
        this.router.navigate(['/projects', created.projectId]);
      },
      error: (err) => {
        console.error('Error creating task:', err);
        this.errorMessage = err.error?.message || err.error?.errors?.join(', ') || 'Error creating task';
        this.submitting = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }
}
