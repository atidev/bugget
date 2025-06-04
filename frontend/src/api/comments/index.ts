import axios from "../axios";
import {
  CreateCommentRequest,
  UpdateCommentRequest,
  CommentResponse,
} from "./models";

export const createComment = async (
  reportId: number,
  bugId: number,
  request: CreateCommentRequest
): Promise<CommentResponse> => {
  const { data } = await axios.post<CommentResponse>(
    `/v2/reports/${reportId}/bugs/${bugId}/comments`,
    request
  );
  return data;
};

export const updateComment = async (
  reportId: number,
  bugId: number,
  commentId: number,
  request: UpdateCommentRequest
): Promise<CommentResponse> => {
  const { data } = await axios.put<CommentResponse>(
    `/v2/reports/${reportId}/bugs/${bugId}/comments/${commentId}`,
    request
  );
  return data;
};

export const deleteComment = async (
  reportId: number,
  bugId: number,
  commentId: number
): Promise<void> => {
  await axios.delete(
    `/v2/reports/${reportId}/bugs/${bugId}/comments/${commentId}`
  );
};
