import { Component } from '@angular/core';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { Card } from '../../../shared/ui/card/card';
import { UserService, passwordMatchValidator } from '../services/user';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Modal } from '../../../shared/ui/modal/modal';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [
    Card,
    PageHeader,
    Modal,
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './create.html',
  styleUrl: './create.scss',
})
export class Create {
  form: FormGroup;
  submitted = false;
  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router,
    private toast: ToastService,
  ) {
    this.form = this.fb.group({
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      username: ['', Validators.required],
      email: ['', Validators.email],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirm_password: ['', Validators.required],
      status: [false],
      role: ['', Validators.required],
    },
      {
        validators: passwordMatchValidator,
      });
  }

  submit() {
    this.submitted = true;

    if (this.form.invalid) return;

    const v = this.form.value;
    
    const request = {
      firstname: v.firstname,
      lastname: v.lastname,
      username: v.username,
      email: v.email,
      password: v.password,
      status: v.status ? 'Active' : 'Inactive',
      roles: [v.role],
      gender: 'Unknown',
      loginType: 'Email',
      isEmailVerified: true,
    }

    this.userService.createUser(request).subscribe({
      next: () => {
        this.form.reset();
        this.submitted = false;
        this.router.navigate(['/users']);
        this.toast.success('Created User', 'Created user successfully!!!');
      },
      error: (err) => {
        console.log('FULL ERROR:', err);
        console.log('VALIDATION:', err.error?.errors);
        this.toast.error('Create User', 'Create user failed!!!');
      }
    });
  }

  isInvalid(field: string) {
    return this.submitted && this.form.get(field)?.invalid;
  }
}
