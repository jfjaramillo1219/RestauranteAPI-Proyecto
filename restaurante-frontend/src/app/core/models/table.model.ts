export type TableStatus = 'Available' | 'Reserved' | 'Occupied';

export interface RestaurantTable {
  id: number;
  number: number;
  capacity: number;
  status: TableStatus;
  location: string | null;
  createdAt: string;
}
