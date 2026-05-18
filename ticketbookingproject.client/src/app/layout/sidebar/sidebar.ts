import { ChangeDetectorRef, Component, inject, Input, NgModule, OnInit, ViewEncapsulation } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HasPermissionDirective } from '../../shared/directives/has-permission-directive';
import { RouteService } from '../../core/services/route.service';
import { UIActionService } from '../../features/ui-action/services/ui-action';
import { UIActionResponse } from '../../features/ui-action/models/ui-action';
import { ToastService } from '../../shared/ui/toast/toast.service';
import { CommonModule } from '@angular/common';
import { Shell } from '../shell/shell';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, HasPermissionDirective, CommonModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Sidebar {
  protected route = inject(RouteService);

  @Input({ required: true })
  actions!: UIActionResponse[];
}
