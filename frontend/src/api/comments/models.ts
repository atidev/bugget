export type CreateCommentRequest = {
  text: string; // required, min length 1, max length 2048
};

export type UpdateCommentRequest = {
  text: string; // required, min length 1, max length 2048
};

export type CommentResponse = {
  id: number;
  bug_id: number;
  text: string;
  creator_user_id: string;
  created_at: string;
  updated_at: string;
};
