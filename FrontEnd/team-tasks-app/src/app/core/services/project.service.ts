import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Project, Task, PaginatedResult } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private endpoint = 'projects';

  constructor(private api: ApiService) {}

  getProjects(): Observable<Project[]> {
    return this.api.get<Project[]>(this.endpoint);
  }

  getProjectTasks(
    projectId: number,
    page: number = 1,
    pageSize: number = 10,
    status?: string,
    assigneeId?: number
  ): Observable<PaginatedResult<Task>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (status) {
      params = params.set('status', status);
    }
    if (assigneeId) {
      params = params.set('assigneeId', assigneeId.toString());
    }

    return this.api.get<PaginatedResult<Task>>(`${this.endpoint}/${projectId}/tasks`, params);
  }
}
