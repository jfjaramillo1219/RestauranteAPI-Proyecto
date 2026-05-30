import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RestaurantTable } from '../models/table.model';

@Injectable({ providedIn: 'root' })
export class TableService {
  private url = `${environment.apiUrl}/Table`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<RestaurantTable[]> {
    return this.http.get<RestaurantTable[]>(this.url);
  }

  getAvailable(): Observable<RestaurantTable[]> {
    return this.http.get<RestaurantTable[]>(`${this.url}/available`);
  }
}
