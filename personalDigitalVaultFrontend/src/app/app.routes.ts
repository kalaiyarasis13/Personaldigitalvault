import { Routes } from "@angular/router";
import { authGuard } from "./core/guards/auth.guard";
import { adminGuard } from "./core/guards/admin.guard";

export const routes: Routes = [
  {
    // User's explicit choice: show the marketing home page first when the
    // app is run, even though BRD 4.1 specifies redirectTo: "vault".
    // authGuard still protects everything under /vault and /admin, so
    // this only changes what an unauthenticated visitor sees at "/".
    path: "",
    pathMatch: "full",
    redirectTo: "home"
  },
  {
    path: "home",
    loadComponent: () => import("./features/home/home.component").then(m => m.HomeComponent)
  },
  { path: "login", loadComponent: () => import("./features/auth/login/login.component").then(m => m.LoginComponent) },
  { path: "register", loadComponent: () => import("./features/auth/register/register.component").then(m => m.RegisterComponent) },
  {
    // BRD 4.1: "child and nested routes under a vault shell". Every
    // authenticated vault feature is a child route rendered inside the
    // shell's own sub-nav + router-outlet.
    path: "vault",
    canActivate: [authGuard],
    loadComponent: () => import("./features/vault-shell/vault-shell.component").then(m => m.VaultShellComponent),
    children: [
      { path: "", loadComponent: () => import("./features/dashboard/dashboard.component").then(m => m.DashboardComponent) },
      { path: "folders/:id", loadComponent: () => import("./features/folders/folder-detail.component").then(m => m.FolderDetailComponent) },
      { path: "credentials", loadComponent: () => import("./features/Credentials/Credentials.component").then(m => m.CredentialsComponent) },
      { path: "search", loadComponent: () => import("./features/search/search.component").then(m => m.SearchComponent) },
      { path: "profile", loadComponent: () => import("./features/profile/profile.component").then(m => m.ProfileComponent) }
    ]
  },
  {
    path: "billing",
    canActivate: [authGuard],
    loadComponent: () => import("./features/billing/billing.component").then(m => m.BillingComponent)
  },
  {
    // PUBLIC - no authGuard. Anyone with the link can reach this page.
    path: "share/:token",
    loadComponent: () => import("./features/share-public/share-public.component").then(m => m.SharePublicComponent)
  },
  {
    // BRD 4.1: "...and an admin shell".
    path: "admin",
    canActivate: [authGuard, adminGuard],
    loadComponent: () => import("./features/admin-shell/admin-shell.component").then(m => m.AdminShellComponent),
    children: [
      { path: "", loadComponent: () => import("./features/admin/admin.component").then(m => m.AdminComponent) }
    ]
  },
  {
    // BRD 4.1: "a wildcard 404" - an actual not-found page rather than a
    // silent redirect back to '/'.
    path: "**",
    loadComponent: () => import("./shared/components/not-found/not-found.component").then(m => m.NotFoundComponent)
  }
];
