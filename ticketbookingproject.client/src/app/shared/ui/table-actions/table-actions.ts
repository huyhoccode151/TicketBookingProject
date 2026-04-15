import { Component, Output, Input, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-table-actions',
  standalone: true,
  templateUrl: './table-actions.html',
  styleUrl: './table-actions.scss',
})
export class TableActions {
  @Input() id!: number;

  @Output() edit = new EventEmitter<number>();
  @Output() delete = new EventEmitter<number>();
  @Output() view = new EventEmitter<number>();
}
