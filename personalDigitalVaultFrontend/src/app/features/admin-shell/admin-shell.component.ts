import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";

/**
 * Admin shell - BRD 4.1: "...and an admin shell". Currently the admin
 * area has a single dashboard child route, but keeping it behind a shell
 * (rather than mapping 'admin' straight to the dashboard component) means
 * further admin screens (e.g. a future account list) can be added as
 * sibling child routes without touching top-level routing again.
 */
@Component({
  selector: "app-admin-shell",
  standalone: true,
  imports: [RouterOutlet],
  template: `<router-outlet></router-outlet>`
})
export class AdminShellComponent {}
