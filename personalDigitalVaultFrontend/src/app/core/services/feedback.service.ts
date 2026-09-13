import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { API_BASE_URL } from "./environment";
import { ApiResponse } from "../models/api-response.model";
import { AdminFeedbackListItem, FeedbackItem } from "../models/feedback.model";

@Injectable({ providedIn: "root" })
export class FeedbackService {
  constructor(private http: HttpClient) {}

  submit(rating: number, comment: string): Observable<ApiResponse<FeedbackItem>> {
    return this.http.post<ApiResponse<FeedbackItem>>(`${API_BASE_URL}/feedback`, { rating, comment });
  }

  getMine(): Observable<ApiResponse<FeedbackItem | null>> {
    return this.http.get<ApiResponse<FeedbackItem | null>>(`${API_BASE_URL}/feedback/mine`);
  }

  getAllForAdmin(): Observable<ApiResponse<AdminFeedbackListItem[]>> {
    return this.http.get<ApiResponse<AdminFeedbackListItem[]>>(`${API_BASE_URL}/feedback`);
  }
}

