import { Injectable, signal } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class LayoutService {
  readonly sidebarCollapsed = signal(false);
  toggleSidebar() { this.sidebarCollapsed.update(v => !v); }
  setSidebarCollapsed(v: boolean) { this.sidebarCollapsed.set(v); }
}
