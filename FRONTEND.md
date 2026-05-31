# FRONTEND.md — Sistema de Reservas de Restaurante
## Plan de implementación Angular — Frontend
> Coloca este archivo en la raíz de la solución (junto al .sln)
> En Claude Code escribe: "Read FRONTEND.md and execute Phase 0"
> **Nunca saltes una fase. Verifica en el navegador antes de continuar.**

---

## CONTEXTO

**Framework:** Angular 17+ (standalone components)
**UI Library:** Angular Material
**API backend:** http://localhost:5127 (debe estar corriendo)
**Carpeta del proyecto Angular:** `restaurante-frontend/` (dentro del mismo repo)
**Puerto del frontend:** http://localhost:4200

**Enums del backend (retornados como strings):**
- TableStatus: "Available" | "Reserved" | "Occupied"
- ReservationStatus: "Pending" | "Confirmed" | "Cancelled" | "Completed"
- MenuItemCategory: "Appetizer" | "MainCourse" | "Dessert" | "Beverage"

---

## TABLA DE FASES

| # | Fase | Commit |
|---|------|--------|
| 0 | Setup del proyecto Angular | `chore: initialize Angular project with Material` |
| 1 | Modelos + Servicios HTTP | `feat: add TypeScript models and API services` |
| 2 | Layout + Navegación | `feat: add app layout and navigation` |
| 3 | Vista Dashboard | `feat: add dashboard view` |
| 4 | Vista Reservaciones (listado) | `feat: add reservations list view` |
| 5 | Vista Nueva Reserva (formulario) | `feat: add new reservation form` |
| 6 | Vista Detalle de Reserva | `feat: add reservation detail view` |
| 7 | Vista Menú | `feat: add menu view` |
| 8 | Polish + README actualizado | `chore: final polish and update README` |

---

## FASE 0 — SETUP DEL PROYECTO ANGULAR

### Paso 0.1 — Verificar herramientas

```bash
node --version     # debe ser 18+
npm --version
```

Instalar Angular CLI globalmente si no está:
```bash
npm install -g @angular/cli
ng version
```

### Paso 0.2 — Crear el proyecto Angular

Desde la raíz de la solución (`RestauranteAPI/`):
```bash
ng new restaurante-frontend --routing=true --style=scss --standalone=true --skip-git=true
cd restaurante-frontend
```

Responde las preguntas del CLI así:
- CSS preprocessor: SCSS
- Server-Side Rendering: No

### Paso 0.3 — Instalar Angular Material

```bash
ng add @angular/material
```

Cuando pregunte:
- Theme: **Indigo/Pink** (o cualquier tema prebuilt)
- Global typography: **Yes**
- Animations: **Yes**

### Paso 0.4 — Configurar la URL de la API

Edita `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5127/api'
};
```

Crea también `src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'http://localhost:5127/api'
};
```

### Paso 0.5 — Configurar HttpClient en app.config.ts

En `src/app/app.config.ts` asegúrate de tener:
```typescript
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    provideAnimationsAsync()
  ]
};
```

### Paso 0.6 — Estructura de carpetas

Crea esta estructura dentro de `src/app/`:
```
src/app/
├── core/
│   ├── models/
│   └── services/
├── shared/
│   └── components/
└── pages/
    ├── dashboard/
    ├── reservations/
    ├── reservation-new/
    ├── reservation-detail/
    └── menu/
```

Comandos para crear las carpetas:
```bash
mkdir -p src/app/core/models
mkdir -p src/app/core/services
mkdir -p src/app/shared/components
mkdir -p src/app/pages/dashboard
mkdir -p src/app/pages/reservations
mkdir -p src/app/pages/reservation-new
mkdir -p src/app/pages/reservation-detail
mkdir -p src/app/pages/menu
```

### Paso 0.7 — Verificación
```bash
ng serve
```
Debe abrir http://localhost:4200 con la página de bienvenida de Angular.
Detén el servidor con Ctrl+C.

### Commit fase 0
```bash
cd ..
git add restaurante-frontend/
git commit -m "chore: initialize Angular project with Material"
```

---

## FASE 1 — MODELOS + SERVICIOS HTTP

### Modelos TypeScript — src/app/core/models/

**CREATE src/app/core/models/customer.model.ts**
```typescript
export interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  createdAt: string;
}

export interface CreateCustomerDto {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}
```

