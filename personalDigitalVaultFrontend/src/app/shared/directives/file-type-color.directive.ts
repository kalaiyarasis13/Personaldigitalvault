import { Directive, ElementRef, Input, OnChanges } from "@angular/core";


@Directive({
  selector: "[appFileTypeColor]",
  standalone: true
})
export class FileTypeColorDirective implements OnChanges {
  @Input("appFileTypeColor") fileName: string = "";

  private static readonly COLOR_MAP: Record<string, string> = {
    pdf: "#dc2626",
    doc: "#2563eb", docx: "#2563eb",
    xls: "#16a34a", xlsx: "#16a34a", csv: "#16a34a",
    ppt: "#ea580c", pptx: "#ea580c",
    png: "#7c3aed", jpg: "#7c3aed", jpeg: "#7c3aed", gif: "#7c3aed", webp: "#7c3aed",
    zip: "#525252", rar: "#525252", "7z": "#525252",
    txt: "#64748b"
  };

  constructor(private el: ElementRef<HTMLElement>) {}

  ngOnChanges() {
    const ext = (this.fileName.split(".").pop() || "").toLowerCase();
    const color = FileTypeColorDirective.COLOR_MAP[ext] || "#94a3b8";
    this.el.nativeElement.style.borderLeft = `4px solid ${color}`;
  }
}
