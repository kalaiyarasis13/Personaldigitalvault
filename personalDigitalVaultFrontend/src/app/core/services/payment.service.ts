import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { API_BASE_URL } from "./environment";
import { ApiResponse } from "../models/api-response.model";
import { BillingStatus, PaymentHistoryItem, PaymentResult } from "../models/payment.model";


@Injectable({ providedIn: "root" })
export class PaymentService {
  constructor(private http: HttpClient) {}

  checkout(planName: string): Observable<ApiResponse<PaymentResult>> {
    return this.http.post<ApiResponse<PaymentResult>>(`${API_BASE_URL}/payments/checkout`, { planName });
  }

  getBillingStatus(): Observable<ApiResponse<BillingStatus>> {
    return this.http.get<ApiResponse<BillingStatus>>(`${API_BASE_URL}/payments/billing-status`);
  }

  getHistory(): Observable<ApiResponse<PaymentHistoryItem[]>> {
    return this.http.get<ApiResponse<PaymentHistoryItem[]>>(`${API_BASE_URL}/payments/history`);
  }
}
