import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { ReservationService } from '../../core/services/reservation.service';
import { CustomerService } from '../../core/services/customer.service';
import { TableService } from '../../core/services/table.service';
import { Customer } from '../../core/models/customer.model';
import { RestaurantTable } from '../../core/models/table.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-reservation-new',
  standalone: true,
  imports: [
    CommonModule, RouterLink, ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatDatepickerModule,
    MatNativeDateModule, MatSnackBarModule, MatProgressSpinnerModule,
    MatIconModule
  ],
  template: `
    <div style="max-width:600px;margin:0 auto;">
      <div style="display:flex;align-items:center;gap:8px;margin-bottom:24px;">
        <button mat-icon-button routerLink="/reservations">
          <mat-icon>arrow_back</mat-icon>
        </button>
        <h2 style="margin:0;">Nueva Reserva</h2>
      </div>

      <mat-card>
        <mat-card-content style="padding:24px;">
          <div *ngIf="loadingData" style="display:flex;justify-content:center;padding:40px;">
            <mat-spinner></mat-spinner>
          </div>

          <form *ngIf="!loadingData" [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" style="width:100%;margin-bottom:16px;">
              <mat-label>Cliente</mat-label>
              <mat-select formControlName="customerId">
                <mat-option *ngFor="let c of customers" [value]="c.id">
                  {{ c.firstName }} {{ c.lastName }}
                </mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline" style="width:100%;margin-bottom:16px;">
              <mat-label>Mesa disponible</mat-label>
              <mat-select formControlName="tableId">
                <mat-option *ngFor="let t of availableTables" [value]="t.id">
                  Mesa {{ t.number }} — {{ t.capacity }} personas — {{ t.location }}
                </mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline" style="width:100%;margin-bottom:16px;">
              <mat-label>Fecha de reserva</mat-label>
              <input matInput [matDatepicker]="picker" formControlName="reservationDate">
              <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
              <mat-datepicker #picker></mat-datepicker>
            </mat-form-field>

            <mat-form-field appearance="outline" style="width:100%;margin-bottom:16px;">
              <mat-label>Número de personas</mat-label>
              <input matInput type="number" formControlName="partySize" min="1">
            </mat-form-field>

            <mat-form-field appearance="outline" style="width:100%;margin-bottom:24px;">
              <mat-label>Notas (opcional)</mat-label>
              <textarea matInput formControlName="notes" rows="3"></textarea>
            </mat-form-field>

            <div style="display:flex;gap:12px;">
              <button mat-raised-button color="primary" type="submit"
                [disabled]="form.invalid || submitting">
                <mat-spinner *ngIf="submitting" diameter="20" style="display:inline-block;"></mat-spinner>
                {{ submitting ? 'Creando...' : 'Crear reserva' }}
              </button>
              <button mat-button type="button" routerLink="/reservations">Cancelar</button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `
})
export class ReservationNew implements OnInit {
  form!: FormGroup;
  customers: Customer[] = [];
  availableTables: RestaurantTable[] = [];
  loadingData = true;
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private snackBar: MatSnackBar,
    private reservationService: ReservationService,
    private customerService: CustomerService,
    private tableService: TableService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      customerId: [null, Validators.required],
      tableId: [null, Validators.required],
      reservationDate: [null, Validators.required],
      partySize: [1, [Validators.required, Validators.min(1)]],
      notes: ['']
    });

    forkJoin({
      customers: this.customerService.getAll(),
      tables: this.tableService.getAvailable()
    }).subscribe({
      next: ({ customers, tables }) => {
        this.customers = customers;
        this.availableTables = tables;
        this.loadingData = false;
      },
      error: () => { this.loadingData = false; }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting = true;
    const value = this.form.value;
    const dto = {
      ...value,
      reservationDate: new Date(value.reservationDate).toISOString()
    };
    this.reservationService.create(dto).subscribe({
      next: (result) => {
        this.snackBar.open('Reserva creada exitosamente', 'Cerrar', { duration: 3000 });
        this.router.navigate(['/reservations', result.id]);
      },
      error: (err) => {
        const msg = err.error?.message || 'Error al crear la reserva';
        this.snackBar.open(msg, 'Cerrar', { duration: 4000 });
        this.submitting = false;
      }
    });
  }
}
