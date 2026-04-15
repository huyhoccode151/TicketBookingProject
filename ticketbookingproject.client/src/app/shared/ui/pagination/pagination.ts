import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.html',
  styleUrl: './pagination.scss',
})
export class Pagination {
  @Input() page = 1;
  @Input() pageSize = 10;
  @Input() totalPages = 1;
  @Input() totalCount = 0;

  @Output() pageChange = new EventEmitter<number>();

  get start() {
    if (this.totalCount === 0) return 0;
    return (this.page - 1) * this.pageSize + 1;
  }

  get end() {
    return Math.min(this.page * this.pageSize, this.totalCount);
  }

  changePage(p: number) {
    if (p < 1 || p > this.totalPages || p === this.page) return;
    this.pageChange.emit(p);
  }

  get pages(): (number | string)[] {
    const pages: (number | string)[] = [];
    const total = this.totalPages;
    const current = this.page;

    if (total <= 5) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    pages.push(1);

    if (current > 3) {
      pages.push('...');
    }

    const start = Math.max(2, current - 1);
    const end = Math.min(total - 1, current + 1);

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    if (current < total - 2) {
      pages.push('...');
    }

    pages.push(total);

    return pages;
  }
}
