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
    ConfirmDialog
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
  filter = {
    role: '',
    status: '',
    loginType: '',
  };
  userId!: string;
  isDialogOpen = false;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };

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
    { label: 'Admin', value: 'admin' },
    { label: 'Customer', value: 'customer' },
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

  private toast = inject(ToastService);
  private userService = inject(UserService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);

  ngOnInit() {
    this.getStatUsers();
    this.loadUser();
  }

  loadUser() {
    this.userService.getUser(this.page, this.pageSize, this.searchTemp, this.filter.role, this.filter.status, this.filter.loginType)
      .subscribe(res => {
        this.users = res.data.items;
        this.pageSize = res.data.pageSize;
        this.totalCount = res.data.totalCount;
        this.totalPages = res.data.totalPages;
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
    this.router.navigate(['/users/edit', userId]);
  }

  deleteUser(userId: number) {
    this.userId = userId.toString();
    this.dialogConfig = {
      title: 'Xóa người dùng',
      message: 'Bạn có chắc muốn xóa người dùng này không?',
      detail: `Tài khoản: ${userId}`,
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
}
