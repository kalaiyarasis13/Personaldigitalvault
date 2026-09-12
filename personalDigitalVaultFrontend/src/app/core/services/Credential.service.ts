import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { API_BASE_URL } from "./environment";
import { ApiResponse } from "../models/api-response.model";
import { CredentialListItem, CredentialReveal } from "../models/Credential.model";

@Injectable({ providedIn: "root" })
export class CredentialService {
  constructor(private http: HttpClient) {}

  getAll(folderId?: number | null, search?: string): Observable<ApiResponse<CredentialListItem[]>> {
    let params = new HttpParams();
    if (folderId !== undefined && folderId !== null) params = params.set("folderId", folderId);
    if (search) params = params.set("search", search);
    return this.http.get<ApiResponse<CredentialListItem[]>>(`${API_BASE_URL}/credentials`, { params });
  }

  reveal(id: number): Observable<ApiResponse<CredentialReveal>> {
    return this.http.get<ApiResponse<CredentialReveal>>(`${API_BASE_URL}/credentials/${id}/reveal`);
  }

  create(payload: {
    title: string; accountUsername?: string; password: string;
    notes?: string; url?: string; folderId?: number | null;
  }): Observable<ApiResponse<CredentialListItem>> {
    return this.http.post<ApiResponse<CredentialListItem>>(`${API_BASE_URL}/credentials`, payload);
  }

  update(id: number, payload: {
    title: string; accountUsername?: string; password?: string;
    notes?: string; url?: string; folderId?: number | null;
  }): Observable<ApiResponse<CredentialListItem>> {
    return this.http.put<ApiResponse<CredentialListItem>>(`${API_BASE_URL}/credentials/${id}`, payload);
  }

  delete(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${API_BASE_URL}/credentials/${id}`);
  }
}
