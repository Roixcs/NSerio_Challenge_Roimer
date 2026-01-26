import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { DeveloperWorkload, DeveloperDelayRisk, ProjectHealth } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private endpoint = 'dashboard';

  constructor(private api: ApiService) {}

  getDeveloperWorkload(): Observable<DeveloperWorkload[]> {
    return this.api.get<DeveloperWorkload[]>(`${this.endpoint}/developer-workload`);
  }

  getProjectHealth(): Observable<ProjectHealth[]> {
    return this.api.get<ProjectHealth[]>(`${this.endpoint}/project-health`);
  }

  getDeveloperDelayRisk(): Observable<DeveloperDelayRisk[]> {
    return this.api.get<DeveloperDelayRisk[]>(`${this.endpoint}/developer-delay-risk`);
  }
}
