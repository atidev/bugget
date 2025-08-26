import { MessageCircle } from "lucide-react";
import { useStoreMap } from "effector-react";
import { $commentsByBugId } from "@/store/comments";
import Comment from "./components/Comment/Comment";
import NewCommentForm from "./components/NewCommentForm/NewCommentForm";

type Props = {
  reportId: number;
  bugId: number;
};

const Comments = ({ reportId, bugId }: Props) => {
  const comments = useStoreMap({
    store: $commentsByBugId,
    keys: [bugId],
    fn: (state, [id]) => state[id] || [],
  });

  return (
    <div className="bg-base-100 border border-base-300 rounded-lg p-2 col-span-2">
      {!!comments.length && (
        <div className="flex items-center gap-2 mb-2 p-2 bg-base-200 rounded-lg w-fit">
          <div className="w-5 h-5 rounded-full bg-info/20 flex items-center justify-center">
            <MessageCircle className="w-3 h-3 text-info" />
          </div>
          <span className="text-sm font-medium text-base-content">
            {comments.length}{" "}
            {comments.length === 1
              ? "комментарий"
              : comments.length < 5
              ? "комментария"
              : "комментариев"}
          </span>
        </div>
      )}
      {!!comments.length && (
        <div className="space-y-2 mb-2">
          {comments.map((comment) => (
            <Comment
              key={comment.id}
              reportId={reportId}
              bugId={bugId}
              id={comment.id}
              text={comment.text}
              creatorUserId={comment.creatorUserId}
              createdAt={comment.createdAt}
              attachments={comment.attachments}
            />
          ))}
        </div>
      )}

      <NewCommentForm reportId={reportId} bugId={bugId} />
    </div>
  );
};

export default Comments;
