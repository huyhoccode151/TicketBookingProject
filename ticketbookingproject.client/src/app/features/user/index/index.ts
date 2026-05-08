import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { StatCard } from '../../../shared/ui/stat-card/stat-card';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { UserStats, User } from '../models/user';
import { UserService } from '../services/user';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { map, Observable, tap } from 'rxjs';
import { Loader } from '../../../shared/ui/loader/loader';
import { HasPermissionDirective } from '../../../shared/directives/has-permission-directive';
import { PermissionService } from '../../../core/services/permission-service';
import { RouteService } from '../../../core/services/route.service';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [
    Pagination,
    PageHeader,
    StatCard,
    DataTable,
    TableActions,
    TableToolbar,
    FilterSelect,
    CommonModule,
    RouterModule,
    Loader,
    ConfirmDialog,
    HasPermissionDirective
  ],
  templateUrl: './index.html',
  styleUrl: './index.scss',
})
export class Index implements OnInit {
  loading: boolean = true;
  users: User[] = [];
  statUsers: UserStats = {
    totalUsers: { value: 0, change: '0%' },
    newUsers: { value: 0, change: '0%' }
  };;
  searchTemp = '';
  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;
  //filter = {
  role: string = '';
  status: string = '';
  loginType: string = '';
  //};
  userId!: string;
  isDialogOpen = false;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  hasManagePermission: boolean = false;

  //Bagde style
  getRoleClass(role: string): string {
    switch (role.toLowerCase()) {
      case 'admin': return 'px-3 py-1 rounded-full text-xs font-bold bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400';
      case 'staff': return 'px-3 py-1 rounded-full text-xs font-bold bg-slate-100 text-slate-700 dark:bg-slate-800 dark:text-slate-400';
      case 'customer': return 'px-3 py-1 rounded-full text-xs font-bold bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400';
      default: return 'px-3 py-1 rounded-full text-xs font-bold bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400';
    }
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'active': return 'w-1.5 h-1.5 rounded-full bg-primary';
      case 'inactive': return 'w-1.5 h-1.5 rounded-full bg-amber-500';
      case 'banned': return 'w-1.5 h-1.5 rounded-full bg-slate-400';
      default: return 'text-gray-600';
    }
  }

  //filter-options
  roleOptions = [
    { label: 'Organizer', value: 'organizer' },
    { label: 'Staff', value: 'staff' },
    { label: 'Customer', value: 'customer' }
  ];

  statusOptions = [
    { label: 'Active', value: 'active' },
    { label: 'Inactive', value: 'inactive' }
  ];

  loginTypes = [
    { label: 'Email', value: 'email' },
    { label: 'Facebook', value: 'facebook' },
    { label: 'Google', value: 'google' }
  ];

  routeNavigate = inject(RouteService);
  private toast = inject(ToastService);
  private userService = inject(UserService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private permissionService = inject(PermissionService);

  ngOnInit() {
    this.hasManagePermission = this.permissionService.has('permission:manage');
    if (this.hasManagePermission) this.roleOptions.push({ label: 'Admin', value: 'admin' });
    this.getStatUsers();
    this.loadUser();
  }

  loadUser() {
    this.userService.getUser(this.page, this.pageSize, this.searchTemp, this.role, this.status, this.loginType)
      .subscribe(res => {
        console.log('API trả về:', res);
        this.users = res ? res.data.items : [];
        this.pageSize = res ? res.data.pageSize : 10;
        this.totalCount = res ? res.data.totalCount : 0;
        this.totalPages = res ? res.data.totalPages : 1;
    
        setTimeout(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }, 1000);
      });
  }

  onSearch(keyword: string) {
    this.searchTemp = keyword;
    this.page = 1;
    this.loadUser();
  }

  onPageChange(p: number) {
    this.page = p;
    this.loadUser();
  }

  roleFilterChange(role: string) {
    this.role = role; this.page = 1; this.loadUser();
    this.cdr.detectChanges();
  }

  statusFilterChange(status: string) {
    this.status = status; this.page = 1; this.loadUser();
    this.cdr.detectChanges();
  }

  loginTypeFilterChange(loginType: string) {
    this.loginType = loginType; this.page = 1; this.loadUser();
    this.cdr.detectChanges();
  }

  getStatUsers() {
    this.userService.getStatUsers().pipe(
      tap(s => console.log('API trả về:', s)),
      map(s => {
        return {
          totalUsers: {
            value: s.data.totalUsers,
            change: this.calcChange(s.data.totalUsers, s.data.totalUsersLastMonth)
          },
          newUsers: {
            value: s.data.newUsersThisWeek,
            change: this.calcChange(s.data.newUsersThisWeek, s.data.newUsersLastWeek)
          }
        };
      })).subscribe(data => {
        this.statUsers = data;
        this.cdr.detectChanges();
      });;
  }

  calcChange(current: number, previous: number) {
    if (previous === 0) {
      return "+ 100 %";
    }

    const pct = ((current - previous) / previous * 100).toFixed(1);
    return `${+pct > 0 ? '+' : ''}${pct}%`;
  }

  editUser(userId: number) {
    console.log('Edit user:', userId);
    this.router.navigate(this.routeNavigate.usersEdit(userId));
  }

  deleteUser(userId: number) {
    this.userId = userId.toString();
    this.dialogConfig = {
      title: 'Delete User',
      message: 'Are you surely to delete this user?',
      detail: `Account: ${userId}`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    }
    this.isDialogOpen = true;
  }

  onConfirmed(): void {
    this.isDialogOpen = false;
    this.userService.deleteUser(this.userId).subscribe({
      next: (res) => {
        this.toast.success('Delete User', 'Deleted User Successfully!!!');
        this.page = 1;
        this.loadUser();
      },
      error: (err) => {
        console.log('FULL_ERRORS', err);
        console.log('VALIDATION:', err.error?.errors);
        this.toast.error('Delete User', 'Delete Failed!!!');
      }
    });
  }

  onCancelled(): void {
    this.isDialogOpen = false;
  }

  viewUser(userId: number) {
    console.log('View user:', userId);
  }

  onAddUser() {
    this.router.navigate(this.routeNavigate.usersCreate());
  }
}