**CREATE src/app/core/models/table.model.ts**
```typescript
export type TableStatus = 'Available' | 'Reserved' | 'Occupied';

export interface RestaurantTable {
  id: number;
  number: number;
  capacity: number;
  status: TableStatus;
  location: string | null;
  createdAt: string;
}
```

**CREATE src/app/core/models/menu-item.model.ts**
```typescript
export type MenuItemCategory = 'Appetizer' | 'MainCourse' | 'Dessert' | 'Beverage';

export interface MenuItem {
  id: number;
  name: string;
  description: string | null;
  price: number;
  category: MenuItemCategory;
  isAvailable: boolean;
  createdAt: string;
}
```

**CREATE src/app/core/models/reservation.model.ts**
```typescript
export type ReservationStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed';

export interface ReservationMenuItem {
  id: number;
  reservationId: number;
  menuItemId: number;
  menuItemName: string;
  price: number;
  quantity: number;
  subtotal: number;
}

export interface Reservation {
  id: number;
  customerId: number;
  customerName: string;
  tableId: number;
  tableNumber: number;
  tableCapacity: number;
  reservationDate: string;
  partySize: number;
  status: ReservationStatus;
  notes: string | null;
  createdAt: string;
}

export interface ReservationDetail extends Reservation {
  items: ReservationMenuItem[];
  totalAmount: number;
}

export interface CreateReservationDto {
  customerId: number;
  tableId: number;
  reservationDate: string;
  partySize: number;
  notes?: string;
}

export interface AddMenuItemDto {
  menuItemId: number;
  quantity: number;
}
```

### Servicios HTTP — src/app/core/services/

**CREATE src/app/core/services/customer.service.ts**
```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Customer } from '../models/customer.model';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private url = `${environment.apiUrl}/Customer`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.url);
  }
}
```

**CREATE src/app/core/services/table.service.ts**
```typescript
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
```

**CREATE src/app/core/services/menu-item.service.ts**
```typescript
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
```

**CREATE src/app/core/services/reservation.service.ts**
```typescript
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
```

### Verificación
```bash
ng build
```
Debe compilar sin errores. Si hay errores de importación, corrígelos antes de continuar.

### Commit fase 1
```bash
cd ..
git add restaurante-frontend/
git commit -m "feat: add TypeScript models and API services"
```

---

## FASE 2 — LAYOUT + NAVEGACIÓN

### Paso 2.1 — Configurar rutas en app.routes.ts

```typescript
import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.component')
      .then(m => m.DashboardComponent)
  },
  {
    path: 'reservations',
    loadComponent: () => import('./pages/reservations/reservations.component')
      .then(m => m.ReservationsComponent)
  },
  {
    path: 'reservations/new',
    loadComponent: () => import('./pages/reservation-new/reservation-new.component')
      .then(m => m.ReservationNewComponent)
  },
  {
    path: 'reservations/:id',
    loadComponent: () => import('./pages/reservation-detail/reservation-detail.component')
      .then(m => m.ReservationDetailComponent)
  },
  {
    path: 'menu',
    loadComponent: () => import('./pages/menu/menu.component')
      .then(m => m.MenuComponent)
  }
];
```

### Paso 2.2 — App Component con Sidenav

