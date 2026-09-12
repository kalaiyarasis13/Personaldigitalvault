import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { API_BASE_URL } from "./environment";
import { ApiResponse } from "../models/api-response.model";
import { ShareLink, ShareLinkListItem } from "../models/share.model";

@Injectable({ providedIn: "root" })
export class SharingService {
  constructor(private http: HttpClient) {}

  createLink(documentId: number, expiryHours: number): Observable<ApiResponse<ShareLink>> {
    return this.http.post<ApiResponse<ShareLink>>(`${API_BASE_URL}/documents/${documentId}/share`, { expiryHours });
  }

  getAll(): Observable<ApiResponse<ShareLinkListItem[]>> {
    return this.http.get<ApiResponse<ShareLinkListItem[]>>(`${API_BASE_URL}/sharing`);
  }

  revoke(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${API_BASE_URL}/sharing/${id}`);
  }
}
