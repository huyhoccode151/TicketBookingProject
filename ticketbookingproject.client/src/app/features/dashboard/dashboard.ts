import { ChangeDetectorRef, Component, inject, NgModule, OnInit, ViewEncapsulation } from '@angular/core';
import { CommonModule } from "@angular/common";
import { ChartPoint, ChartSegment, EventTrendingResponse, RecentBookingListRequest, TicketWithEventType, TotalVenueResponse } from './models/dashboard';
import { DashboardService } from './services/dashboard-service';
import { ToastService } from '../../shared/ui/toast/toast.service';
import { FormsModule } from '@angular/forms';
import { Loader } from '../../shared/ui/loader/loader';
import { AdminBookingListItemResponse } from '../booking/models/booking';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, Loader],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Dashboard implements OnInit {
  totalVenue!: TotalVenueResponse[];
  cateWithTic!: TicketWithEventType[];
  bookings!: AdminBookingListItemResponse[];
  events!: EventTrendingResponse[];
  eventNames!: string[];
  userNames!: string[];
  filterCriteria: RecentBookingListRequest = {
    userName: '',
    eventName: '',
    dateFrom: '',
    dateTo: ''
  };

  loading: boolean = true;

  //point chart revenue
  linePath: string = '';
  areaPath: string = '';
  labels: string[] = [];

  //donut chart
  segments: ChartSegment[] = [];
  totalSold: number = 0;
  colors = ['var(--primary)', '#bc4800', '#515f74', '#cbd5e1', '#6366f1'];

  private dashboardService = inject(DashboardService);
  private toast = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.loadTotalVenue();
    this.loadCateWithTicNum();
    this.loadEventTrending();
    this.loadEventName();
    this.loadUserName();
    this.loadRecentBooking();
    setTimeout(() => {
      this.loading = false;
      this.cdr.markForCheck();
    }, 1000);
  }

  loadEventName(req: string = '') {
    this.dashboardService.getEventName(req).subscribe({
      next: (res) => {
        this.eventNames = res.data;
        console.log("load event name success", this.eventNames);
        this.cdr.markForCheck();
      }
    });
  }

  loadUserName(req: string = '') {
    this.dashboardService.getUserName(req).subscribe({
      next: (res) => {
        this.userNames = res.data;
        console.log("load user name success", this.userNames);
        this.cdr.markForCheck();
      }
    });
  }

  loadTotalVenue() {
    this.dashboardService.getRevenueOverTime().subscribe({
      next: (res) => {
        this.totalVenue = res.data;
        console.log("load revenue success", this.totalVenue);
        this.generateChart(this.totalVenue);
        this.cdr.markForCheck();
      },
      error: () => {
        this.toast.error("load revenue failed");
      }
    });
  }

  loadCateWithTicNum() {
    this.dashboardService.getCateWithTicNum().subscribe({
      next: (res) => {
        this.cateWithTic = res.data;
        console.log("load cate with tic num success", this.cateWithTic);
        this.calculateChart();
        this.cdr.markForCheck();
      },
      error: () => {
        this.toast.error("load cate with tic num failed");
      }
    });
  }

  loadEventTrending() {
    this.dashboardService.getEventTrending().subscribe({
      next: (res) => {
        this.events = res.data;
        console.log("load event trending success", this.events);
        this.cdr.markForCheck();
      },
      error: () => {
        this.toast.error("load event trending failed");
      }
    });
  }

  loadRecentBooking(req?: RecentBookingListRequest) {
    this.dashboardService.getRecentBooking(req).subscribe({
      next: (res) => {
        this.bookings = res.data;
        console.log("load booking success", this.bookings);
        this.cdr.markForCheck();
      },
      error: () => {
        this.toast.error("load booking failed");
      }
    });
  }

  getInitials(name: string): string {
    if (!name) return '??';
    return name.split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  // Hàm để map class CSS theo status (giả sử: 1: Paid, 2: Pending, 3: Cancelled)
  getStatusClass(status: number): string {
    switch (status) {
      case 1: return 'status-paid';    // Thành công / Đã thanh toán
      case 2: return 'status-pending'; // Chờ
      case 0: return 'status-cancelled'; // Đã hủy
      default: return 'status-pending';
    }
  }

  //point chart
  generateChart(rawData: TotalVenueResponse[]) {
    const dailyData: { [key: string]: number } = {};
    rawData.forEach(item => {
      const date = item.date.split('T')[0];
      dailyData[date] = (dailyData[date] || 0) + item.revenue;
    });

    const sortedDates = Object.keys(dailyData).sort();
    const values = sortedDates.map(date => dailyData[date]);

    const maxRevenue = Math.max(...values) * 1.2;
    const totalPoints = sortedDates.length;

    const points: ChartPoint[] = sortedDates.map((date, index) => {
      const x = (index / (totalPoints - 1)) * 100;
      const y = 100 - (dailyData[date] / maxRevenue) * 100;
      return {
        x,
        y,
        label: this.formatDateLabel(date),
        originalValue: dailyData[date]
      };
    });

    this.linePath = points.map((p, i) => `${i === 0 ? 'M' : 'L'} ${p.x},${p.y}`).join(' ');

    this.areaPath = `${this.linePath} L 100,100 L 0,100 Z`;

    this.labels = points.map(p => p.label);
  }

  formatDateLabel(dateStr: string) {
    const d = new Date(dateStr);
    return d.toLocaleDateString('en-US', { month: 'short', day: '2-digit' });
  }

  //donut chart
  calculateChart() {
    this.totalSold = this.cateWithTic.reduce((sum, item) => sum + item.sold, 0);
    
    let cumulativePercentage = 0;

    this.segments = this.cateWithTic.map((item, index) => {
      const percentage = (item.sold / this.totalSold) * 100;

      const offset = 25 - cumulativePercentage;

      const segment = {
        label: item.eventType,
        value: item.sold,
        percentage: Math.round(percentage),
        color: this.colors[index % this.colors.length],
        dashArray: `${percentage} ${100 - percentage}`,
        dashOffset: offset
      }

      cumulativePercentage += percentage;
      return segment;
    });
  }
  //event trending
  getCapacity(event: EventTrendingResponse): number {
    if (!event.stock || event.stock === 0) return 0;
    const cap = (event.sold / (event.stock + event.sold)) * 100;
    // Nếu phần trăm quá nhỏ (như 0.3%), chúng ta có thể làm tròn lên 1 hoặc để số thập phân 1 chữ số
    return parseFloat(cap.toFixed(1));
  }

  // Lấy màu sắc cho Progress Bar dựa trên vị trí
  getEventStyles(index: number) {
    const styles = [
      { color: '#2563eb' }, // Blue
      { color: '#7c3aed' }, // Purple
      { color: '#db2777' }  // Pink
    ];
    return styles[index % styles.length];
  }

  onApplyFilters() {
    // Kiểm tra logic nếu cần (ví dụ: ngày bắt đầu không được lớn hơn ngày kết thúc)
    if (this.filterCriteria.dateFrom && this.filterCriteria.dateTo) {
      if (new Date(this.filterCriteria.dateFrom) > new Date(this.filterCriteria.dateTo)) {
        alert('Start date cannot be later than end date');
        return;
      }
    }
    console.log(this.filterCriteria);
    this.loadRecentBooking(this.filterCriteria);
  }
}
