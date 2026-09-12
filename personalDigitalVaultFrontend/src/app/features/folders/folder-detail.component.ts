import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { FolderService } from "../../core/services/folder.service";
import { DocumentService } from "../../core/services/document.service";
import { CredentialService } from "../../core/services/credential.service";
import { ToastService } from "../../core/services/toast.service";
import { SharingService } from "../../core/services/sharing.service";
import { VaultStateService } from "../../core/services/vault-state.service";
import { FolderItem } from "../../core/models/folder.model";
import { DocumentItem } from "../../core/models/document.model";
import { CredentialListItem } from "../../core/models/credential.model";
import { CredentialCardComponent } from "../../shared/components/credential-card/credential-card.component";
import { FileSizePipe } from "../../shared/pipes/file-size.pipe";
import { DropZoneDirective } from "../../shared/directives/drop-zone.directive";
import { FileTypeColorDirective } from "../../shared/directives/file-type-color.directive";
import { usernameOrEmailRequired } from "../../shared/validators/username-or-email-required.validator";

@Component({
  selector: "app-folder-detail",
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule, RouterLink,
    CredentialCardComponent, FileSizePipe, DropZoneDirective, FileTypeColorDirective
  ],
  templateUrl: "./folder-detail.component.html",
  styleUrl: "./folder-detail.component.css"
})
export class FolderDetailComponent implements OnInit {
  folderId!: number;
  folder: FolderItem | null = null;
  documents: DocumentItem[] = [];
  credentials: CredentialListItem[] = [];
  loading = true;
  uploading = false;
  documentSearch = "";

  // --- Public sharing (out-of-BRD-scope feature, kept as-is) ---
  shareModalDoc: DocumentItem | null = null;
  shareExpiryHours = 24;
  generatingLink = false;
  generatedShareUrl: string | null = null;

  // Credential reveal state now lives centrally in VaultStateService (BRD
  // 4.1: "the id of the single currently revealed credential") so this tab
  // and the standalone /credentials page can never both have one revealed.
  revealedValues = new Map<number, string>();

  showCredentialForm = false;
  savingCredential = false;
  credentialForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private folderService: FolderService,
    private documentService: DocumentService,
    private credentialService: CredentialService,
    private toast: ToastService,
    private sharingService: SharingService,
    private fb: FormBuilder,
    public vaultState: VaultStateService
  ) {
    this.credentialForm = this.fb.group(
      {
        title: ["", [Validators.required, Validators.maxLength(150)]],
        accountUsername: [""],
        password: ["", [Validators.required]],
        url: [""],
        notes: [""],
        customFields: this.fb.array([])
      },
      { validators: usernameOrEmailRequired() }
    );
  }

  get customFields(): FormArray {
    return this.credentialForm.get("customFields") as FormArray;
  }

  addCustomField() {
    this.customFields.push(this.fb.group({ label: ["", Validators.required], value: [""] }));
  }

  removeCustomField(index: number) {
    this.customFields.removeAt(index);
  }

  ngOnInit() {
    this.folderId = Number(this.route.snapshot.paramMap.get("id"));
    this.load();
  }

  load() {
    this.loading = true;
    this.folderService.getAll().subscribe((res) => {
      this.folder = (res.data ?? []).find((f) => f.id === this.folderId) ?? null;
    });
    this.documentService.getAll(this.folderId, this.documentSearch || undefined).subscribe((res) => (this.documents = res.data ?? []));
    this.credentialService.getAll(this.folderId).subscribe({
      next: (res) => { this.credentials = res.data ?? []; this.loading = false; },
      error: () => (this.loading = false)
    });
  }

  onDocumentSearchChange() {
    this.documentService.getAll(this.folderId, this.documentSearch || undefined).subscribe((res) => {
      this.documents = res.data ?? [];
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;
    this.uploadFile(input.files[0]);
    input.value = "";
  }

  // Handles files dropped via the appDropZone directive (BRD 4.1) as well
  // as the plain file input - both funnel into the same validated upload.
  onFilesDropped(files: FileList) {
    if (files.length === 0) return;
    this.uploadFile(files[0]);
  }

  private uploadFile(file: File) {
    this.uploading = true;
    this.documentService.upload(file, this.folderId).subscribe({
      next: (res) => {
        this.uploading = false;
        this.toast.success(res.message || "File uploaded.");
        this.load();
      },
      error: () => { this.uploading = false; }
    });
  }

  viewDocument(doc: DocumentItem) {
    this.documentService.view(doc.id);
  }

  downloadDocument(doc: DocumentItem) {
    this.documentService.download(doc.id, doc.originalFileName);
  }

  deleteDocument(doc: DocumentItem) {
    if (!confirm(`Delete "${doc.originalFileName}"?`)) return;
    this.documentService.delete(doc.id).subscribe({
      next: (res) => { this.toast.success(res.message || "Document deleted."); this.load(); }
    });
  }

  renameDocument(doc: DocumentItem) {
    const newName = prompt("Rename document to:", doc.originalFileName);
    if (!newName || !newName.trim()) return;
    this.documentService.rename(doc.id, newName.trim()).subscribe({
      next: (res) => { this.toast.success(res.message || "Renamed."); this.load(); }
    });
  }

  openShareModal(doc: DocumentItem) {
    this.shareModalDoc = doc;
    this.generatedShareUrl = null;
    this.shareExpiryHours = 24;
  }

  closeShareModal() {
    this.shareModalDoc = null;
    this.generatedShareUrl = null;
  }

  generateShareLink() {
    if (!this.shareModalDoc) return;
    this.generatingLink = true;
    this.sharingService.createLink(this.shareModalDoc.id, this.shareExpiryHours).subscribe({
      next: (res) => {
        this.generatingLink = false;
        this.generatedShareUrl = res.data?.shareUrl ?? null;
      },
      error: () => (this.generatingLink = false)
    });
  }

  copyShareLink() {
    if (!this.generatedShareUrl) return;
    navigator.clipboard.writeText(this.generatedShareUrl);
    this.toast.success("Link copied to clipboard.");
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

  saveCredential() {
    if (this.credentialForm.invalid) {
      this.credentialForm.markAllAsTouched();
      return;
    }
    const raw = this.credentialForm.value;
    const customFieldLines = (raw.customFields as { label: string; value: string }[])
      .filter((f) => f.label?.trim())
      .map((f) => `${f.label}: ${f.value ?? ""}`)
      .join("\n");
    const notes = [raw.notes, customFieldLines].filter(Boolean).join("\n\n");

    this.savingCredential = true;
    this.credentialService
      .create({
        title: raw.title,
        accountUsername: raw.accountUsername || undefined,
        password: raw.password,
        url: raw.url || undefined,
        notes: notes || undefined,
        folderId: this.folderId
      })
      .subscribe({
        next: (res) => {
          this.savingCredential = false;
          this.showCredentialForm = false;
          this.credentialForm.reset();
          this.customFields.clear();
          this.toast.success(res.message || "Credential saved.");
          this.load();
        },
        error: () => { this.savingCredential = false; }
      });
  }

  deleteCredential(id: number) {
    const cred = this.credentials.find((c) => c.id === id);
    if (!cred) return;
    if (!confirm(`Delete credential "${cred.title}"?`)) return;
    this.credentialService.delete(id).subscribe({
      next: (res) => {
        this.toast.success(res.message || "Credential deleted.");
        this.revealedValues.delete(id);
        this.load();
      }
    });
  }
}
