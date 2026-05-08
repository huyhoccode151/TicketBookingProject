import { ChangeDetectorRef, Directive } from '@angular/core';
import { ElementRef, inject, Injectable, Input, Renderer2, TemplateRef, ViewContainerRef } from '@angular/core';
import { PermissionService } from '../../core/services/permission-service';
import { Subject, takeUntil } from 'rxjs';

@Directive({
  selector: '[hasPermission]',
})
export class HasPermissionDirective {
  private currentPerm!: string;
  private destroy$ = new Subject<void>();

  private el = inject(ElementRef);
  private renderer = inject(Renderer2);
  private permission = inject(PermissionService);
  private vcr = inject(ViewContainerRef);
  private tpl = inject(TemplateRef<any>);
  private cdr = inject(ChangeDetectorRef);


  @Input() set hasPermission(perm: string) {

    this.currentPerm = perm; // ✅ NEW
    this.updateView();
    //this.vcr.clear();

    //if (this.permission.has(perm)) {
    //  this.vcr.createEmbeddedView(this.tpl);
    //}
    //this.cdr.markForCheck();
  }

  ngOnInit() {
    this.permission.permissions$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.updateView();
      });
  }

  // ✅ NEW: tách riêng logic render
  private updateView() {
    this.vcr.clear();

    if (this.permission.has(this.currentPerm)) {
      this.vcr.createEmbeddedView(this.tpl);
    }
  }

  // ✅ NEW: tránh memory leak
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
