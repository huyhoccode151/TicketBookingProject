import { ChangeDetectionStrategy, Component, EventEmitter, ViewEncapsulation, Output, HostListener, inject, Input, NgModule } from "@angular/core";
import { AuthService } from "../../core/services/auth.service";
import { CommonModule } from "@angular/common";
import { Router } from "@angular/router";
import { RouteService } from "../../core/services/route.service";
import { UIActionResponse } from "../../features/ui-action/models/ui-action";
import { FormsModule } from "@angular/forms";
import { Shell } from "../shell/shell";
import { HasPermissionDirective } from "../../shared/directives/has-permission-directive";
@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, FormsModule, HasPermissionDirective],
  templateUrl: './header.html',
  styleUrls: ['./header.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Header {
  isDropdownOpen: boolean = false;
  isDropdownSOpen: boolean = false;
  searchKeyword: string = '';
  filteredActions: UIActionResponse[] = [];

  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(RouteService);

  @Input({ required: true })
  actions!: UIActionResponse[];

  toggleDropdown(event: MouseEvent) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  onSearch() {
    const keyword = this.searchKeyword.toLowerCase();

    if (!keyword) {
      this.filteredActions = [];
      this.isDropdownSOpen = false;
      return;
    }

    this.filteredActions = this.actions.filter(action =>
      action.label.toLowerCase().includes(keyword)
    );

    this.isDropdownSOpen = this.filteredActions.length > 0;
  }

  navigate(action: UIActionResponse) {

    const route = this.route.getRoutePath(action.actionKey);

    if (!route) return;

    this.router.navigate(route);
  }

  @HostListener('document:click')
  closeSDropdown() {
    this.isDropdownSOpen = false;
  }

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
