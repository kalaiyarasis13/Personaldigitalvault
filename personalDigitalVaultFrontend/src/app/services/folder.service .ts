import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { API_BASE_URL } from "./environment";
import { ApiResponse } from "../models/api-response.model";
import { FolderItem } from "../models/folder.model";

@Injectable({ providedIn: "root" })
export class FolderService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<FolderItem[]>> {
    return this.http.get<ApiResponse<FolderItem[]>>(`${API_BASE_URL}/folders`);
  }

  create(payload: { name: string; parentFolderId?: number | null }): Observable<ApiResponse<FolderItem>> {
    return this.http.post<ApiResponse<FolderItem>>(`${API_BASE_URL}/folders`, payload);
  }

  rename(id: number, name: string): Observable<ApiResponse<FolderItem>> {
    return this.http.put<ApiResponse<FolderItem>>(`${API_BASE_URL}/folders/${id}`, { name });
  }

  delete(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${API_BASE_URL}/folders/${id}`);
  }
}
