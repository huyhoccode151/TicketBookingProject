import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-table-toolbar',
  standalone: true,
  templateUrl: './table-toolbar.html',
  styleUrl: './table-toolbar.scss',
})
export class TableToolbar {
  @Output() search = new EventEmitter<string>();

  onSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.search.emit(value);
  }
}
