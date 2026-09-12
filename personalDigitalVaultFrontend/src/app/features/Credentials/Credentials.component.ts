import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { CredentialService } from "../../core/services/credential.service";
import { ToastService } from "../../core/services/toast.service";
import { VaultStateService } from "../../core/services/vault-state.service";
import { CredentialListItem } from "../../core/models/credential.model";
import { CredentialCardComponent } from "../../shared/components/credential-card/credential-card.component";
import { usernameOrEmailRequired } from "../../shared/validators/username-or-email-required.validator";

@Component({
  selector: "app-credentials",
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, CredentialCardComponent],
  templateUrl: "./credentials.component.html",
  styleUrl: "./credentials.component.css"
})
export class CredentialsComponent implements OnInit {
  credentials: CredentialListItem[] = [];
  loading = true;
  search = "";
  revealedValues = new Map<number, string>();

  showForm = false;
  saving = false;
  form: FormGroup;

  constructor(
    private credentialService: CredentialService,
    private toast: ToastService,
    private fb: FormBuilder,
    public vaultState: VaultStateService
  ) {
    this.form = this.fb.group(
      {
        title: ["", [Validators.required, Validators.maxLength(150)]],
        accountUsername: [""],
        password: ["", [Validators.required]],
        url: [""],
        notes: [""],
        // FormArray of user-defined fields - BRD 4.1. Lets a record carry
        // arbitrary extra data (PIN, security question, licence key, ...)
        // beyond the fixed title/username/password/url/notes columns.
        customFields: this.fb.array([])
      },
      { validators: usernameOrEmailRequired() }
    );
  }

  get customFields(): FormArray {
    return this.form.get("customFields") as FormArray;
  }

  addCustomField() {
    this.customFields.push(this.fb.group({ label: ["", Validators.required], value: [""] }));
  }

  removeCustomField(index: number) {
    this.customFields.removeAt(index);
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;
    this.credentialService.getAll(null, this.search || undefined).subscribe({
      next: (res) => { this.credentials = res.data ?? []; this.loading = false; },
      error: () => (this.loading = false)
    });
  }

  onSearchChange() {
    this.load();
  }

  // Single source of truth for "who is revealed" - VaultStateService only
  // ever allows one id at a time and clears itself on navigation.
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

  toggleForm() {
    this.showForm = !this.showForm;
  }

  saveCredential() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.value;
    // Custom fields have no dedicated backend columns yet, so they're
    // folded into the encrypted notes as a labelled block rather than
    // dropped - still end-to-end encrypted server-side via EncryptedNotes.
    const customFieldLines = (raw.customFields as { label: string; value: string }[])
      .filter((f) => f.label?.trim())
      .map((f) => `${f.label}: ${f.value ?? ""}`)
      .join("\n");
    const notes = [raw.notes, customFieldLines].filter(Boolean).join("\n\n");

    this.saving = true;
    this.credentialService
      .create({
        title: raw.title,
        accountUsername: raw.accountUsername || undefined,
        password: raw.password,
        url: raw.url || undefined,
        notes: notes || undefined
      })
      .subscribe({
        next: (res) => {
          this.saving = false;
          this.showForm = false;
          this.form.reset();
          this.customFields.clear();
          this.toast.success(res.message || "Credential saved.");
          this.load();
        },
        error: () => { this.saving = false; }
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
