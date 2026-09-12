export interface PaymentResult {
    success: boolean;
    planName: string;
    mockGatewayReference: string;
    currentPlan: string;
  }
  
  export interface BillingStatus {
    currentPlan: string;
    storageUsedBytes: number;
    storageLimitBytes: number;
  }
  
  export interface PaymentHistoryItem {
    id: number;
    planName: string;
    amountCents: number;
    currency: string;
    status: string;
    mockGatewayReference: string;
    createdAt: string;
  }
  