import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../core/services/auth.service";
import { ToastService } from "../../core/services/toast.service";

@Component({
  selector: "app-profile",
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: "./profile.component.html",
  styleUrl: "./profile.component.css"
})
export class ProfileComponent implements OnInit {
  savingProfile = false;
  savingPassword = false;

  profileForm: ReturnType<FormBuilder["group"]>;
  passwordForm: ReturnType<FormBuilder["group"]>;

  constructor(
    public auth: AuthService,
    private fb: FormBuilder,
    private toast: ToastService
  ) {
    this.profileForm = this.fb.group({
      fullName: [""],
      email: ["", [Validators.required, Validators.email]]
    });

    this.passwordForm = this.fb.group({
      currentPassword: ["", Validators.required],
      newPassword: ["", [Validators.required, Validators.minLength(8)]]
    });
  }

  ngOnInit() {
    const user = this.auth.currentUser();
    if (user) {
      this.profileForm.patchValue({ fullName: user.fullName ?? "", email: user.email });
    }
  }

  saveProfile() {
    if (this.profileForm.invalid) { this.profileForm.markAllAsTouched(); return; }
    this.savingProfile = true;
    this.auth.updateProfile(this.profileForm.getRawValue() as any).subscribe({
      next: (res) => { this.savingProfile = false; this.toast.success(res.message || "Profile updated."); },
      error: () => (this.savingProfile = false)
    });
  }

  changePassword() {
    if (this.passwordForm.invalid) { this.passwordForm.markAllAsTouched(); return; }
    this.savingPassword = true;
    this.auth.changePassword(this.passwordForm.getRawValue() as any).subscribe({
      next: (res) => {
        this.savingPassword = false;
        this.toast.success(res.message || "Password changed.");
        this.passwordForm.reset();
      },
      error: () => (this.savingPassword = false)
    });
  }
}
