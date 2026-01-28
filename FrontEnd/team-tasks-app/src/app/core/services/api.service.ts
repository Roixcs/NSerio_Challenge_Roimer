import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResult } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * GET request that handles Result<T> response
   */
  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    return this.http.get<ApiResult<T>>(`${this.baseUrl}/${endpoint}`, { params }).pipe(
      map(result => this.handleResult(result)),
      catchError(error => this.handleError(error))
    );
  }

  /**
   * POST request that handles Result<T> response
   */
  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<ApiResult<T>>(`${this.baseUrl}/${endpoint}`, body).pipe(
      map(result => this.handleResult(result)),
      catchError(error => this.handleError(error))
    );
  }

  /**
   * PUT request that handles Result<T> response
   */
  put<T>(endpoint: string, body: any): Observable<T> {
    return this.http.put<ApiResult<T>>(`${this.baseUrl}/${endpoint}`, body).pipe(
      map(result => this.handleResult(result)),
      catchError(error => this.handleError(error))
    );
  }

  /**
   * DELETE request that handles Result<T> response
   */
  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<ApiResult<T>>(`${this.baseUrl}/${endpoint}`).pipe(
      map(result => this.handleResult(result)),
      catchError(error => this.handleError(error))
    );
  }

  /**
   * Extracts data from Result<T> or throws error if not successful
   */
  private handleResult<T>(result: ApiResult<T>): T {
    if (result.isSuccess && result.data !== null) {
      return result.data;
    }
    throw new Error(result.errorMessage || 'An unexpected error occurred');
  }

  /**
   * Handles HTTP errors
   */
  private handleError(error: any): Observable<never> {
    let errorMessage = 'An unexpected error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else if (error.error?.errorMessage) {
      // Server returned Result with error
      errorMessage = error.error.errorMessage;
    } else if (error.error?.message) {
      // Server returned error object
      errorMessage = error.error.message;
    } else if (error.status) {
      // HTTP error
      errorMessage = `Error ${error.status}: ${error.statusText}`;
    }

    return throwError(() => new Error(errorMessage));
  }
}
