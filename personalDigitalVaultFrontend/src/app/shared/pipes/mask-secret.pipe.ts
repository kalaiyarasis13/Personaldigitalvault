import { Pipe, PipeTransform } from "@angular/core";


@Pipe({
  name: "maskSecret",
  standalone: true
})
export class MaskSecretPipe implements PipeTransform {
  transform(value: string | null | undefined, revealed: boolean = false, visibleChars: number = 0): string {
    if (revealed) return value ?? "";
    if (!visibleChars) return "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022";
    const safeValue = value ?? "";
    return safeValue.slice(0, visibleChars).padEnd(safeValue.length, "\u2022") || "\u2022\u2022\u2022\u2022\u2022\u2022\u2022\u2022";
  }
}