Reemplaza el contenido de `src/app/app.component.ts` con:
```typescript
import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet, RouterLink, RouterLinkActive,
    MatToolbarModule, MatSidenavModule,
    MatListModule, MatIconModule, MatButtonModule
  ],
  template: `
    <mat-sidenav-container style="height: 100vh;">
      <mat-sidenav mode="side" opened style="width: 220px;">
        <mat-toolbar color="primary">
          <span style="font-size:16px;">🍽️ Restaurante</span>
        </mat-toolbar>
        <mat-nav-list>
          <a mat-list-item routerLink="/dashboard" routerLinkActive="active-link">
            <mat-icon matListItemIcon>dashboard</mat-icon>
            <span matListItemTitle>Dashboard</span>
          </a>
          <a mat-list-item routerLink="/reservations" routerLinkActive="active-link">
            <mat-icon matListItemIcon>event_seat</mat-icon>
            <span matListItemTitle>Reservaciones</span>
          </a>
          <a mat-list-item routerLink="/menu" routerLinkActive="active-link">
            <mat-icon matListItemIcon>restaurant_menu</mat-icon>
            <span matListItemTitle>Menú</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>
      <mat-sidenav-content>
        <mat-toolbar color="primary">
          <span>Sistema de Reservas</span>
        </mat-toolbar>
        <div style="padding: 24px;">
          <router-outlet></router-outlet>
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .active-link { background: rgba(0,0,0,0.1); }
    mat-sidenav { background: #3f51b5; color: white; }
    mat-sidenav mat-icon { color: white; }
    mat-sidenav span { color: white; }
  `]
})
export class AppComponent {}
```

### Paso 2.3 — Crear componentes placeholder para cada página

Genera los 5 componentes:
```bash
cd restaurante-frontend
ng generate component pages/dashboard --standalone
ng generate component pages/reservations --standalone
ng generate component pages/reservation-new --standalone
ng generate component pages/reservation-detail --standalone
ng generate component pages/menu --standalone
```

### Verificación
```bash
ng serve
```
Abre http://localhost:4200 — debe mostrar la barra lateral con los 3 enlaces de navegación.
Verifica que los enlaces navegan entre páginas (aunque vacías por ahora).

### Commit fase 2
```bash
cd ..
git add restaurante-frontend/
git commit -m "feat: add app layout and navigation"
```

---

## FASE 3 — VISTA DASHBOARD

El dashboard muestra 4 tarjetas de resumen usando datos reales de la API.

**REPLACE src/app/pages/dashboard/dashboard.component.ts:**
```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { forkJoin } from 'rxjs';
import { ReservationService } from '../../core/services/reservation.service';
import { TableService } from '../../core/services/table.service';
import { Reservation } from '../../core/models/reservation.model';
import { RestaurantTable } from '../../core/models/table.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, RouterLink,
    MatCardModule, MatButtonModule,
    MatIconModule, MatProgressSpinnerModule
  ],
  template: `
    <h2>Dashboard</h2>

    <div *ngIf="loading" style="display:flex;justify-content:center;padding:40px;">
      <mat-spinner></mat-spinner>
    </div>

    <div *ngIf="!loading">
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
              <tr *ngFor="let r of recentReservations" style="border-bottom:1px solid #eee;">
                <td style="padding:8px;">{{ r.customerName }}</td>
                <td style="padding:8px;">Mesa {{ r.tableNumber }}</td>
                <td style="padding:8px;">{{ r.reservationDate | date:'dd/MM/yyyy' }}</td>
                <td style="padding:8px;">
                  <span [style.color]="getStatusColor(r.status)">
                    {{ getStatusLabel(r.status) }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </mat-card-content>
        <mat-card-actions>
          <button mat-button color="primary" routerLink="/reservations">Ver todas</button>
          <button mat-raised-button color="primary" routerLink="/reservations/new">+ Nueva reserva</button>
        </mat-card-actions>
      </mat-card>
    </div>
  `
})
export class DashboardComponent implements OnInit {
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
```

### Verificación
```bash
ng serve
```
Con el backend corriendo, el dashboard debe mostrar datos reales de la API.

### Commit fase 3
```bash
cd ..
git add restaurante-frontend/
git commit -m "feat: add dashboard view"
```

---

## FASE 4 — VISTA RESERVACIONES (LISTADO)

**REPLACE src/app/pages/reservations/reservations.component.ts:**
```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
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
import { Reservation, ReservationStatus } from '../../core/models/reservation.model';

@Component({
  selector: 'app-reservations',
  standalone: true,
  imports: [
    CommonModule, RouterLink, FormsModule,
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

    <div *ngIf="loading" style="display:flex;justify-content:center;padding:40px;">
      <mat-spinner></mat-spinner>
    </div>

    <div *ngIf="!loading" style="display:grid;gap:12px;">
      <mat-card *ngFor="let r of filteredReservations" style="cursor:pointer;"
        [routerLink]="['/reservations', r.id]">
        <mat-card-content style="padding:16px;">
          <div style="display:flex;justify-content:space-between;align-items:center;">
            <div>
              <h3 style="margin:0 0 4px;">{{ r.customerName }}</h3>
              <p style="margin:0;color:#666;font-size:14px;">
                Mesa {{ r.tableNumber }} · {{ r.partySize }} personas ·
                {{ r.reservationDate | date:'dd/MM/yyyy HH:mm' }}
              </p>
              <p *ngIf="r.notes" style="margin:4px 0 0;font-size:13px;color:#888;">
                📝 {{ r.notes }}
              </p>
            </div>
            <span [style.background]="getStatusBg(r.status)"
                  [style.color]="getStatusColor(r.status)"
                  style="padding:4px 12px;border-radius:16px;font-size:13px;font-weight:500;">
              {{ getStatusLabel(r.status) }}
            </span>
          </div>
        </mat-card-content>
      </mat-card>

      <p *ngIf="filteredReservations.length === 0" style="text-align:center;color:#666;padding:40px;">
        No hay reservaciones para mostrar.
      </p>
    </div>
  `
})
export class ReservationsComponent implements OnInit {
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
    const l: Record<string,string> = { Pending:'Pendiente', Confirmed:'Confirmada', Cancelled:'Cancelada', Completed:'Completada' };
    return l[s] || s;
  }

  getStatusColor(s: string): string {
    const c: Record<string,string> = { Pending:'#e65100', Confirmed:'#1b5e20', Cancelled:'#b71c1c', Completed:'#424242' };
    return c[s] || '#000';
  }

  getStatusBg(s: string): string {
    const b: Record<string,string> = { Pending:'#fff3e0', Confirmed:'#e8f5e9', Cancelled:'#ffebee', Completed:'#f5f5f5' };
    return b[s] || '#eee';
  }
}
```

### Commit fase 4
```bash
cd ..
git add restaurante-frontend/
git commit -m "feat: add reservations list view"
```

---

## FASE 5 — VISTA NUEVA RESERVA (FORMULARIO)

**REPLACE src/app/pages/reservation-new/reservation-new.component.ts:**
```typescript
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
    MatNativeDateModule, MatSnackBarModule, MatProgressSpinnerModule
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
export class ReservationNewComponent implements OnInit {
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
      }
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
```

### Commit fase 5
```bash
cd ..
git add restaurante-frontend/
git commit -m "feat: add new reservation form"
```

---

## FASE 6 — VISTA DETALLE DE RESERVA

**REPLACE src/app/pages/reservation-detail/reservation-detail.component.ts:**
```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
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
import { ReservationDetail, ReservationStatus } from '../../core/models/reservation.model';
import { MenuItem } from '../../core/models/menu-item.model';

