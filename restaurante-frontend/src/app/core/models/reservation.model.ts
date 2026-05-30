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
