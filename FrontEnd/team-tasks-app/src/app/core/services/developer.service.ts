import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Developer } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class DeveloperService {
  private endpoint = 'developers';

  constructor(private api: ApiService) {}

  getActiveDevelopers(): Observable<Developer[]> {
    return this.api.get<Developer[]>(this.endpoint);
  }
}
