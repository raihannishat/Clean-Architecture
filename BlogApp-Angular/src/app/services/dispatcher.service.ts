import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

export interface DispatcherRequest {
  operation: string;
  data: string;
}

export interface DispatcherResponse {
  isSuccess: boolean;
  message: string;
  data: any;
  errors: string[];
  statusCode: number;
}

// Type for dynamic methods
export interface DynamicDispatcher {
  [key: string]: (data?: any) => Observable<any>;
}

@Injectable({
  providedIn: 'root'
})
export class DispatcherService {
  private apiUrl = 'https://localhost:7001/api/dispatch';

  constructor(private http: HttpClient) {}

  dispatch<T>(operation: string, data: any = {}): Observable<T> {
    const request: DispatcherRequest = {
      operation: operation,
      data: JSON.stringify(data)
    };

    return this.http.post<DispatcherResponse>(this.apiUrl, request).pipe(
      map((response: DispatcherResponse) => {
        if (response.isSuccess) {
          return response.data as T;
        } else {
          const errorMessage = response.errors && response.errors.length > 0 
            ? response.errors.join(', ') 
            : response.message || 'Unknown error occurred';
          throw new Error(errorMessage);
        }
      }),
      catchError((error: any) => {
        console.error('Dispatcher error:', error);
        return throwError(() => error);
      })
    );
  }

  // Dynamic method generator using Proxy
  private createDynamicMethods(): DynamicDispatcher {
    return new Proxy({} as DynamicDispatcher, {
      get: (target, prop) => {
        if (typeof prop === 'string') {
          return (data: any = {}) => {
            return this.dispatch(prop, data);
          };
        }
        return target[prop as unknown as keyof DynamicDispatcher];
      }
    });
  }

  // Return the proxied instance
  get dynamic(): DynamicDispatcher {
    return this.createDynamicMethods();
  }
} 