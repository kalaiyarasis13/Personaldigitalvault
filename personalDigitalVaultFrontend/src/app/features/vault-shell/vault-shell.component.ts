import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";

/**
 * Vault shell - BRD 4.1: "child and nested routes under a vault shell and
 * an admin shell". Every authenticated vault feature (folders, folder
 * detail, credentials, search, profile) is a child route rendered into
 * this shell's <router-outlet>. Navigation for these lives in the main
 * app navbar already, so the shell itself stays a plain wrapper - no
 * duplicate sub-nav.
 */
@Component({
  selector: "app-vault-shell",
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: "./vault-shell.component.html",
  styleUrl: "./vault-shell.component.css"
})
export class VaultShellComponent {}
