import { Component, OnInit, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { FormsModule } from '@angular/forms';
import { ReservationService } from '../../core/services/reservation.service';
import { MenuItemService } from '../../core/services/menu-item.service';
import { ReservationDetail as ReservationDetailModel, ReservationStatus } from '../../core/models/reservation.model';
import { MenuItem } from '../../core/models/menu-item.model';

@Component({
  selector: 'app-reservation-detail',
  standalone: true,
  imports: [
    DatePipe, DecimalPipe, RouterLink, FormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatSelectModule, MatFormFieldModule, MatDividerModule,
    MatSnackBarModule, MatProgressSpinnerModule, MatChipsModule
  ],
  template: `
    @if (loading) {
      <div style="display:flex;justify-content:center;padding:40px;">
        <mat-spinner></mat-spinner>
      </div>
    }

    @if (!loading && reservation) {
      <div style="max-width:700px;margin:0 auto;">
        <div style="display:flex;align-items:center;gap:8px;margin-bottom:24px;">
          <button mat-icon-button routerLink="/reservations">
            <mat-icon>arrow_back</mat-icon>
          </button>
          <h2 style="margin:0;">Reserva #{{ reservation.id }}</h2>
          <span [style.background]="getStatusBg(reservation.status)"
                [style.color]="getStatusColor(reservation.status)"
                style="padding:4px 12px;border-radius:16px;font-size:13px;font-weight:500;margin-left:8px;">
            {{ getStatusLabel(reservation.status) }}
          </span>
        </div>

        <mat-card style="margin-bottom:16px;">
          <mat-card-header><mat-card-title>Información</mat-card-title></mat-card-header>
          <mat-card-content style="padding:16px;">
            <div style="display:grid;grid-template-columns:1fr 1fr;gap:12px;">
              <div><p style="color:#666;margin:0;font-size:13px;">Cliente</p><p style="margin:4px 0 0;font-weight:500;">{{ reservation.customerName }}</p></div>
              <div><p style="color:#666;margin:0;font-size:13px;">Mesa</p><p style="margin:4px 0 0;font-weight:500;">Mesa {{ reservation.tableNumber }} ({{ reservation.tableCapacity }} personas)</p></div>
              <div><p style="color:#666;margin:0;font-size:13px;">Fecha</p><p style="margin:4px 0 0;font-weight:500;">{{ reservation.reservationDate | date:'dd/MM/yyyy HH:mm' }}</p></div>
              <div><p style="color:#666;margin:0;font-size:13px;">Personas</p><p style="margin:4px 0 0;font-weight:500;">{{ reservation.partySize }}</p></div>
              @if (reservation.notes) {
                <div style="grid-column:span 2;"><p style="color:#666;margin:0;font-size:13px;">Notas</p><p style="margin:4px 0 0;">{{ reservation.notes }}</p></div>
              }
            </div>
          </mat-card-content>
        </mat-card>

        <!-- Cambio de estado -->
        @if (reservation.status !== 'Completed' && reservation.status !== 'Cancelled') {
          <mat-card style="margin-bottom:16px;">
            <mat-card-header><mat-card-title>Cambiar estado</mat-card-title></mat-card-header>
            <mat-card-content style="padding:16px;">
              <div style="display:flex;gap:12px;flex-wrap:wrap;">
                @if (reservation.status === 'Pending') {
                  <button mat-raised-button color="primary" (click)="changeStatus('Confirmed')">
                    ✓ Confirmar
                  </button>
                }
                @if (reservation.status === 'Confirmed') {
                  <button mat-raised-button color="accent" (click)="changeStatus('Completed')">
                    ✓ Completar
                  </button>
                }
                <button mat-raised-button color="warn" (click)="changeStatus('Cancelled')">
                  ✗ Cancelar
                </button>
              </div>
            </mat-card-content>
          </mat-card>
        }

        <!-- Items del pedido -->
        <mat-card>
          <mat-card-header>
            <mat-card-title>Pedido</mat-card-title>
          </mat-card-header>
          <mat-card-content style="padding:16px;">
            @if (reservation.items.length === 0) {
              <div style="color:#666;padding:16px 0;">
                No hay ítems en el pedido.
              </div>
            }
            @for (item of reservation.items; track item.id) {
              <div style="display:flex;justify-content:space-between;padding:8px 0;border-bottom:1px solid #eee;">
                <span>{{ item.menuItemName }} × {{ item.quantity }}</span>
                <span style="font-weight:500;">\${{ item.subtotal | number }}</span>
              </div>
            }
            @if (reservation.items.length > 0) {
              <div style="display:flex;justify-content:space-between;padding:12px 0;font-weight:700;font-size:16px;">
                <span>Total</span>
                <span>\${{ reservation.totalAmount | number }}</span>
              </div>
            }

            <!-- Agregar ítem -->
            @if (reservation.status === 'Pending' || reservation.status === 'Confirmed') {
              <div style="margin-top:16px;padding-top:16px;border-top:2px solid #eee;">
                <p style="font-weight:500;margin:0 0 12px;">Agregar ítem al pedido:</p>
                <div style="display:flex;gap:12px;align-items:flex-end;flex-wrap:wrap;">
                  <mat-form-field appearance="outline" style="flex:1;min-width:200px;">
                    <mat-label>Seleccionar ítem</mat-label>
                    <mat-select [(ngModel)]="selectedMenuItemId">
                      @for (m of availableMenuItems; track m.id) {
                        <mat-option [value]="m.id">
                          {{ m.name }} — \${{ m.price | number }}
                        </mat-option>
                      }
                    </mat-select>
                  </mat-form-field>
                  <mat-form-field appearance="outline" style="width:80px;">
                    <mat-label>Cant.</mat-label>
                    <input matInput type="number" [(ngModel)]="selectedQuantity" min="1">
                  </mat-form-field>
                  <button mat-raised-button color="primary"
                    [disabled]="!selectedMenuItemId"
                    (click)="addItem()"
                    style="margin-bottom:22px;">
                    Agregar
                  </button>
                </div>
              </div>
            }
          </mat-card-content>
        </mat-card>
      </div>
    }
  `
})
export class ReservationDetail implements OnInit {
  private snackBar = inject(MatSnackBar);

  loading = true;
  reservation: ReservationDetailModel | null = null;
  availableMenuItems: MenuItem[] = [];
  selectedMenuItemId: number | null = null;
  selectedQuantity = 1;

  constructor(
    private route: ActivatedRoute,
    private reservationService: ReservationService,
    private menuItemService: MenuItemService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadReservation(id);
    this.menuItemService.getAll().subscribe(items => {
      this.availableMenuItems = items.filter(i => i.isAvailable);
    });
  }

  loadReservation(id: number): void {
    this.loading = true;
    this.reservationService.getById(id).subscribe({
      next: (data) => { this.reservation = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  changeStatus(status: ReservationStatus): void {
    if (!this.reservation) return;
    this.reservationService.changeStatus(this.reservation.id, status).subscribe({
      next: () => {
        this.snackBar.open('Estado actualizado', 'Cerrar', { duration: 3000 });
        this.loadReservation(this.reservation!.id);
      },
      error: (err) => {
        this.snackBar.open(err.error?.message || 'Error al cambiar estado', 'Cerrar', { duration: 4000 });
      }
    });
  }

  addItem(): void {
    if (!this.reservation || !this.selectedMenuItemId) return;
    this.reservationService.addItem(this.reservation.id, {
      menuItemId: this.selectedMenuItemId,
      quantity: this.selectedQuantity
    }).subscribe({
      next: () => {
        this.snackBar.open('Ítem agregado', 'Cerrar', { duration: 2000 });
        this.loadReservation(this.reservation!.id);
        this.selectedMenuItemId = null;
        this.selectedQuantity = 1;
      },
      error: (err) => {
        this.snackBar.open(err.error?.message || 'Error al agregar ítem', 'Cerrar', { duration: 4000 });
      }
    });
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
