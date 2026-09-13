import { Component } from "@angular/core";
import { RouterLink } from "@angular/router";

@Component({
  selector: "app-not-found",
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="not-found">
      <h1>404</h1>
      <p>That page doesn't exist.</p>
      <a routerLink="/" class="btn btn-primary">Go home</a>
    </div>
  `,
  styles: [`
    .not-found {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 80px 20px;
      text-align: center;
    }
    .not-found h1 { font-size: 3rem; margin: 0; color: #2563eb; }
    .not-found p { color: #64748b; margin: 8px 0 20px; }
  `]
})
export class NotFoundComponent {}