@Component({
  selector: 'app-reservation-detail',
  standalone: true,
  imports: [
    CommonModule, RouterLink, FormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatSelectModule, MatFormFieldModule, MatDividerModule,
    MatSnackBarModule, MatProgressSpinnerModule, MatChipsModule
  ],
  template: `
    <div *ngIf="loading" style="display:flex;justify-content:center;padding:40px;">
      <mat-spinner></mat-spinner>
    </div>

    <div *ngIf="!loading && reservation" style="max-width:700px;margin:0 auto;">
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
            <div *ngIf="reservation.notes" style="grid-column:span 2;"><p style="color:#666;margin:0;font-size:13px;">Notas</p><p style="margin:4px 0 0;">{{ reservation.notes }}</p></div>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Cambio de estado -->
      <mat-card style="margin-bottom:16px;" *ngIf="reservation.status !== 'Completed' && reservation.status !== 'Cancelled'">
        <mat-card-header><mat-card-title>Cambiar estado</mat-card-title></mat-card-header>
        <mat-card-content style="padding:16px;">
          <div style="display:flex;gap:12px;flex-wrap:wrap;">
            <button mat-raised-button color="primary"
              *ngIf="reservation.status === 'Pending'"
              (click)="changeStatus('Confirmed')">
              ✓ Confirmar
            </button>
            <button mat-raised-button color="accent"
              *ngIf="reservation.status === 'Confirmed'"
              (click)="changeStatus('Completed')">
              ✓ Completar
            </button>
            <button mat-raised-button color="warn"
              (click)="changeStatus('Cancelled')">
              ✗ Cancelar
            </button>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Items del pedido -->
      <mat-card>
        <mat-card-header>
          <mat-card-title>Pedido</mat-card-title>
        </mat-card-header>
        <mat-card-content style="padding:16px;">
          <div *ngIf="reservation.items.length === 0" style="color:#666;padding:16px 0;">
            No hay ítems en el pedido.
          </div>
          <div *ngFor="let item of reservation.items"
               style="display:flex;justify-content:space-between;padding:8px 0;border-bottom:1px solid #eee;">
            <span>{{ item.menuItemName }} × {{ item.quantity }}</span>
            <span style="font-weight:500;">\${{ item.subtotal | number }}</span>
          </div>
          <div *ngIf="reservation.items.length > 0"
               style="display:flex;justify-content:space-between;padding:12px 0;font-weight:700;font-size:16px;">
            <span>Total</span>
            <span>\${{ reservation.totalAmount | number }}</span>
          </div>

          <!-- Agregar ítem -->
          <div *ngIf="reservation.status === 'Pending' || reservation.status === 'Confirmed'"
               style="margin-top:16px;padding-top:16px;border-top:2px solid #eee;">
            <p style="font-weight:500;margin:0 0 12px;">Agregar ítem al pedido:</p>
            <div style="display:flex;gap:12px;align-items:flex-end;flex-wrap:wrap;">
              <mat-form-field appearance="outline" style="flex:1;min-width:200px;">
                <mat-label>Seleccionar ítem</mat-label>
                <mat-select [(ngModel)]="selectedMenuItemId">
                  <mat-option *ngFor="let m of availableMenuItems" [value]="m.id">
                    {{ m.name }} — \${{ m.price | number }}
                  </mat-option>
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
        </mat-card-content>
      </mat-card>
    </div>
  `
})
export class ReservationDetailComponent implements OnInit {
  loading = true;
  reservation: ReservationDetail | null = null;
  availableMenuItems: MenuItem[] = [];
  selectedMenuItemId: number | null = null;
  selectedQuantity = 1;

