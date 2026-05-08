import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { Loader } from '../../../shared/ui/loader/loader';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { CreateRoleRequest, ListRoleRequest, RoleListResponse } from '../models/role';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Role } from '../services/role';
import { NgSelectModule } from '@ng-select/ng-select';
import { Router } from '@angular/router';
import { HasPermissionDirective } from '../../../shared/directives/has-permission-directive';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    PageHeader,
    DataTable,
    TableToolbar,
    FilterSelect,
    Pagination,
    Loader,
    NgSelectModule,
    TableActions,
    ReactiveFormsModule,
    HasPermissionDirective,
    ConfirmDialog,
  ],
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Index {
  loading: boolean = true;
  isDialogOpen: boolean = false;
  isCreateDialogOpen: boolean = false;
  isEditDialogOpen: boolean = false;
  roles: RoleListResponse[] = [];
  selectedPermissions: string[] = [];
  errorMessage = '';

  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;

  editForm!: FormGroup;
  createForm!: FormGroup;

  selectedRole!: number;
  deleteRoleId!: number;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  createDialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  editDialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  roleGroups: string[] = [];
  editLoading = false;

  filters: ListRoleRequest = {
    keyword: '',
    permissionNames: [],
    page: 1,
    pageSize: 10,
  };

  permissionOptions: { label: string; value: string }[] = [
    // Populate from API or static list as needed
    // { label: 'Manage Users', value: 'manage_users' },
  ];
  permissions: string[] = [];

  private toast = inject(ToastService);
  private roleService = inject(Role);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  ngOnInit() {
    this.loadPermissions();
    this.loadRoles();
    this.editForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      permissions: this.fb.group({})
    });
    this.createForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      permissions: this.fb.group({})
    });
    this.cdr.markForCheck();
  }

  loadPermissions() {
    this.roleService.getListPermissions(this.selectedPermissions).subscribe({
      next: (res) => {
        console.log('Permissions response:', res);
        this.permissions = res.data;
        console.log('Permissions loaded:', this.permissions);
      },
      error: (err) => {
        this.loading = false;
        console.log(err?.error?.errors || err?.message || 'Unknown error');
        this.toast.error('Loading Roles failed!!!', err?.error?.errors || err?.message || 'Unknown error');
      },
    })
  }

  loadRoles() {
    //this.loading = true;
    this.roleService.getRoles(this.filters).subscribe({
      next: (res) => {
        if (res.success) {
          this.roles = res.data.items;
          this.pageSize = res.data.pageSize;
          this.totalCount = res.data.totalCount;
          this.totalPages = res.data.totalPages;
        }
        setTimeout(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }, 500);
      },
      error: (err) => {
        this.loading = false;
        console.log(err?.error?.errors || err?.message || 'Unknown error');
        this.toast.error('Loading Roles failed!!!', err?.error?.errors || err?.message || 'Unknown error');
      },
    });
  }

  onSearch(keyword: string) {
    this.filters.keyword = keyword;
    this.filters.page = 1;
    this.loadRoles();
  }

  onPageChange(p: number) {
    this.filters.page = p;
    this.loadRoles();
  }

  onPermissionFilterChange(value: string) {
    this.filters.permissionNames = value ? [value] : [];
    this.filters.page = 1;
    this.loadRoles();
  }

  private buildEditForm(): void {
    const controls: Record<string, boolean> = {};
    this.roleGroups.forEach(p => controls[p] = false);

    this.editForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      permissions: this.fb.group(controls)
    });
  }

  private buildCreateForm(): void {
    const controls: Record<string, boolean> = {};
    this.roleGroups.forEach(p => (controls[p] = false));

    this.createForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      permissions: this.fb.group(controls)
    });
  }

  private loadPermissionFromRole(id: number): void {
    this.roleService.getRole(id).subscribe({
      next: res => {
        const r = res.data;
        const permGroup = this.editForm.get('permissions') as FormGroup;
        console.log('Loaded role: ', r);

        const permissions = r.permissions ?? [];
        const patch: Record<string, boolean> = {};
        permissions.forEach(p => {
          if (permGroup.contains(p.name)) patch[p.name] = true;
        });

        this.editForm.patchValue({
          name: r.name,
          description: r.description,
          permissions: patch
        });
        this.cdr.markForCheck();
      },
      error: () => {
        this.editLoading = false;
        this.cdr.detectChanges();
        this.toast.error('Load Permissions', 'Failed to load role permissions!!!');
      }
    })
  }

  get groupedPermissions(): { resource: string; permissions: string[] }[] {
    const map = new Map<string, string[]>();
    this.roleGroups.forEach(p => {
      const resource = p.split(':')[0];
      if (!map.has(resource)) map.set(resource, []);
      map.get(resource)!.push(p);
    });
    return Array.from(map.entries()).map(([resource, permissions]) => ({ resource, permissions }));
  }

  viewRole(id: number) {
    // this.router.navigate(['/admin/roles', id]);
  }

  editRole(id: number) {
    // this.router.navigate(['/admin/roles', id, 'edit']);
  }

  openEditRole(id: number) {
    this.selectedRole = id;
    this.editDialogConfig = {
      title: 'Edit Role',
      message: 'Careful!!! you might accident change st in this role?',
      detail: `Role ID: ${id}`,
      confirmText: 'Confirm',
      cancelText: 'Cancel',
      variant: 'info',
    };
    this.isEditDialogOpen = true;
    this.roleService.getListPermissions([]).subscribe({
      next: (groups) => {
        this.roleGroups = groups.data;

        this.buildEditForm();
        this.loadPermissionFromRole(this.selectedRole);
      }
    });
  }

  openCreateRole() {
    this.createDialogConfig = {
      title: 'Create New Role',
      message: 'Fill in the details to create a new role.',
      confirmText: 'Create',
      cancelText: 'Cancel',
      variant: 'success',
    };

    this.roleService.getListPermissions([]).subscribe({
      next: (groups) => {
        this.roleGroups = groups.data;
        this.buildCreateForm();         
        this.isCreateDialogOpen = true;  
        this.cdr.markForCheck();
      },
      error: () => {
        this.toast.error('Load Permissions', 'Failed to load permissions!');
      }
    });
  }

  deleteRole(id: number) {
    this.deleteRoleId = id;
    this.dialogConfig = {
      title: 'Delete Role',
      message: 'Are you sure you want to permanently delete this role?',
      detail: `Role ID: ${id}`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    };
    this.isDialogOpen = true;
  }

  onCreateConfirmed(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const permissionsValue = this.createForm.get('permissions')?.value as Record<string, boolean>;
    const selectedPermissions = Object.entries(permissionsValue)
      .filter(([_, checked]) => checked)
      .map(([name]) => name);

    const payload: CreateRoleRequest = {
      name: this.createForm.value.name,
      description: this.createForm.value.description,
      permissionNames: selectedPermissions
    };

    this.roleService.createRole(payload).subscribe({
      next: () => {
        this.toast.success('Create Role', 'Role created successfully!');
        this.isCreateDialogOpen = false;
        this.filters.page = 1;
        this.loadRoles();
      },
      error: (err) => {
        if (err.status === 400 && (err.error?.errors || err.error?.message)) {
          const serverError = err.error?.errors ?? err.error?.message;
          for (const field in serverError) {
            const control = this.createForm.get(field.toLowerCase());
            if (control) {
              control.setErrors({ serverError: serverError[field][0] });
            }
          }
          this.toast.error('Validation failed. Please check your input.');
        } else {
          this.toast.error('Create Role', err?.error?.message || 'Create Failed!');
        }
      }
    });
  }

  onEditConfirmed(): void {
    if (this.editForm.invalid) { this.editForm.markAllAsTouched(); return; }

    const permissionsValue = this.editForm.get('permissions')?.value as Record<string, boolean>;
    const selectedPermissions = Object.entries(permissionsValue)
      .filter(([_, checked]) => checked)
      .map(([name]) => name);

    const payload = {
      description: this.editForm.value.description,
      permissionNames: selectedPermissions
    };

    this.roleService.updateRole(this.selectedRole, payload).subscribe({
      next: () => {
        this.toast.success('Update Role', 'Updated Role Successfully!!!');
        this.isEditDialogOpen = false;
        this.editForm.reset();
        this.loadRoles();
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors || err.status === 400 && err.error?.message) {
          const serverError = err.error?.errors ?? err.error?.message;
          console.log(serverError, 'dfsaf');
          for (const field in serverError) {

            let controlName = field.toLowerCase();

            const control = this.editForm.get(controlName);

            if (control) {
              control.setErrors({ serverError: serverError[field][0] });
            }
            console.log(controlName, 'controlName');
          }

          this.toast.error('Validation failed. Please check your input.');
        } else {
          this.errorMessage = err?.error?.message ?? 'An error occured. Please try again.';
          this.toast.error('Update failed!!!');
        }
        this.toast.error('Update Role', err?.error?.errors || err?.message || 'Update Failed!!!');
      }
    });

    this.isEditDialogOpen = false;
    this.editForm.reset();
  }

  onConfirmed(): void {
    this.isDialogOpen = false;
    this.roleService.deleteRole(this.deleteRoleId).subscribe({
      next: (res) => {
        if (res.success) {
          this.toast.success('Delete Role', 'Deleted Role Successfully!!!');
          this.filters.page = 1;
          this.loadRoles();
        }
      },
      error: (err) => {
        if (err.status === 409) {
          const msg = err.error?.message || 'Delete Failed!!!';
          this.toast.error('Delete Role', msg);
        } else {
          this.toast.error('Delete Role', 'Unexpected error!');
        }
      }
    });
  }

  onCreateCancelled(): void {
    this.isCreateDialogOpen = false;
    this.roleGroups = [];
  }

  onEditCancelled(): void {
    this.isEditDialogOpen = false;
    this.roleGroups = [];
  }

  onCancelled(): void {
    this.isDialogOpen = false;
  }

  isEditInvalid(controlPath: string): boolean {
    const control = this.editForm.get(controlPath);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  isCreateInvalid(controlPath: string): boolean {
    const control = this.createForm.get(controlPath);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }
}
