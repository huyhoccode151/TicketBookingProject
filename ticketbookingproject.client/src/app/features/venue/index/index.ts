import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { VenueListItemResponse } from '../models/venue';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { RouteService } from '../../../core/services/route.service';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Venue } from '../services/venue';
import { Router, RouterModule } from '@angular/router';
import { PermissionService } from '../../../core/services/permission-service';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { StatCard } from '../../../shared/ui/stat-card/stat-card';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { CommonModule } from '@angular/common';
import { Loader } from '../../../shared/ui/loader/loader';
import { HasPermissionDirective } from '../../../shared/directives/has-permission-directive';

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
  styleUrls: ['./index.scss'],
})
export class Index implements OnInit {
  loading: boolean = true;
  venues: VenueListItemResponse[] = [];

  searchTemp = '';
  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;

  province: string = '';

  venueId!: string;
  isDialogOpen = false;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  hasManagePermission: boolean = false;

  provinceOptions = [
    { label: 'Hà Nội', value: 'Hà Nội' },
    { label: 'Hồ Chí Minh', value: 'Hồ Chí Minh' },
    { label: 'Đà Nẵng', value: 'Đà Nẵng' },
    { label: 'Hải Phòng', value: 'Hải Phòng' },
    { label: 'Cần Thơ', value: 'Cần Thơ' },
  ];

  routeNavigate = inject(RouteService);
  private toast = inject(ToastService);
  private venueService = inject(Venue);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private permissionService = inject(PermissionService);

  ngOnInit() {
    this.hasManagePermission = this.permissionService.has('permission:manage');
    this.loadVenues();
  }

  loadVenues() {
    this.loading = true;
    this.venueService
      .getVenues(this.page, this.pageSize, this.searchTemp, this.province)
      .subscribe(res => {
        this.venues = res?.data?.items ?? [];
        this.pageSize = res?.data?.pageSize ?? 10;
        this.totalCount = res?.data?.totalCount ?? 0;
        this.totalPages = res?.data?.totalPages ?? 1;

        setTimeout(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }, 500);
      });
  }

  onSearch(keyword: string) {
    this.searchTemp = keyword;
    this.page = 1;
    this.loadVenues();
  }

  onPageChange(p: number) {
    this.page = p;
    this.loadVenues();
  }

  provinceFilterChange(province: string) {
    this.province = province;
    this.page = 1;
    this.loadVenues();
    this.cdr.detectChanges();
  }

  editVenue(venueId: number) {
    //this.router.navigate(this.routeNavigate.venuesEdit(venueId));
  }

  deleteVenue(venueId: number) {
    this.venueId = venueId.toString();
    this.dialogConfig = {
      title: 'Delete Venue',
      message: 'Are you sure you want to delete this venue?',
      detail: `Venue ID: ${venueId}`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    };
    this.isDialogOpen = true;
  }

  onConfirmed(): void {
    this.isDialogOpen = false;
    this.venueService.deleteVenue(this.venueId).subscribe({
      next: () => {
        this.toast.success('Delete Venue', 'Venue deleted successfully!');
        this.page = 1;
        this.loadVenues();
      },
      error: (err) => {
        console.error('Delete error:', err);
        this.toast.error('Delete Venue', 'Failed to delete venue!');
      }
    });
  }

  onCancelled(): void {
    this.isDialogOpen = false;
  }

  viewVenue(venueId: number) {
    //this.router.navigate(this.routeNavigate.venuesDetail(venueId));
  }

  onAddVenue() {
    this.router.navigate(this.routeNavigate.venuesCreate());
  }
}
