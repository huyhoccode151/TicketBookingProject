import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { AuditLogService } from '../services/audit-log';
import { AuditLog, AuditLogQuery } from '../models/audit-log';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { CommonModule, DatePipe } from '@angular/common';
import { Pagination } from '../../../shared/ui/pagination/pagination';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [PageHeader, TableToolbar, FilterSelect, DataTable, DatePipe, Pagination, CommonModule],
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
})
export class Index {
  logs: AuditLog[] = [];
  searchTemp = '';
  page = 1;
  pageSize = 10
  totalCount = 0;
  totalPages = 1;
  filter = {
    action: ''
  }

  actionOptions = [
    { label: 'All', value: '' },
    { label: 'Create', value: 'create' },
    { label: 'Update', value: 'update' },
    { label: 'Delete', value: 'delete' },
    { label: 'Login', value: 'login' },
    { label: 'Logout', value: 'logout' },
  ];

  private auditLogs = inject(AuditLogService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.loadLogs();
  }

  loadLogs()
  {
    const query: AuditLogQuery = {
      page: this.page,
      pageSize: this.pageSize,
      search: this.searchTemp,
    };
    this.auditLogs.getLogs(query).subscribe(res => {
      this.logs = res.data.items;
      this.pageSize = res.data.pageSize;
      this.totalCount = res.data.totalCount;
      this.totalPages = res.data.totalPages;
      this.cdr.detectChanges();
    });
    console.log(this.logs);
  }

  onSearch(keyword: string)
  {
    this.searchTemp = keyword;
    this.page = 1;
    this.loadLogs();
  }

  onPageChange(p: number) {
    this.page = p;
    this.loadLogs();
  }

  viewDetail(log: AuditLog) {

  }
}
