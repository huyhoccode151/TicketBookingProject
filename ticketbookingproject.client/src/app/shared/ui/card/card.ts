import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-card',
  standalone: true,
  templateUrl: './card.html',
  styleUrl: './card.scss',
})
export class Card {
  @Input() icon = 'person';
  @Input() title = '';
}
