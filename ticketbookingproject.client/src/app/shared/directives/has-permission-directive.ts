import { Directive } from '@angular/core';
import { ElementRef, inject, Injectable, Input, Renderer2, TemplateRef, ViewContainerRef } from '@angular/core';
import { PermissionService } from '../../core/services/permission-service';

@Directive({
  selector: '[hasPermission]',
})
export class HasPermissionDirective {
  private el = inject(ElementRef);
  private renderer = inject(Renderer2);
  private permission = inject(PermissionService);
  private vcr = inject(ViewContainerRef);
  private tpl = inject(TemplateRef<any>);


  @Input() set hasPermission(perm: string) {
    this.vcr.clear();

    if (this.permission.has(perm)) {
      this.vcr.createEmbeddedView(this.tpl);
    }
  }

}
