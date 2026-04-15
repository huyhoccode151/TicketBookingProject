import { ChangeDetectionStrategy, Component, HostBinding, ViewEncapsulation, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from '../sidebar/sidebar';
import { Header } from '../header/header';
import { NgClass } from '@angular/common';
import { LayoutService } from '../layout.service';


@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, Sidebar, Header, NgClass],
  templateUrl: './shell.html',
  styleUrls: ['./shell.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Shell {
  layout = inject(LayoutService);
}
