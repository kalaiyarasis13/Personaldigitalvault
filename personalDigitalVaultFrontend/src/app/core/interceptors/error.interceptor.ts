import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, from, switchMap, throwError } from "rxjs";
import { AuthService } from "../services/auth.service";
import { ToastService } from "../services/toast.service";

function extractMessage(error: any): Promise<string> {
  // Requests made with responseType: 'blob' (e.g. document download) get a
  // Blob back even on error, not parsed JSON - error.error.message is
  // undefined for these, which is why every failed download used to show
  // the generic fallback instead of the backend's real message.
  if (error?.error instanceof Blob) {
    return error.error.text().then((text: string) => {
      try { return JSON.parse(text)?.message || "Something went wrong. Please try again."; }
      catch { return "Something went wrong. Please try again."; }
    });
  }
  return Promise.resolve(error?.error?.message || "Something went wrong. Please try again.");
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const auth = inject(AuthService);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((error) => from(extractMessage(error)).pipe(
      switchMap((message) => {
        if (error.status === 401) {
          auth.logout();
          router.navigate(["/login"]);
          toast.error("Your session has expired. Please log in again.");
        } else {
          toast.error(message);
        }
        return throwError(() => error);
      })
    ))
  );
};
