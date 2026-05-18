import { ChangeDetectionStrategy, Component, HostBinding, OnInit, ViewEncapsulation, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from '../sidebar/sidebar';
import { Header } from '../header/header';
import { NgClass } from '@angular/common';
import { LayoutService } from '../layout.service';
import { RouteService } from '../../core/services/route.service';
import { UIActionResponse } from '../../features/ui-action/models/ui-action';
import { UIActionService } from '../../features/ui-action/services/ui-action';
import { ToastService } from '../../shared/ui/toast/toast.service';


@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, Sidebar, Header, NgClass],
  templateUrl: './shell.html',
  styleUrls: ['./shell.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Shell implements OnInit {
  UIActionsManage: UIActionResponse[] = [];

  layout = inject(LayoutService);
  route = inject(RouteService);
  uiAction = inject(UIActionService);
  toast = inject(ToastService);
  actions = signal<UIActionResponse[]>([]);

  ngOnInit() {
    this.load();
  }

  load() {
    this.uiAction.getAllUiManageActions().subscribe({
      next: (res) => {
        console.log(res.data);
        this.UIActionsManage = res.data;
        this.UIActionsManage = this.UIActionsManage.map(action => ({
          ...action,
          routePath: this.route.getRoutePath(action.actionKey)[0]
        }));

        this.actions.set(this.UIActionsManage);
        console.log(this.UIActionsManage);
        this.toast.success("Load UI Actions Manage success");
      },
      error: (err) => {
        this.toast.success("Load UI Actions Manage failed");
      }
    });
  }
}
