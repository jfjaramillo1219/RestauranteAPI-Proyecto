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
export class Menu implements OnInit {
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
    const l: Record<string, string> = {
      Appetizer: '🥗 Entrada', MainCourse: '🍽️ Plato fuerte',
      Dessert: '🍮 Postre', Beverage: '🥤 Bebida'
    };
    return l[cat] || cat;
  }
}
