import { Component } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterLink } from "@angular/router";
import { AuthService } from "../../../core/services/auth.service";
import { ToastService } from "../../../core/services/toast.service";
import { ThemeService } from "../../../core/services/theme.service";

@Component({
  selector: "app-register",
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: "./register.component.html",
  styleUrl: "../auth.shared.css"
})
export class RegisterComponent {
  submitting = false;
  form: ReturnType<FormBuilder["group"]>;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private toast: ToastService,
    public theme: ThemeService
  ) {
    this.form = this.fb.group({
      username: ["", [Validators.required, Validators.minLength(3)]],
      email: ["", [Validators.required, Validators.email]],
      fullName: [""],
      password: ["", [Validators.required, Validators.minLength(8)]]
    });
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.auth.register(this.form.getRawValue() as any).subscribe({
      next: (res) => {
        this.submitting = false;
        this.toast.success(res.message || "Account created!");
        this.router.navigate(["/vault"]);
      },
      error: () => { this.submitting = false; }
    });
  }
}
