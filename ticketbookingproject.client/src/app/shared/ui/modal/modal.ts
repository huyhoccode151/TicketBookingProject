import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './modal.html',
  styleUrls: ['./modal.scss'],
})
export class Modal {
  @Input() id!: string;
  @Input() title!: string;
  @Input() buttonText: string = 'Confirm';
  @Input() color: string = 'primary';
  @Input() dialogClass: string = 'modal-dialog-centered';

  @Output() confirm = new EventEmitter<void>();
}