  constructor(
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
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
    const l: Record<string,string> = { Pending:'Pendiente', Confirmed:'Confirmada', Cancelled:'Cancelada', Completed:'Completada' };
    return l[s] || s;
  }
  getStatusColor(s: string): string {
    const c: Record<string,string> = { Pending:'#e65100', Confirmed:'#1b5e20', Cancelled:'#b71c1c', Completed:'#424242' };
    return c[s] || '#000';
  }
  getStatusBg(s: string): string {
    const b: Record<string,string> = { Pending:'#fff3e0', Confirmed:'#e8f5e9', Cancelled:'#ffebee', Completed:'#f5f5f5' };
    return b[s] || '#eee';
  }
}
```

### Commit fase 6
```bash
cd ..
git add restaurante-frontend/
git commit -m "feat: add reservation detail view"
```

---

## FASE 7 — VISTA MENÚ

**REPLACE src/app/pages/menu/menu.component.ts:**
```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { FormsModule } from '@angular/forms';
import { MenuItemService } from '../../core/services/menu-item.service';
import { MenuItem, MenuItemCategory } from '../../core/models/menu-item.model';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatButtonToggleModule,
    MatIconModule, MatProgressSpinnerModule, MatChipsModule
  ],
  template: `
    <h2>Menú del Restaurante</h2>

    <mat-button-toggle-group [(ngModel)]="selectedCategory"
      (ngModelChange)="filterItems()"
      style="margin-bottom:24px;flex-wrap:wrap;">
      <mat-button-toggle value="">Todos</mat-button-toggle>
      <mat-button-toggle value="Appetizer">🥗 Entradas</mat-button-toggle>
      <mat-button-toggle value="MainCourse">🍽️ Platos fuertes</mat-button-toggle>
      <mat-button-toggle value="Dessert">🍮 Postres</mat-button-toggle>
      <mat-button-toggle value="Beverage">🥤 Bebidas</mat-button-toggle>
    </mat-button-toggle-group>

    <div *ngIf="loading" style="display:flex;justify-content:center;padding:40px;">
      <mat-spinner></mat-spinner>
    </div>

    <div *ngIf="!loading"
         style="display:grid;grid-template-columns:repeat(auto-fill,minmax(260px,1fr));gap:16px;">
      <mat-card *ngFor="let item of filteredItems">
        <mat-card-header>
          <mat-card-title>{{ item.name }}</mat-card-title>
          <mat-card-subtitle>{{ getCategoryLabel(item.category) }}</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content style="padding:0 16px 8px;">
          <p *ngIf="item.description" style="color:#666;font-size:14px;margin:8px 0;">
            {{ item.description }}
          </p>
          <div style="display:flex;justify-content:space-between;align-items:center;margin-top:12px;">
            <span style="font-size:1.3rem;font-weight:700;color:#3f51b5;">
              \${{ item.price | number }}
            </span>
            <span [style.background]="item.isAvailable ? '#e8f5e9' : '#ffebee'"
                  [style.color]="item.isAvailable ? '#1b5e20' : '#b71c1c'"
                  style="padding:3px 10px;border-radius:12px;font-size:12px;">
              {{ item.isAvailable ? 'Disponible' : 'No disponible' }}
            </span>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <p *ngIf="!loading && filteredItems.length === 0"
       style="text-align:center;color:#666;padding:40px;">
      No hay ítems en esta categoría.
    </p>
  `
})
export class MenuComponent implements OnInit {
  loading = true;
  items: MenuItem[] = [];
  filteredItems: MenuItem[] = [];
  selectedCategory = '';

