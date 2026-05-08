import { CommonModule, NgClass } from '@angular/common';
import { ChangeDetectorRef, Component, HostListener, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { RouteService } from '../../core/services/route.service';

@Component({
  selector: 'app-user-header',
  standalone: true,
  imports: [RouterOutlet, NgClass, CommonModule],
  templateUrl: './user-header.html',
  styleUrls: ['../shell/shell.scss', './user-header.scss'],
})
export class UserHeader {
  isDropdownOpen: boolean = false;
  isLogin: boolean = false;

  private authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private route = inject(RouteService);

  ngOnInit() {
    const token = localStorage.getItem('access_token');
    if (token) this.isLogin = true;
    this.cdr.markForCheck();
  }

  toggleDropdown(event: MouseEvent) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  @HostListener('document:click')
  closeDropdown() {
    this.isDropdownOpen = false;
  }

  goProfile() {
    this.router.navigate(this.route.profile());
  }

  logout() {
    this.authService.logout();
  }
}
