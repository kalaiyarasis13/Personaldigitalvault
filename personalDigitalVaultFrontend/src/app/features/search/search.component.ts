import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { DocumentService } from "../../core/services/documents.service";
import { CredentialService } from "../../core/services/Credential.service";
import { VaultStateService } from "../../core/services/vault-state.service";
import { DocumentItem } from "../../core/models/document.model";
import { CredentialListItem } from "../../core/models/Credential.model";
import { CredentialCardComponent } from "../../shared/components/Credential-card/credential-card.component";
import { FileSizePipe } from "../../shared/pipes/file-size.pipe";

/**
 * Search screen - BRD 4.1 routing: "a query-parameter search screen".
 * The search term lives in the URL as ?q=..., not component state, so the
 * search is shareable/bookmarkable and survives a refresh.
 */
@Component({
  selector: "app-search",
  standalone: true,
  imports: [CommonModule, FormsModule, CredentialCardComponent, FileSizePipe],
  templateUrl: "./search.component.html",
  styleUrl: "./search.component.css"
})
export class SearchComponent implements OnInit {
  query = "";
  loading = false;
  searched = false;
  documents: DocumentItem[] = [];
  credentials: CredentialListItem[] = [];
  revealedValues = new Map<number, string>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private documentService: DocumentService,
    private credentialService: CredentialService,
    public vaultState: VaultStateService
  ) {}

  ngOnInit() {
    this.route.queryParamMap.subscribe((params) => {
      const q = params.get("q") ?? "";
      this.query = q;
      if (q) this.runSearch(q);
      else { this.documents = []; this.credentials = []; this.searched = false; }
    });
  }

  onSubmit() {
    // Push the term into the query string - the queryParamMap subscription
    // above is what actually triggers the search.
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { q: this.query || null },
      queryParamsHandling: "merge"
    });
  }

  private runSearch(q: string) {
    this.loading = true;
    this.searched = true;
    this.documentService.getAll(undefined, q).subscribe({
      next: (res) => { this.documents = res.data ?? []; this.loading = false; },
      error: () => (this.loading = false)
    });
    this.credentialService.getAll(undefined, q).subscribe((res) => (this.credentials = res.data ?? []));
  }

  onToggleReveal(id: number) {
    const wasRevealed = this.vaultState.isRevealed(id);
    this.vaultState.reveal(id);
    if (!wasRevealed && this.vaultState.isRevealed(id)) {
      this.credentialService.reveal(id).subscribe((res) => {
        if (res.data) this.revealedValues.set(id, res.data.password);
      });
    }
  }

  isRevealed(id: number): boolean {
    return this.vaultState.isRevealed(id);
  }

  revealedValueFor(id: number): string | null {
    return this.revealedValues.get(id) ?? null;
  }

  viewDocument(doc: DocumentItem) {
    this.documentService.view(doc.id);
  }

  downloadDocument(doc: DocumentItem) {
    this.documentService.download(doc.id, doc.originalFileName);
  }
}
