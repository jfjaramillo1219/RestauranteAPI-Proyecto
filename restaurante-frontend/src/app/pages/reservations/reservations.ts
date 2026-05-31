import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ReservationService } from '../../core/services/reservation.service';
import { Reservation } from '../../core/models/reservation.model';

@Component({
  selector: 'app-reservations',
  standalone: true,
  imports: [
    DatePipe, RouterLink, FormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatSelectModule, MatFormFieldModule,
    MatProgressSpinnerModule, MatChipsModule
  ],
  template: `
    <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:24px;">
      <h2 style="margin:0;">Reservaciones</h2>
      <button mat-raised-button color="primary" routerLink="/reservations/new">
        <mat-icon>add</mat-icon> Nueva reserva
      </button>
    </div>

    <mat-form-field appearance="outline" style="width:200px;margin-bottom:16px;">
      <mat-label>Filtrar por estado</mat-label>
      <mat-select [(ngModel)]="selectedStatus" (ngModelChange)="filterReservations()">
        <mat-option value="">Todos</mat-option>
        <mat-option value="Pending">Pendiente</mat-option>
        <mat-option value="Confirmed">Confirmada</mat-option>
        <mat-option value="Completed">Completada</mat-option>
        <mat-option value="Cancelled">Cancelada</mat-option>
      </mat-select>
    </mat-form-field>

    @if (loading) {
      <div style="display:flex;justify-content:center;padding:40px;">
        <mat-spinner></mat-spinner>
      </div>
    }

    @if (!loading) {
      <div style="display:grid;gap:12px;">
        @for (r of filteredReservations; track r.id) {
          <mat-card style="cursor:pointer;" [routerLink]="['/reservations', r.id]">
            <mat-card-content style="padding:16px;">
              <div style="display:flex;justify-content:space-between;align-items:center;">
                <div>
                  <h3 style="margin:0 0 4px;">{{ r.customerName }}</h3>
                  <p style="margin:0;color:#666;font-size:14px;">
                    Mesa {{ r.tableNumber }} · {{ r.partySize }} personas ·
                    {{ r.reservationDate | date:'dd/MM/yyyy HH:mm' }}
                  </p>
                  @if (r.notes) {
                    <p style="margin:4px 0 0;font-size:13px;color:#888;">
                      📝 {{ r.notes }}
                    </p>
                  }
                </div>
                <span [style.background]="getStatusBg(r.status)"
                      [style.color]="getStatusColor(r.status)"
                      style="padding:4px 12px;border-radius:16px;font-size:13px;font-weight:500;">
                  {{ getStatusLabel(r.status) }}
                </span>
              </div>
            </mat-card-content>
          </mat-card>
        }

        @if (filteredReservations.length === 0) {
          <p style="text-align:center;color:#666;padding:40px;">
            No hay reservaciones para mostrar.
          </p>
        }
      </div>
    }
  `
})
export class Reservations implements OnInit {
  loading = true;
  reservations: Reservation[] = [];
  filteredReservations: Reservation[] = [];
  selectedStatus = '';

  constructor(private reservationService: ReservationService) {}

  ngOnInit(): void {
    this.reservationService.getAll().subscribe({
      next: (data) => {
        this.reservations = data;
        this.filteredReservations = data;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  filterReservations(): void {
    this.filteredReservations = this.selectedStatus
      ? this.reservations.filter(r => r.status === this.selectedStatus)
      : this.reservations;
  }

  getStatusLabel(s: string): string {
    const l: Record<string, string> = { Pending: 'Pendiente', Confirmed: 'Confirmada', Cancelled: 'Cancelada', Completed: 'Completada' };
    return l[s] || s;
  }

  getStatusColor(s: string): string {
    const c: Record<string, string> = { Pending: '#e65100', Confirmed: '#1b5e20', Cancelled: '#b71c1c', Completed: '#424242' };
    return c[s] || '#000';
  }

  getStatusBg(s: string): string {
    const b: Record<string, string> = { Pending: '#fff3e0', Confirmed: '#e8f5e9', Cancelled: '#ffebee', Completed: '#f5f5f5' };
    return b[s] || '#eee';
  }
}
