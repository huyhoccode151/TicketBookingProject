import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { Permission, Role } from '../models/permission';
import { PermissionService } from '../services/permission-service';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { Loader } from '../../../shared/ui/loader/loader';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [FormsModule, CommonModule, Pagination, Loader, ReactiveFormsModule],
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
})
export class Index {
  loading: boolean = true;
  permissions: Permission[] = [];
  roles: Role[] = [];
  permForm!: FormGroup;

  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;

  // Filter & Create
  filterAction: string = '';
  filterResource: string = '';
  newPerm: any = { action: '', resource: '', description: '' };
  errorMessage: string = '';

  // Role ID Mapping (Phải khớp với DB)

  private permService = inject(PermissionService);
  private cdr = inject(ChangeDetectorRef);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  //ngOnInit() {
  //  this.loadPermissions();
  //}

  ngOnInit() {
    this.loadAll();
    this.permForm = this.fb.group({
      resource: ['', Validators.required],
      action: ['', Validators.required],
      description: [''],
    });
    this.cdr.markForCheck();
  }

  loadAll() {
    this.loading = true;
    forkJoin({
      roles: this.permService.getRoles(),
      perms: this.permService.getPermissions(
        this.filterAction, this.filterResource, this.page, this.pageSize
      )
    }).subscribe({
      next: ({ roles, perms }) => {
        this.roles = roles.data.items;        // ApiResponse<Role[]> → .data
        this.permissions = perms.data.items;
        this.pageSize = perms.data.pageSize;
        this.totalCount = perms.data.totalCount;
        this.totalPages = perms.data.totalPages;
        setTimeout(() => {
          this.loading = false;
          this.cdr.markForCheck();
        }, 1000);
      },
      error: (err) => {
        this.loading = false;
        this.toast.error('Loading failed!!!');
      }
    });
  }

  loadPermissions() {
    this.permService.getPermissions(this.filterAction, this.filterResource, this.page, this.pageSize)
      .subscribe({
        next: (res) => {
          this.permissions = res.data.items;
          this.pageSize = res.data.pageSize;
          this.totalCount = res.data.totalCount;
          this.totalPages = res.data.totalPages;
          console.log(this.permissions);
          setTimeout(() => {
            this.loading = false;
            this.cdr.markForCheck();
          }, 1000);
        },
        error: () => {
          this.loading = false;
          this.toast.error("Loading Payments failed!!!");
        }
      });
  }

  isRoleSelected(perm: Permission, roleId: number): boolean {
    return perm.roleStates?.[roleId.toString() as any] ?? false;
  }

  onToggle(perm: Permission, role: Role, event: Event) {
    const newValue = (event.target as HTMLInputElement).checked;
    perm.isUpdating = true;

    this.permService.togglePermission({
      roleId: role.id,
      permissionId: perm.id,
      isSelected: newValue
    }).subscribe({
      next: () => {
        perm.isUpdating = false;
        if (!perm.roleStates) perm.roleStates = {};
        perm.roleStates[role.id] = newValue;
        this.toast.success('Update role permission!!!', 'Updated successfully!!!');
      },
      error: () => {
        perm.isUpdating = false;
        // Revert
        if (!perm.roleStates) perm.roleStates = {};
        perm.roleStates[role.id] = !newValue;
        this.toast.error('Update failed!');
      }
    });
  }

  createPermission() {
    if (this.permForm.invalid) return;
    const payload = this.permForm.value;
    this.permService.createPermission(payload).subscribe({
      next: () => {
        this.loadPermissions();
        this.toast.success('Create permission', 'Create permission successfully!!!');
        this.newPerm = { action: '', resource: '', description: '' };
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors || err.status === 400 && err.error?.message) {
          const serverError = err.error?.errors ?? err.error?.message;
          console.log(serverError, 'dfsaf');
          for (const field in serverError) {

            let controlName = field.toLowerCase();

            const control = this.permForm.get(controlName);

            if (control) {
              control.setErrors({ serverError: serverError[field][0] });
            }
            console.log(controlName, 'controlName');
          }

          this.toast.error('Validation failed. Please check your input.');
        } else {
          this.errorMessage = err?.error?.message ?? 'An error occured. Please try again.';
          this.toast.error('SignUp failed!!!');
        }
        this.toast.error('Create permission failed!!!');
      }
    });
  }

  onPageChange(p: number) {
    this.page = p;
    this.loadPermissions();
  }

  getError(field: string): string | null {
    const control = this.permForm.get(field);
    if (!control || !control.errors || !control.touched) return null;

    if (control.errors['required']) return `${field} is required`;
    if (control.errors['serverError']) return control.errors['serverError'];
    return null;
  }

  getRoleAccentColor(roleName: string): string {
    const map: Record<string, string> = {
      admin: '#dc2626',
      organizer: '#ea580c',
      customer: '#16a34a',
      staff: '#7c3aed',
    };
    // Fallback tự sinh màu theo hash tên nếu có role mới
    const name = roleName.toLowerCase();
    if (map[name]) return map[name];
    let hash = 0;
    for (const c of name) hash = c.charCodeAt(0) + ((hash << 5) - hash);
    return `hsl(${Math.abs(hash) % 360}, 60%, 45%)`;
  }
}
