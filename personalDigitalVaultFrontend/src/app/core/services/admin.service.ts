import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { API_BASE_URL } from "./environment";
import { ApiResponse } from "../models/api-response.model";
import { AdminUserListItem, DashboardStats } from "../models/admin.model";

@Injectable({ providedIn: "root" })
export class AdminService {
  constructor(private http: HttpClient) {}

  getDashboard(): Observable<ApiResponse<DashboardStats>> {
    return this.http.get<ApiResponse<DashboardStats>>(`${API_BASE_URL}/admin/dashboard`);
  }

  getUsers(): Observable<ApiResponse<AdminUserListItem[]>> {
    return this.http.get<ApiResponse<AdminUserListItem[]>>(`${API_BASE_URL}/admin/users`);
  }

  setUserStatus(id: number, isActive: boolean): Observable<ApiResponse<object>> {
    return this.http.put<ApiResponse<object>>(`${API_BASE_URL}/admin/users/${id}/status?isActive=${isActive}`, {});
  }

  deleteUser(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${API_BASE_URL}/admin/users/${id}`);
  }
}
