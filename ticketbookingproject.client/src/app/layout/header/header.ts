import { ChangeDetectionStrategy, Component, EventEmitter, ViewEncapsulation, Output, HostListener, inject } from "@angular/core";
import { AuthService } from "../../core/services/auth.service";
import { CommonModule } from "@angular/common";
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

  toggleDropdown(event: MouseEvent) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  @HostListener('document:click')
  closeDropdown() {
    this.isDropdownOpen = false;
  }

  logout() {
    this.authService.logout();
  }
}