  constructor(private menuItemService: MenuItemService) {}

  ngOnInit(): void {
    this.menuItemService.getAll().subscribe({
      next: (data) => {
        this.items = data;
        this.filteredItems = data;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  filterItems(): void {
    this.filteredItems = this.selectedCategory
      ? this.items.filter(i => i.category === this.selectedCategory)
      : this.items;
  }

  getCategoryLabel(cat: string): string {
    const l: Record<string,string> = {
      Appetizer:'🥗 Entrada', MainCourse:'🍽️ Plato fuerte',
      Dessert:'🍮 Postre', Beverage:'🥤 Bebida'
    };
    return l[cat] || cat;
  }
}
```

### Verificación final
```bash
cd restaurante-frontend
ng serve
```
Verifica que las 5 vistas carguen con datos reales del backend.

### Commit fase 7
```bash
cd ..
git add restaurante-frontend/
git commit -m "feat: add menu view"
```

---

## FASE 8 — POLISH + README ACTUALIZADO

### Paso 8.1 — Actualizar styles.scss

En `restaurante-frontend/src/styles.scss` agrega:
```scss
html, body {
  height: 100%;
  margin: 0;
  font-family: Roboto, "Helvetica Neue", sans-serif;
}

h2 {
  color: #3f51b5;
  font-weight: 500;
}

mat-card {
  transition: box-shadow 0.2s;
}

mat-card:hover {
  box-shadow: 0 4px 12px rgba(0,0,0,0.15) !important;
}
```

### Paso 8.2 — Actualizar README.md en la raíz

Agrega una sección Frontend al README.md existente:

```markdown
## Frontend (Angular)

### Tecnologías
- Angular 17+ (Standalone Components)
- Angular Material
- TypeScript

### Instrucciones de ejecución del frontend

**Prerrequisitos:** Node.js 18+ y Angular CLI (`npm install -g @angular/cli`)

1. Asegúrate de que el backend esté corriendo en `http://localhost:5127`
2. En una terminal separada:
```bash
cd restaurante-frontend
npm install
ng serve
```
3. Abre `http://localhost:4200`

### Vistas disponibles
- `/dashboard` — Resumen con métricas en tiempo real
- `/reservations` — Listado de reservaciones con filtro por estado
- `/reservations/new` — Formulario para crear nueva reserva
- `/reservations/:id` — Detalle, cambio de estado y gestión del pedido
- `/menu` — Catálogo del menú por categoría
```

### Paso 8.3 — Verificación final completa

Con backend corriendo (`dotnet run`) y frontend corriendo (`ng serve`):

```
[ ] Dashboard carga con datos reales (mesas, conteos de reservas)
[ ] Reservaciones muestra el listado con filtro funcional
[ ] Nueva reserva: dropdown de clientes y mesas disponibles carga
[ ] Nueva reserva: formulario crea reserva y redirige al detalle
[ ] Detalle: muestra información completa de la reserva
[ ] Detalle: botón Confirmar cambia estado a Confirmed
[ ] Detalle: agregar ítem actualiza el pedido y el total
[ ] Detalle: botón Completar libera la mesa
[ ] Menú: muestra 13 ítems agrupables por categoría
[ ] Navegación: los 3 enlaces del sidenav funcionan
```

### Commit fase 8
```bash
cd ..
git add .
git commit -m "chore: final polish and update README"
git push origin master
```

---

## RESUMEN DE COMMITS DEL FRONTEND

```
chore: initialize Angular project with Material
feat: add TypeScript models and API services
feat: add app layout and navigation
feat: add dashboard view
feat: add reservations list view
feat: add new reservation form
feat: add reservation detail view
feat: add menu view
chore: final polish and update README
```

---

## NOTAS TÉCNICAS IMPORTANTES

**Puerto del backend:** Si el backend corre en un puerto diferente a 5127,
actualiza `src/environments/environment.ts` con el puerto correcto.

**CORS:** Ya configurado en el backend con `AllowAll`.

**Enums como strings:** El backend retorna `"Confirmed"` no `1`.
Los modelos TypeScript usan `type` strings, no enums numéricos.

**PATCH /status:** El servicio envía el valor como JSON string con comillas:
`JSON.stringify(status)` con `Content-Type: application/json`.
Sin esto el backend retorna 400.

---

*FRONTEND.md — Sistema de Reservas de Restaurante — ITM 2026*
*Framework: Angular 17+ con Angular Material*
