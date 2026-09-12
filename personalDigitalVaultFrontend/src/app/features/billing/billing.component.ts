import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { PaymentService } from "../../core/services/payment.service";
import { FeedbackService } from "../../core/services/feedback.service";
import { ToastService } from "../../core/services/toast.service";
import { BillingStatus, PaymentHistoryItem } from "../../core/models/payment.model";

type CheckoutStage = "idle" | "processing" | "success" | "feedback" | "done";

@Component({
  selector: "app-billing",
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: "./billing.component.html",
  styleUrl: "./billing.component.css"
})
export class BillingComponent implements OnInit {
  status: BillingStatus | null = null;
  history: PaymentHistoryItem[] = [];
  loading = true;

  stage: CheckoutStage = "idle";
  feedbackRating = 0;
  feedbackComment = "";
  submittingFeedback = false;

  constructor(
    private paymentService: PaymentService,
    private feedbackService: FeedbackService,
    private toast: ToastService
  ) {}

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.paymentService.getBillingStatus().subscribe((res) => { this.status = res.data ?? null; this.loading = false; });
    this.paymentService.getHistory().subscribe((res) => (this.history = res.data ?? []));
  }

  upgrade() {
    this.stage = "processing";

    // Brief simulated processing delay so the checkout feels like a real payment flow.
    setTimeout(() => {
      this.paymentService.checkout("Premium").subscribe({
        next: () => {
          this.stage = "success";
          this.load();
          setTimeout(() => (this.stage = "feedback"), 1100);
        },
        error: () => (this.stage = "idle")
      });
    }, 1500);
  }

  setRating(value: number) {
    this.feedbackRating = value;
  }

  submitFeedback() {
    if (this.feedbackRating < 1) return;
    this.submittingFeedback = true;
    this.feedbackService.submit(this.feedbackRating, this.feedbackComment).subscribe({
      next: () => {
        this.submittingFeedback = false;
        this.stage = "done";
      },
      error: () => (this.submittingFeedback = false)
    });
  }

  skipFeedback() {
    this.stage = "done";
  }

  closeModal() {
    this.stage = "idle";
    this.feedbackRating = 0;
    this.feedbackComment = "";
  }

  formatBytes(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    if (bytes < 1024 * 1024 * 1024) return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    return `${(bytes / (1024 * 1024 * 1024)).toFixed(2)} GB`;
  }

  usagePercent(): number {
    if (!this.status || this.status.storageLimitBytes === 0) return 0;
    return Math.min(100, (this.status.storageUsedBytes / this.status.storageLimitBytes) * 100);
  }
}
