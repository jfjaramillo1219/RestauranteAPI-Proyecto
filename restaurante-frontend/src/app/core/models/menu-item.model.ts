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
