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
export class App {}
