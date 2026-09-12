import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { API_BASE_URL } from "./environment";
import { ApiResponse } from "../models/api-response.model";
import { DocumentItem } from "../models/document.model";

@Injectable({ providedIn: "root" })
export class DocumentService {
  constructor(private http: HttpClient) {}

  getAll(folderId?: number | null, search?: string): Observable<ApiResponse<DocumentItem[]>> {
    let params = new HttpParams();
    if (folderId !== undefined && folderId !== null) params = params.set("folderId", folderId);
    if (search) params = params.set("search", search);
    return this.http.get<ApiResponse<DocumentItem[]>>(`${API_BASE_URL}/documents`, { params });
  }

  upload(file: File, folderId?: number | null): Observable<ApiResponse<DocumentItem>> {
    const formData = new FormData();
    formData.append("file", file);
    if (folderId !== undefined && folderId !== null) formData.append("folderId", folderId.toString());
    return this.http.post<ApiResponse<DocumentItem>>(`${API_BASE_URL}/documents/upload`, formData);
  }

  // BRD FR#7: "views" is a distinct action from "downloads". Fetched via
  // HttpClient (so the Authorization header is attached - a bare <a
  // href> to the API URL wouldn't carry the JWT) as a blob, then opened
  // in a new tab with no download attribute, so the browser renders
  // previewable types (images, PDF) inline instead of saving them.
  view(id: number) {
    this.http.get(`${API_BASE_URL}/documents/${id}/view`, { responseType: "blob" }).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      window.open(url, "_blank");
      // Revoke well after the new tab has had time to load the blob URL.
      setTimeout(() => window.URL.revokeObjectURL(url), 60000);
    });
  }

  download(id: number, fileName: string) {
    // Errors surface through the global errorInterceptor (which now
    // correctly parses Blob-typed error bodies), so no local error
    // handling/toast here - that would just duplicate the notification.
    this.http.get(`${API_BASE_URL}/documents/${id}/download`, { responseType: "blob" }).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = fileName;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  rename(id: number, newFileName: string): Observable<ApiResponse<DocumentItem>> {
    return this.http.put<ApiResponse<DocumentItem>>(`${API_BASE_URL}/documents/${id}/rename`, { newFileName });
  }

  move(id: number, folderId: number | null): Observable<ApiResponse<DocumentItem>> {
    return this.http.put<ApiResponse<DocumentItem>>(`${API_BASE_URL}/documents/${id}/move`, { folderId });
  }

  delete(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${API_BASE_URL}/documents/${id}`);
  }
}
