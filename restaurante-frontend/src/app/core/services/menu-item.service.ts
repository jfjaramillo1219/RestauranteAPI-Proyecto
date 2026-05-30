import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MenuItem, MenuItemCategory } from '../models/menu-item.model';

@Injectable({ providedIn: 'root' })
export class MenuItemService {
  private url = `${environment.apiUrl}/MenuItem`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<MenuItem[]> {
    return this.http.get<MenuItem[]>(this.url);
  }

  getByCategory(category: MenuItemCategory): Observable<MenuItem[]> {
    return this.http.get<MenuItem[]>(`${this.url}/category/${category}`);
  }
}
