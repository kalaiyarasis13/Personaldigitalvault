import { AbstractControl, FormArray, ValidationErrors, ValidatorFn } from "@angular/forms";

export function usernameOrEmailRequired(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const username = (group.get("accountUsername")?.value ?? "").trim();
    const customFields = (group.get("customFields") as FormArray)?.controls ?? [];
    const hasEmailField = customFields.some((c) =>
      (c.get("label")?.value ?? "").trim().toLowerCase() === "email" && (c.get("value")?.value ?? "").trim()
    );
    return username || hasEmailField ? null : { usernameOrEmailRequired: true };
  };
}
