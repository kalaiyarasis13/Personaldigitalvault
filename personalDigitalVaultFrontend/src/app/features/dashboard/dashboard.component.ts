import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { FolderService } from "../../core/services/folder.service";
import { ToastService } from "../../core/services/toast.service";
import { FeedbackService } from "../../core/services/feedback.service";
import { FolderItem } from "../../core/models/folder.model";
import { FeedbackItem } from "../../core/models/feedback.model";

@Component({
  selector: "app-dashboard",
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: "./dashboard.component.html",
  styleUrl: "./dashboard.component.css"
})
export class DashboardComponent implements OnInit {
  folders: FolderItem[] = [];
  loading = true;
  showCreateForm = false;
  newFolderName = "";
  creating = false;

  renamingId: number | null = null;
  renameValue = "";

  myFeedback: FeedbackItem | null = null;

  constructor(
    private folderService: FolderService,
    private toast: ToastService,
    private feedbackService: FeedbackService
  ) {}

  ngOnInit() {
    this.load();
    this.feedbackService.getMine().subscribe((res) => (this.myFeedback = res.data ?? null));
  }

  load() {
    this.loading = true;
    this.folderService.getAll().subscribe({
      next: (res) => { this.folders = res.data ?? []; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  createFolder() {
    if (!this.newFolderName.trim()) return;
    this.creating = true;
    this.folderService.create({ name: this.newFolderName.trim() }).subscribe({
      next: (res) => {
        this.creating = false;
        this.newFolderName = "";
        this.showCreateForm = false;
        this.toast.success(res.message || "Folder created.");
        this.load();
      },
      error: () => { this.creating = false; }
    });
  }

  startRename(folder: FolderItem) {
    this.renamingId = folder.id;
    this.renameValue = folder.name;
  }

  confirmRename(folder: FolderItem) {
    if (!this.renameValue.trim()) return;
    this.folderService.rename(folder.id, this.renameValue.trim()).subscribe({
      next: (res) => {
        this.toast.success(res.message || "Folder renamed.");
        this.renamingId = null;
        this.load();
      }
    });
  }

  deleteFolder(folder: FolderItem) {
    if (!confirm(`Delete folder "${folder.name}"? This cannot be undone.`)) return;
    this.folderService.delete(folder.id).subscribe({
      next: (res) => { this.toast.success(res.message || "Folder deleted."); this.load(); }
    });
  }
}
