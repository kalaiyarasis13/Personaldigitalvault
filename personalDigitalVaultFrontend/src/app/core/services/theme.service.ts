import { Injectable, signal } from "@angular/core";

export type ThemeMode = "dark" | "light";

const THEME_KEY = "pdv_theme";

@Injectable({ providedIn: "root" })
export class ThemeService {
  readonly theme = signal<ThemeMode>(this.loadInitialTheme());

  constructor() {
    this.applyToDocument(this.theme());
  }

  private loadInitialTheme(): ThemeMode {
    const stored = localStorage.getItem(THEME_KEY) as ThemeMode | null;
    if (stored === "dark" || stored === "light") return stored;

    // Default to dark (matches the app's default black + purple look),
    // but respect an explicit system preference for light mode.
    const prefersLight = window.matchMedia?.("(prefers-color-scheme: light)").matches;
    return prefersLight ? "light" : "dark";
  }

  private applyToDocument(mode: ThemeMode) {
    document.documentElement.classList.toggle("light", mode === "light");
  }

  toggle() {
    const next: ThemeMode = this.theme() === "dark" ? "light" : "dark";
    this.theme.set(next);
    localStorage.setItem(THEME_KEY, next);
    this.applyToDocument(next);
  }
}
