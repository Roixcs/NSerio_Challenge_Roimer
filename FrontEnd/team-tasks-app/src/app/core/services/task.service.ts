import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { TaskDetail, CreateTask, UpdateTaskStatus } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private endpoint = 'tasks';

  constructor(private api: ApiService) {}

  getTaskById(taskId: number): Observable<TaskDetail> {
    return this.api.get<TaskDetail>(`${this.endpoint}/${taskId}`);
  }

  createTask(task: CreateTask): Observable<TaskDetail> {
    return this.api.post<TaskDetail>(this.endpoint, task);
  }

  updateTaskStatus(taskId: number, update: UpdateTaskStatus): Observable<TaskDetail> {
    return this.api.put<TaskDetail>(`${this.endpoint}/${taskId}/status`, update);
  }
}
