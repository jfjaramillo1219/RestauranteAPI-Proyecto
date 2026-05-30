import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard')
      .then(m => m.Dashboard)
  },
  {
    path: 'reservations',
    loadComponent: () => import('./pages/reservations/reservations')
      .then(m => m.Reservations)
  },
  {
    path: 'reservations/new',
    loadComponent: () => import('./pages/reservation-new/reservation-new')
      .then(m => m.ReservationNew)
  },
  {
    path: 'reservations/:id',
    loadComponent: () => import('./pages/reservation-detail/reservation-detail')
      .then(m => m.ReservationDetail)
  },
  {
    path: 'menu',
    loadComponent: () => import('./pages/menu/menu')
      .then(m => m.Menu)
  }
];
