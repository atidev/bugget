export interface CreateCommentRequest {
  text: string; // required, min length 1, max length 512
}

export interface UpdateCommentRequest {
  text: string; // required, min length 1, max length 512
}

export interface CommentResponse {
  id: number;
  bug_id: number;
  text: string;
  creator_user_id: string;
  created_at: string;
  updated_at: string;
}
