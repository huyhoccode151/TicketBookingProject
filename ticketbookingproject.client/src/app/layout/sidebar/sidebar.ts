import { Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HasPermissionDirective } from '../../shared/directives/has-permission-directive';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, HasPermissionDirective],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Sidebar {
}
