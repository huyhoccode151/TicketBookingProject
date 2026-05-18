import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { Card } from '../../../shared/ui/card/card';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService, passwordMatchValidator } from '../services/user';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { ApiResponse } from '../models/paged-result';
import { Permission, RolePermissionGroup, User } from '../models/user';
import { disabled } from '@angular/forms/signals';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { CommonModule } from '@angular/common';
import { HasPermissionDirective } from '../../../shared/directives/has-permission-directive';
import { RouteService } from '../../../core/services/route.service';

@Component({
  selector: 'app-edit',
  standalone: true,
  imports: [PageHeader, Card, ReactiveFormsModule, ConfirmDialog, CommonModule, HasPermissionDirective],
  templateUrl: './edit.html',
  styleUrl: './edit.scss',
})
export class Edit implements OnInit {
  form: FormGroup;
  submitted = false;
  userId!: string;
  email!: string;
  errorMessage: string = '';

  isDialogOpen = false;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };

  permissionForm!: FormGroup;
  permissionLoading = false;
  isPermissionDialogOpen = false;
  roleGroups: Permission[] = [];
  permissionDialogConfig: ConfirmDialogConfig = { title: 'Permissions', message: 'Permission management is not implemented yet.' };


  routeNavigate = inject(RouteService);
  private cdr = inject(ChangeDetectorRef);
  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    private toast: ToastService
  ) {
    this.form = this.fb.group({
      firstname: [''],
      lastname: [''],
      username: [{ value:'', disabled: true }],
      email: [{ value: '', disabled: true }, Validators.email],
      password: [null],
      confirmpassword: [null],
      status: [false],
      role: [''],
    },
      {
        validators: passwordMatchValidator
      });
    this.permissionForm = this.fb.group({});
  }

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id')!;
    this.userService.getUserById(this.userId).subscribe({
      next: (res) => {
        const user = res.data;
        this.email = user.email ?? "";
        this.form.patchValue({
          firstname: user.firstname,
          lastname: user.lastname,
          username: user.username,
          email: user.email,
          status: user.status == 'Active' ? true : false,
          role: user.roles?.[0] || ''
        },);
        console.log(this.form.value);
      },
      error: () => {
        this.toast.error('Error', 'Cannot load user');
        this.router.navigate(this.routeNavigate.users());
      }
    });

    this.userService.getRolePermissions(this.userId).subscribe({
      next: (groups) => {
        this.roleGroups = groups.data.map(p => ({
          key: p,
          label: p,
        }));
        this.buildEmptyPermissionForm();
      },

    });
  }

  submit() {
    this.submitted = true;

    if (this.form.invalid) return;

    const v = this.form.value;

    const request: any = {
      firstname: v.firstname,
      lastname: v.lastname,
      username: v.username,
      email: v.email,
      password: v.password,
      confirmpassword: v.confirmpassword,
      status: v.status ? 'Active' : 'Inactive',
      roles: v.role ? [v.role] : null,
      gender: 'Unknown',
      loginType: 'Email',
      isEmailVerified: true,
    }

    this.userService.editUser(this.userId, request).subscribe({
      next: () => {
        this.form.reset();
        this.submitted = false;
        this.router.navigate(this.routeNavigate.users());
        this.toast.success('Edited User', 'Edited user successfully!!!');
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors) {
          const serverError = err.error?.errors;
          console.log(serverError, 'dfsaf');
          for (const field in serverError) {

            let controlName = field.toLowerCase();

            const control = this.form.get(controlName);

            if (control) {
              control.setErrors({ serverError: serverError[field][0] });
            }
          }

          this.toast.error('Validation failed. Please check your input.');
        } else {
          this.toast.error('Edit failed!!!');
        }
      }
    });
  }

  isInvalid(field: string) {
    return this.submitted && this.form.get(field)?.invalid;
  }

  openDelete(): void {
    this.dialogConfig = {
      title: 'Delete User',
      message: 'Are you surely delete this user?',
      detail: `Account: ${this.email}`,
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

  openPermissions() {
    this.permissionDialogConfig = {
      title: 'Change user permission',
      message: 'Change user permission!!!',
      detail: `Account: ${this.email}`,
      confirmText: 'Submit',
      cancelText: 'Cancel',
      variant: 'warning',
    } 
    this.isPermissionDialogOpen = true;

    this.buildEmptyPermissionForm();
    this.loadUserPermissions();
  }

  private buildEmptyPermissionForm()
  {
    const controls: Record<string, boolean> = {};
    this.roleGroups.forEach(p => controls[p.key] = false);
    this.permissionForm = this.fb.group(controls);
  }

  private loadUserPermissions() {
    this.permissionLoading = true;
    this.userService.getUserPermissions(this.userId).subscribe({
      next: (userPerms) => {
        const patch: Record<string, boolean> = {};
        userPerms.data.forEach(key => {
          if (this.permissionForm.contains(key)) patch[key] = true;
        });
        this.permissionForm.patchValue(patch);
        this.permissionLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.permissionLoading = false;
        this.cdr.detectChanges();
        this.toast.error('Load Permissions', 'Failed to load user permissions!!!');
      }
    });
  }

  onPermissionConfirmed() {
    const selected = Object.entries(this.permissionForm.value)
      .filter(([_, checked]) => checked)
      .map(([key]) => key);

    this.userService.updatePermissions(this.userId, selected).subscribe({
      next: () => {
        this.toast.success('Permissions', 'Updated permissions successfully!');
        this.isPermissionDialogOpen = false;
      },
      error: (err) => {
        this.toast.error("Permission", err?.error?.errors?.Permissions[0]);
        //this.toast.error('Permissions', 'Update permissions failed!');
      }
    });
  }

  onPermissionCancelled(): void {
    this.isPermissionDialogOpen = false;
  }

  onNavigateToUsers() {
    this.router.navigate(this.routeNavigate.users());
  }
}
