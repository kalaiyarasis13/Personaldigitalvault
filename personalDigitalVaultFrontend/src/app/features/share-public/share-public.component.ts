import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ActivatedRoute } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { API_BASE_URL } from "../../core/services/environment";
import { ThemeService } from "../../core/services/theme.service";

@Component({
  selector: "app-share-public",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./share-public.component.html",
  styleUrl: "./share-public.component.css"
})
export class SharePublicComponent implements OnInit {
  token = "";
  loading = false;
  error: string | null = null;

  constructor(private route: ActivatedRoute, private http: HttpClient, public theme: ThemeService) {}

  ngOnInit() {
    this.token = this.route.snapshot.paramMap.get("token") ?? "";
  }

  download() {
    this.loading = true;
    this.error = null;

    this.http.get(`${API_BASE_URL}/share/${this.token}`, { observe: "response", responseType: "blob" }).subscribe({
      next: (response) => {
        this.loading = false;
        const blob = response.body as Blob;
        const disposition = response.headers.get("content-disposition") || "";
        const match = disposition.match(/filename="?([^"]+)"?/);
        const fileName = match ? match[1] : "shared-file";

        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message || "This link is invalid, expired, or has been revoked.";
      }
    });
  }
}
