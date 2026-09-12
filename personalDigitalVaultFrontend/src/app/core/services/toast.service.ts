import { Injectable, signal } from "@angular/core";

export interface ToastMessage {
  id: number;
  text: string;
  type: "success" | "error" | "info";
}

@Injectable({ providedIn: "root" })
export class ToastService {
  private nextId = 1;
  readonly messages = signal<ToastMessage[]>([]);

  show(text: string, type: ToastMessage["type"] = "info") {
    const id = this.nextId++;
    this.messages.update((list) => [...list, { id, text, type }]);
    setTimeout(() => this.dismiss(id), 4000);
  }

  success(text: string) { this.show(text, "success"); }
  error(text: string) { this.show(text, "error"); }

  dismiss(id: number) {
    this.messages.update((list) => list.filter((m) => m.id !== id));
  }
}
