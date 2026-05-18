import { CommonModule, NgClass } from '@angular/common';
import { ChangeDetectorRef, Component, HostListener, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { RouteService } from '../../core/services/route.service';
import { Subscription } from 'rxjs';
import { NotificationService } from '../../core/services/notification.service';
import { LayoutService, NotificationDto, NotificationStatus } from '../layout.service';
import { ToastService } from '../../shared/ui/toast/toast.service';

@Component({
  selector: 'app-user-header',
  standalone: true,
  imports: [RouterOutlet, NgClass, CommonModule],
  templateUrl: './user-header.html',
  styleUrls: ['../shell/shell.scss', './user-header.scss'],
})
export class UserHeader {
  isDropdownOpen: boolean = false;
  isLogin: boolean = false;
  notifications: NotificationDto[] = [];
  unreadNotiCount: number = 99;
  isNotifOpen: boolean = false;
  isLoadingNotifs = false;
  unreadCount = 0;

  private notiSub!: Subscription;
  private notificationService = inject(NotificationService);
  private layoutService = inject(LayoutService);
  private authService = inject(AuthService);
  private toast = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private route = inject(RouteService);

  ngOnInit() {
    this.getNotifications();
    const token = localStorage.getItem('access_token');
    if (token) {
      this.isLogin = true;
      this.notificationService.startConnection(token);
      this.notificationService.notificationReceived$.subscribe(msg => {
        alert("new noti " + msg);
      });
    }

    this.cdr.markForCheck();
  }

  ngOnDestroy() {
    // Hủy lắng nghe khi component bị hủy
    if (this.notiSub) {
      this.notiSub.unsubscribe();
    }
    this.notificationService.stopConnection();
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.isNotifOpen = false;
    this.isDropdownOpen = false;
  }

  getNotifications() {
    this.isLoadingNotifs = true;
    this.layoutService.getNotification().subscribe({
      next: (res) => {
        this.notifications = res.data;
        this.countUnreadNoti();
        this.isLoadingNotifs = false;
        this.cdr.markForCheck();
      }
    });
  }

  countUnreadNoti() {
    this.unreadNotiCount = this.notifications.filter(n => n.status == 1).length;
  }

  markAllAsRead(): void {
    this.notifications.forEach(n => n.status = 1);
    this.unreadNotiCount = 0;
    this.layoutService.markAllAsRead().subscribe({
      next: () => {

      },
      error: (err) => {
        this.toast.error("Can't mark all as read");
      }
    });
  }

  toggleDropdown(event: MouseEvent) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  toggleNotifications(event: MouseEvent): void {
    event.stopPropagation();
    this.isNotifOpen = !this.isNotifOpen;
    this.isDropdownOpen = false; // close profile if open

    if (this.isNotifOpen && this.notifications.length === 0) {
      this.getNotifications();
    }
  }

  onNotifClick(notif: NotificationDto): void {
    if (notif.status === 1) {
      notif.status = 3;
      this.unreadNotiCount = Math.max(0, this.unreadNotiCount - 1);
      this.layoutService.markAsRead(notif.id).subscribe();
    }
  }

  goProfile() {
    this.router.navigate(this.route.profile());
  }

  logout() {
    this.authService.logout();
  }
}
