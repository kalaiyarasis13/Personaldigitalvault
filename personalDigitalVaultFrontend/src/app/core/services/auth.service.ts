import { Injectable, signal, computed } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable, tap } from "rxjs";
import { API_BASE_URL } from "./environment";
import { ApiResponse } from "../models/api-response.model";
import { AuthResponse, UserProfile } from "../models/user.model";

const TOKEN_KEY = "pdv_token";
const USER_KEY = "pdv_user";

/**
 * BRD 4.2 frontend security rule: "No plaintext secret or decrypted file
 * content is written to browser storage; the JWT lives in memory with a
 * sessionStorage fallback and is cleared on logout."
 *
 * The token is held in a private in-memory field for the life of the tab.
 * sessionStorage is used ONLY so an accidental page refresh doesn't force a
 * re-login mid-task - it is tab-scoped and cleared when the tab closes,
 * unlike localStorage. Never read/write the token via localStorage.
 */
@Injectable({ providedIn: "root" })
export class AuthService {
  private inMemoryToken: string | null = sessionStorage.getItem(TOKEN_KEY);

  private readonly currentUserSignal = signal<UserProfile | null>(this.loadStoredUser());
  readonly currentUser = computed(() => this.currentUserSignal());
  // Requires BOTH a stored profile AND a live token. Closing the browser
  // clears sessionStorage (token) but not localStorage (profile) - without
  // this check, a stale profile alone made the UI think the session was
  // still active (showing the user's name, "Go to Dashboard", etc.) when
  // every API call would actually 401 because the token was gone.
  readonly isAuthenticated = computed(() => !!this.currentUserSignal() && !!this.inMemoryToken);
  readonly isAdmin = computed(() => this.currentUserSignal()?.role === "Administrator");

  constructor(private http: HttpClient) {
    // Startup consistency check: if the token didn't survive (browser was
    // closed) but a profile did, the session is actually over - drop the
    // stale profile too so nothing in the UI implies the user is still
    // logged in.
    if (!this.inMemoryToken && this.currentUserSignal()) {
      localStorage.removeItem(USER_KEY);
      this.currentUserSignal.set(null);
    }
  }

  private loadStoredUser(): UserProfile | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as UserProfile) : null;
  }

  get token(): string | null {
    return this.inMemoryToken;
  }

  register(payload: { username: string; email: string; password: string; fullName?: string }): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${API_BASE_URL}/auth/register`, payload).pipe(
      tap((res) => res.data && this.persistSession(res.data))
    );
  }

  login(payload: { usernameOrEmail: string; password: string }): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${API_BASE_URL}/auth/login`, payload).pipe(
      tap((res) => res.data && this.persistSession(res.data))
    );
  }

  updateProfile(payload: { fullName?: string; email?: string }): Observable<ApiResponse<UserProfile>> {
    return this.http.put<ApiResponse<UserProfile>>(`${API_BASE_URL}/auth/profile`, payload).pipe(
      tap((res) => {
        if (res.data) {
          localStorage.setItem(USER_KEY, JSON.stringify(res.data));
          this.currentUserSignal.set(res.data);
        }
      })
    );
  }

  changePassword(payload: { currentPassword: string; newPassword: string }): Observable<ApiResponse<object>> {
    return this.http.put<ApiResponse<object>>(`${API_BASE_URL}/auth/change-password`, payload);
  }

  private persistSession(data: AuthResponse) {
    this.inMemoryToken = data.token;
    sessionStorage.setItem(TOKEN_KEY, data.token);
    // Only non-secret profile fields go here - never the token.
    localStorage.setItem(USER_KEY, JSON.stringify(data.user));
    this.currentUserSignal.set(data.user);
  }

  logout() {
    this.inMemoryToken = null;
    sessionStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUserSignal.set(null);
  }
}
