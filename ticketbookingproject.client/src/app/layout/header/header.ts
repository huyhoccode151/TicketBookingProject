import { ChangeDetectionStrategy, Component, EventEmitter, ViewEncapsulation, Output, HostListener, inject } from "@angular/core";
import { AuthService } from "../../core/services/auth.service";
import { CommonModule } from "@angular/common";
import { Router } from "@angular/router";
import { RouteService } from "../../core/services/route.service";
@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.html',
  styleUrls: ['./header.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Header {
  isDropdownOpen: boolean = false;

  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(RouteService);

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
