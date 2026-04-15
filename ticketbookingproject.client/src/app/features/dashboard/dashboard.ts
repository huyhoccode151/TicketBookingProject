import { Component, ViewEncapsulation } from '@angular/core';
import { CommonModule } from "@angular/common";

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Dashboard {
  title = 'Barren Admin Dashboard';

  activities = [
    {
      action: 'Event Updated',
      icon: 'edit',
      iconColor: '#3b82f6',
      user: 'Alex Johnson',
      userInitials: 'AJ',
      subject: 'Tech Summit 2024',
      status: 'Approved',
      statusClass: 'status-approved',
      time: '2 minutes ago'
    },
    {
      action: 'New Organizer',
      icon: 'person_add',
      iconColor: '#2aa74d',
      user: 'EventHub LLC',
      userInitials: 'EH',
      subject: 'Partner Application',
      status: 'Pending Review',
      statusClass: 'status-pending',
      time: '15 minutes ago'
    },
    {
      action: 'New Ticket',
      icon: 'report',
      iconColor: '#ef4444',
      user: 'Maria Smith',
      userInitials: 'MS',
      subject: 'Payment Issue',
      status: 'Open',
      statusClass: 'status-open',
      time: '42 minutes ago'
    },
    {
      action: 'Event Deleted',
      icon: 'delete_sweep',
      iconColor: '#64748b',
      user: 'System Admin',
      userInitials: 'S',
      subject: 'Draft #9283',
      status: 'Archived',
      statusClass: 'status-archived',
      time: '1 hour ago'
    }
  ];
}
