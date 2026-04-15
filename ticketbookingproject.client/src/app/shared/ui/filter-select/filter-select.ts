import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface FilterOption {
  label: string;
  value: string;
}

@Component({
  selector: 'app-filter-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './filter-select.html',
  styleUrl: './filter-select.scss',
})
export class FilterSelect {

  @Input() label = '';
  @Input() options: FilterOption[] = [];
  @Input() value = '';

  @Output() valueChange = new EventEmitter<string>();

  onChange(event: Event) {
    const val = (event.target as HTMLSelectElement).value;
    this.valueChange.emit(val);
  }
}
