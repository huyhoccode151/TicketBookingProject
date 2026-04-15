import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Loader } from '../../../shared/ui/loader/loader';
import { AdminPaymentListItemResponse, AdminPaymentListRequest } from '../models/booking_payment';
import { ToastComponent } from '../../../shared/ui/toast/toast';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { BookingPayments } from '../services/booking-payments';
import { Router } from '@angular/router';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [CommonModule,
    FormsModule,
    PageHeader,
    DataTable,
    TableToolbar,
    FilterSelect,
    Pagination,
    Loader,
    TableActions,
    ConfirmDialog],
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
})
export class Index {
  loading: boolean = true;
  isDialogOpen: boolean = false;
  payments: AdminPaymentListItemResponse[] = [];

  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;

  deletePaymentId!: number;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  filters: AdminPaymentListRequest = {
    searchTemp: '',
    status: '',
    method: '',

    dateFrom: '',
    dateTo: '',

    sortDesc: true,
    page: 1,
    pageSize: 10
  }
  statusOptions = [
    { label: 'Pending', value: 'pending' },
    { label: 'Success', value: 'success' },
    { label: 'Failed', value: 'failed' },
    { label: 'Refunded', value: 'refunded' },
  ];
  methodOptions = [
    { label: 'Momo', value: 'momo' },
    { label: 'ZaloPay', value: 'zalopay' },
    { label: 'VnPay', value: 'vnpay' },
  ];

  getStatusColor(status: number): string {
    switch (status) {
      case 1: return 'text-emerald-600'; 
      case 2: return 'text-amber-600';   
      case 3: return 'text-red-600';    
      default: return 'text-slate-600';
    }
  }

  getStatusDotClass(status: number): string {
    switch (status) {
      case 1: return 'bg-emerald-500';
      case 2: return 'bg-amber-500';
      case 3: return 'bg-red-500';
      default: return 'bg-slate-500';
    }
  }

  private toast = inject(ToastService);
  private paymentService = inject(BookingPayments);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);

  ngOnInit() {
    this.loadPayment();
  }

  loadPayment() {
    this.paymentService.getListPayment(this.filters).subscribe({
      next: (res) => {
        this.payments = res.data.items;
        this.pageSize = res.data.pageSize;
        this.totalCount = res.data.totalCount;
        this.totalPages = res.data.totalPages;
        setTimeout(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }, 1000);
      },
      error: () => {
        this.loading = false;
        this.toast.error("Loading Payments failed!!!");
      }
    });
  }

  onSearch(keyword: string) {
    this.filters.searchTemp = keyword;
    this.filters.page = 1;
    this.loadPayment();
  }

  onPageChange(p: number) {
    this.filters.page = p;
    this.loadPayment();
  }

  onDateChange(): void {
    if (!this.filters.dateFrom && !this.filters.dateTo) {
      this.loadPayment();
      return;
    }
    if (this.filters.dateFrom && this.filters.dateTo) {
      this.loadPayment();
    }
  }

  viewPayment(id: number) {

  }

  deletePayment(id: number) {
    this.deletePaymentId = id;
    this.dialogConfig = {
      title: 'Xóa bản ghi',
      message: 'Bạn có chắc muốn xóa vĩnh viễn bản ghi này không?',
      detail: `Bản ghi thanh toán: ${id}`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    }
    this.isDialogOpen = true;
  }

  onConfirmed(): void {
    this.isDialogOpen = false;
    this.paymentService.deletePayment(this.deletePaymentId).subscribe({
      next: (res) => {
        this.toast.success('Delete Payment', 'Deleted Payment Successfully!!!');
        this.loadPayment();
      },
      error: (err) => {
        console.log('FULL_ERRORS', err);
        console.log('VALIDATION:', err.error?.errors);
        this.toast.error('Delete Payment', 'Delete Failed!!!');
      }
    });
  }

  onCancelled(): void {
    this.isDialogOpen = false;
  }

}
