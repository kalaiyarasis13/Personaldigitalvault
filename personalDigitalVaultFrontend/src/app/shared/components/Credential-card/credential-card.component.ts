import { Component, EventEmitter, Input, Output } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MaskSecretPipe } from "../../pipes/mask-secret.pipe";
import { CredentialListItem } from "../../../core/models/credential.model";

/**
 * Dumb child component - BRD 4.1. Owns no API calls and no reveal state of
 * its own; it only renders what the parent gives it and emits events
 * upward. The single source of truth for "is this one revealed" is the
 * parent (via VaultStateService), which is exactly why this component has
 * no local boolean of its own.
 */
@Component({
  selector: "app-credential-card",
  standalone: true,
  imports: [CommonModule, MaskSecretPipe],
  templateUrl: "./credential-card.component.html",
  styleUrl: "./credential-card.component.css"
})
export class CredentialCardComponent {
  @Input({ required: true }) credential!: CredentialListItem;
  @Input() revealed = false;
  @Input() revealedValue: string | null = null;

  @Output() toggleReveal = new EventEmitter<number>();
  @Output() deleteCredential = new EventEmitter<number>();

  onToggleReveal() {
    this.toggleReveal.emit(this.credential.id);
  }

  onDelete() {
    this.deleteCredential.emit(this.credential.id);
  }
}
