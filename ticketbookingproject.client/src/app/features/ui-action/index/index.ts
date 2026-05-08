import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { Loader } from '../../../shared/ui/loader/loader';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { NgSelectModule } from '@ng-select/ng-select';
import { HasPermissionDirective } from '../../../shared/directives/has-permission-directive';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { ListUIActionRequest, UIActionRequest, UIActionResponse } from '../models/ui-action';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { UIActionService } from '../services/ui-action';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PageHeader,
    DataTable,
    TableToolbar,
    Pagination,
    Loader,
    TableActions,
    NgSelectModule,
    HasPermissionDirective,
    ConfirmDialog,
    FilterSelect
  ],
  templateUrl: './index.html',
  styleUrl: './index.scss',
})
export class Index {
  loading = true;
  isDialogOpen = false;
  isCreateDialogOpen = false;
  isEditDialogOpen = false;
  errorMessage = '';

  uiActions: UIActionResponse[] = [];
  totalCount = 0;
  totalPages = 1;

  selectedId!: number;
  deleteId!: number;

  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  createDialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  editDialogConfig: ConfirmDialogConfig = { title: '', message: '' };

  createForm!: FormGroup;
  editForm!: FormGroup;

  filters: ListUIActionRequest = {
    keyword: '',
    actionType: null,
    isActive: null,
    page: 1,
    pageSize: 10,
  };
  keyword: string | null = '';
  actionType: string = '';
  isActive: boolean | null = null;
  page: number = 1;
  pageSize: number = 10;

  actionTypeOptions = [
    { label: 'Nav', value: 'nav' },
    { label: 'Button', value: 'button' },
  ];

  //selectedActionType: string | null = null;

  private toast = inject(ToastService);
  private service = inject(UIActionService);
  private cdr = inject(ChangeDetectorRef);
  private fb = inject(FormBuilder);

  ngOnInit() {
    this.buildCreateForm();
    this.buildEditForm();
    this.loadList();
  }

  // ── Load ──────────────────────────────────────────────
  loadList() {
    //this.loading = true;
    this.filters.keyword = this.keyword;
    this.filters.actionType = this.actionType;
    this.filters.isActive = this.isActive;
    this.filters.page = this.page;
    this.filters.pageSize = this.pageSize;

    this.service.getList(this.filters).subscribe({
      next: (res) => {
        console.log(this.filters);
        if (res.success) {
          this.uiActions = res.data.items;
          this.totalCount = res.data.totalCount;
          this.totalPages = res.data.totalPages;
          this.filters.pageSize = res.data.pageSize;
        }
        setTimeout(() => { this.loading = false; this.cdr.detectChanges(); }, 300);
      },
      error: (err) => {
        this.loading = false;
        this.toast.error('Load failed', err?.error?.message || 'Cannot load UI actions.');
      }
    });
  }

  // ── Filters ───────────────────────────────────────────
  onSearch(keyword: string) {
    this.keyword = keyword;
    this.page = 1;
    this.loadList();
  }

  onActionTypeChange(value: string) {
    this.actionType = value;
    this.page = 1;
    this.loadList();
  }

  onPageChange(p: number) {
    this.page = p;
    this.loadList();
  }

  // ── Forms ─────────────────────────────────────────────
  private buildCreateForm() {
    this.createForm = this.fb.group({
      actionKey: ['', Validators.required],
      label: ['', Validators.required],
      icon: [''],
      routePath: [''],
      permissionRequired: [''],
      actionType: ['nav', Validators.required],
      parentId: [null],
      displayOrder: [0],
      isActive: [true],
    });
  }

  private buildEditForm() {
    this.editForm = this.fb.group({
      actionKey: ['', Validators.required],
      label: ['', Validators.required],
      icon: [''],
      routePath: [''],
      permissionRequired: [''],
      actionType: ['nav', Validators.required],
      parentId: [null],
      displayOrder: [0],
      isActive: [true],
    });
  }

  // ── Create ────────────────────────────────────────────
  openCreate() {
    this.buildCreateForm();
    this.createDialogConfig = {
      title: 'Create UI Action',
      message: 'Fill in the details to create a new UI action.',
      confirmText: 'Create',
      cancelText: 'Cancel',
      variant: 'success',
    };
    this.isCreateDialogOpen = true;
  }

  onCreateConfirmed() {
    if (this.createForm.invalid) { this.createForm.markAllAsTouched(); return; }

    const payload: UIActionRequest = this.createForm.value;
    this.service.create(payload).subscribe({
      next: () => {
        this.toast.success('Create UI Action', 'Created successfully!');
        this.isCreateDialogOpen = false;
        this.page = 1;
        this.loadList();
      },
      error: (err) => {
        this.handleFormError(err, this.createForm);
        this.toast.error('Create failed', err?.error?.message || 'Please check your input.');
      }
    });
  }

  onCreateCancelled() {
    this.isCreateDialogOpen = false;
  }

  // ── Edit ──────────────────────────────────────────────
  openEdit(id: number) {
    this.selectedId = id;
    this.editDialogConfig = {
      title: 'Edit UI Action',
      message: 'Update the details of this UI action.',
      confirmText: 'Save',
      cancelText: 'Cancel',
      variant: 'info',
    };

    this.service.getById(id).subscribe({
      next: (res) => {
        console.log(res);
        this.editForm.patchValue(res.data);
        this.isEditDialogOpen = true;
        this.cdr.markForCheck();
      },
      error: () => this.toast.error('Load failed', 'Cannot load UI action details.')
    });
  }

  onEditConfirmed() {
    if (this.editForm.invalid) { this.editForm.markAllAsTouched(); return; }

    const payload: UIActionRequest = this.editForm.value;
    this.service.update(this.selectedId, payload).subscribe({
      next: () => {
        this.toast.success('Update UI Action', 'Updated successfully!');
        this.isEditDialogOpen = false;
        this.loadList();
      },
      error: (err) => {
        this.handleFormError(err, this.editForm);
        this.toast.error('Update failed', err?.error?.message || 'Please check your input.');
      }
    });
  }

  onEditCancelled() {
    this.isEditDialogOpen = false;
  }

  // ── Delete ────────────────────────────────────────────
  openDelete(id: number) {
    this.deleteId = id;
    this.dialogConfig = {
      title: 'Delete UI Action',
      message: 'Are you sure you want to permanently delete this UI action?',
      detail: `UI Action ID: ${id}`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    };
    this.isDialogOpen = true;
  }

  onConfirmed() {
    this.isDialogOpen = false;
    this.service.delete(this.deleteId).subscribe({
      next: (res) => {
          this.toast.success('Delete UI Action', 'Deleted successfully!');
          this.page = 1;
          this.loadList();
      },
      error: (err) => this.toast.error('Delete failed', err?.error?.message || 'Unexpected error.')
    });
  }

  onCancelled() {
    this.isDialogOpen = false;
  }

  // ── Helpers ───────────────────────────────────────────
  isInvalid(form: FormGroup, field: string): boolean {
    const c = form.get(field);
    return !!(c && c.invalid && (c.dirty || c.touched));
  }

  private handleFormError(err: any, form: FormGroup) {
    if (err.status === 400 && err.error?.errors) {
      for (const field in err.error.errors) {
        const control = form.get(field.toLowerCase());
        if (control) control.setErrors({ serverError: err.error.errors[field][0] });
      }
    }
  }
}
