import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Reservation, ReservationDetail,
  CreateReservationDto, ReservationStatus,
  AddMenuItemDto, ReservationMenuItem
} from '../models/reservation.model';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private url = `${environment.apiUrl}/Reservation`;
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(this.url);
  }

  getById(id: number): Observable<ReservationDetail> {
    return this.http.get<ReservationDetail>(`${this.url}/${id}`);
  }

  getByCustomer(customerId: number): Observable<Reservation[]> {
    return this.http.get<Reservation[]>(`${this.url}/customer/${customerId}`);
  }

  create(dto: CreateReservationDto): Observable<ReservationDetail> {
    return this.http.post<ReservationDetail>(this.url, dto);
  }

  changeStatus(id: number, status: ReservationStatus): Observable<Reservation> {
    return this.http.patch<Reservation>(
      `${this.url}/${id}/status`,
      JSON.stringify(status),
      { headers: { 'Content-Type': 'application/json' } }
    );
  }

  addItem(reservationId: number, dto: AddMenuItemDto): Observable<ReservationMenuItem> {
    return this.http.post<ReservationMenuItem>(
      `${this.baseUrl}/reservation/${reservationId}/items`, dto
    );
  }

  removeItem(reservationId: number, menuItemId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/reservation/${reservationId}/items/${menuItemId}`
    );
  }
}
