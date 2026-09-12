import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AdminService } from "../../core/services/admin.service";
import { ToastService } from "../../core/services/toast.service";
import { FeedbackService } from "../../core/services/feedback.service";
import { AdminUserListItem, DashboardStats } from "../../core/models/admin.model";
import { AdminFeedbackListItem } from "../../core/models/feedback.model";

@Component({
  selector: "app-admin",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./admin.component.html",
  styleUrl: "./admin.component.css"
})
export class AdminComponent implements OnInit {
  stats: DashboardStats | null = null;
  users: AdminUserListItem[] = [];
  feedbacks: AdminFeedbackListItem[] = [];
  loading = true;

  constructor(
    private adminService: AdminService,
    private toast: ToastService,
    private feedbackService: FeedbackService
  ) {}

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.adminService.getDashboard().subscribe((res) => (this.stats = res.data ?? null));
    this.adminService.getUsers().subscribe({
      next: (res) => { this.users = res.data ?? []; this.loading = false; },
      error: () => (this.loading = false)
    });
    this.feedbackService.getAllForAdmin().subscribe((res) => (this.feedbacks = res.data ?? []));
  }

  formatBytes(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }

  toggleStatus(user: AdminUserListItem) {
    this.adminService.setUserStatus(user.id, !user.isActive).subscribe({
      next: (res) => { this.toast.success(res.message || "Updated."); this.load(); }
    });
  }

  deleteUser(user: AdminUserListItem) {
    if (!confirm(`Delete user "${user.username}"? This removes all their folders, documents, and credentials.`)) return;
    this.adminService.deleteUser(user.id).subscribe({
      next: (res) => { this.toast.success(res.message || "User deleted."); this.load(); }
    });
  }
}
