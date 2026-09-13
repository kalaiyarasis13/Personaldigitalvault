import { Injectable, signal } from "@angular/core";
import { NavigationStart, Router } from "@angular/router";
import { filter } from "rxjs";

/**
 * Vault state singleton - BRD 4.1: "A vault state singleton holds item
 * counts, storage used and the id of the single currently revealed
 * credential."
 *
 * Every component that can reveal a credential (credentials list,
 * folder-detail credentials tab) reads/writes revealedCredentialId here
 * instead of keeping its own local reveal flag, so only one credential is
 * ever revealed anywhere in the app - and BRD 4.2's "any reveal ends on
 * navigation" is enforced centrally by clearing on every route change.
 */
@Injectable({ providedIn: "root" })
export class VaultStateService {
  private readonly revealedIdSignal = signal<number | null>(null);
  readonly revealedCredentialId = this.revealedIdSignal.asReadonly();

  private readonly documentCountSignal = signal(0);
  private readonly credentialCountSignal = signal(0);
  private readonly storageUsedBytesSignal = signal(0);

  readonly documentCount = this.documentCountSignal.asReadonly();
  readonly credentialCount = this.credentialCountSignal.asReadonly();
  readonly storageUsedBytes = this.storageUsedBytesSignal.asReadonly();

  constructor(router: Router) {
    router.events.pipe(filter((e) => e instanceof NavigationStart)).subscribe(() => {
      this.revealedIdSignal.set(null);
    });
  }

  reveal(id: number) {
    // Toggle: revealing the same id again hides it; revealing a different
    // id replaces whichever one was previously shown.
    this.revealedIdSignal.set(this.revealedIdSignal() === id ? null : id);
  }

  hideRevealed() {
    this.revealedIdSignal.set(null);
  }

  isRevealed(id: number): boolean {
    return this.revealedIdSignal() === id;
  }

  setCounts(documentCount: number, credentialCount: number, storageUsedBytes: number) {
    this.documentCountSignal.set(documentCount);
    this.credentialCountSignal.set(credentialCount);
    this.storageUsedBytesSignal.set(storageUsedBytes);
  }
}
