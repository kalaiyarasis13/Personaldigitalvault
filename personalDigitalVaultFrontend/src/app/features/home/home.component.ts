import { Component } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterLink } from "@angular/router";
import { AuthService } from "../../core/services/auth.service";
import { ThemeService } from "../../core/services/theme.service";

@Component({
  selector: "app-home",
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: "./home.component.html",
  styleUrl: "./home.component.css"
})
export class HomeComponent {
  constructor(public auth: AuthService, public theme: ThemeService) {}
}
