import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DatePipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { ReservationService } from '../../core/services/reservation.service';
import { TableService } from '../../core/services/table.service';
import { Reservation } from '../../core/models/reservation.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    RouterLink, DatePipe,
    MatCardModule, MatButtonModule,
    MatIconModule, MatProgressSpinnerModule
  ],
  template: `
    <h2>Dashboard</h2>

    @if (loading) {
      <div style="display:flex;justify-content:center;padding:40px;">
        <mat-spinner></mat-spinner>
      </div>
    }

    @if (!loading) {
      <div>
        <div style="display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:16px;margin-bottom:32px;">

          <mat-card>
            <mat-card-content style="text-align:center;padding:24px 16px;">
              <mat-icon style="font-size:40px;height:40px;width:40px;color:#3f51b5;">event_seat</mat-icon>
              <h1 style="margin:8px 0 4px;font-size:2rem;">{{ availableTables }}</h1>
              <p style="color:#666;margin:0;">Mesas disponibles</p>
            </mat-card-content>
          </mat-card>

          <mat-card>
            <mat-card-content style="text-align:center;padding:24px 16px;">
              <mat-icon style="font-size:40px;height:40px;width:40px;color:#ff9800;">pending</mat-icon>
              <h1 style="margin:8px 0 4px;font-size:2rem;">{{ pendingCount }}</h1>
              <p style="color:#666;margin:0;">Reservas pendientes</p>
            </mat-card-content>
          </mat-card>

          <mat-card>
            <mat-card-content style="text-align:center;padding:24px 16px;">
              <mat-icon style="font-size:40px;height:40px;width:40px;color:#4caf50;">check_circle</mat-icon>
              <h1 style="margin:8px 0 4px;font-size:2rem;">{{ confirmedCount }}</h1>
              <p style="color:#666;margin:0;">Reservas confirmadas</p>
            </mat-card-content>
          </mat-card>

          <mat-card>
            <mat-card-content style="text-align:center;padding:24px 16px;">
              <mat-icon style="font-size:40px;height:40px;width:40px;color:#e91e63;">table_restaurant</mat-icon>
              <h1 style="margin:8px 0 4px;font-size:2rem;">{{ reservedTables }}</h1>
              <p style="color:#666;margin:0;">Mesas reservadas</p>
            </mat-card-content>
          </mat-card>

        </div>

        <mat-card>
          <mat-card-header>
            <mat-card-title>Reservas recientes</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <table style="width:100%;border-collapse:collapse;">
              <thead>
                <tr style="border-bottom:2px solid #eee;">
                  <th style="text-align:left;padding:8px;">Cliente</th>
                  <th style="text-align:left;padding:8px;">Mesa</th>
                  <th style="text-align:left;padding:8px;">Fecha</th>
                  <th style="text-align:left;padding:8px;">Estado</th>
                </tr>
              </thead>
              <tbody>
                @for (r of recentReservations; track r.id) {
                  <tr style="border-bottom:1px solid #eee;">
                    <td style="padding:8px;">{{ r.customerName }}</td>
                    <td style="padding:8px;">Mesa {{ r.tableNumber }}</td>
                    <td style="padding:8px;">{{ r.reservationDate | date:'dd/MM/yyyy' }}</td>
                    <td style="padding:8px;">
                      <span [style.color]="getStatusColor(r.status)">
                        {{ getStatusLabel(r.status) }}
                      </span>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </mat-card-content>
          <mat-card-actions>
            <button mat-button color="primary" routerLink="/reservations">Ver todas</button>
            <button mat-raised-button color="primary" routerLink="/reservations/new">+ Nueva reserva</button>
          </mat-card-actions>
        </mat-card>
      </div>
    }
  `
})
export class Dashboard implements OnInit {
  loading = true;
  availableTables = 0;
  reservedTables = 0;
  pendingCount = 0;
  confirmedCount = 0;
  recentReservations: Reservation[] = [];

  constructor(
    private reservationService: ReservationService,
    private tableService: TableService
  ) {}

  ngOnInit(): void {
    forkJoin({
      reservations: this.reservationService.getAll(),
      tables: this.tableService.getAll()
    }).subscribe({
      next: ({ reservations, tables }) => {
        this.availableTables = tables.filter(t => t.status === 'Available').length;
        this.reservedTables = tables.filter(t => t.status === 'Reserved').length;
        this.pendingCount = reservations.filter(r => r.status === 'Pending').length;
        this.confirmedCount = reservations.filter(r => r.status === 'Confirmed').length;
        this.recentReservations = reservations.slice(0, 5);
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  getStatusColor(status: string): string {
    const colors: Record<string, string> = {
      Pending: '#ff9800', Confirmed: '#4caf50',
      Cancelled: '#f44336', Completed: '#9e9e9e'
    };
    return colors[status] || '#000';
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      Pending: 'Pendiente', Confirmed: 'Confirmada',
      Cancelled: 'Cancelada', Completed: 'Completada'
    };
    return labels[status] || status;
  }
}
