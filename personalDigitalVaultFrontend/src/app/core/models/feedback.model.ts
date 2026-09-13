export interface FeedbackItem  {
    id : number;
    rating: number;
    comment?: string| null;
    createdAt: string;
}
export interface AdminFeedbackListItem{
    id: number;
    username: string;
    rating: number;
    comment: string | null;
    createdAt: string;
}
