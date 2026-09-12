import { Directive, EventEmitter, HostBinding, HostListener, OnDestroy, OnInit, Output } from "@angular/core";


@Directive({
  selector: "[appDropZone]",
  standalone: true
})
export class DropZoneDirective implements OnInit, OnDestroy {
  @Output() filesDropped = new EventEmitter<FileList>();

  @HostBinding("class.drop-zone-active") isDragOver = false;

  private readonly preventWindowDefault = (event: DragEvent) => event.preventDefault();

  ngOnInit() {
    window.addEventListener("dragover", this.preventWindowDefault);
    window.addEventListener("drop", this.preventWindowDefault);
  }

  ngOnDestroy() {
    window.removeEventListener("dragover", this.preventWindowDefault);
    window.removeEventListener("drop", this.preventWindowDefault);
  }

  @HostListener("dragenter", ["$event"])
  onDragEnter(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  @HostListener("dragover", ["$event"])
  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  @HostListener("dragleave", ["$event"])
  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
  }

  @HostListener("drop", ["$event"])
  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.filesDropped.emit(files);
    }
  }
}
