import { inject, Injectable, signal } from "@angular/core";
import { environment } from "../../environments/environments";
import { HttpClient } from "@angular/common/http";
import { ApiResponse } from "../features/user/models/paged-result";
export interface NotificationDto {
  id: number;
  userId: number;
  type: NotificationType;
  title?: string | null;
  content: string;
  status: NotificationStatus;
  sentAt: string;
  createdAt: string;
}

export enum NotificationType {
  System = 0,
  Booking = 1,
  Payment = 2
}

export enum NotificationStatus {
  Pending = 0,
  Sent = 1,
  Failed = 2,
  Read = 3
}

@Injectable({ providedIn: 'root' })
export class LayoutService {
  private api = environment.apiUrl + '/Notification';
  private http = inject(HttpClient);

  readonly sidebarCollapsed = signal(false);
  toggleSidebar() { this.sidebarCollapsed.update(v => !v); }
  setSidebarCollapsed(v: boolean) { this.sidebarCollapsed.set(v); }

  getNotification() {
    return this.http.get<ApiResponse<NotificationDto[]>>(this.api);
  }

  markAllAsRead() {
    return this.http.patch<ApiResponse<boolean>>(this.api, { });
  }

  markAsRead(id: number) {
    return this.http.patch<ApiResponse<boolean>>(this.api + "/" + id, { });
  }
}
