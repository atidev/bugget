import React, { useState, useRef, useEffect } from "react";
import { useStoreMap } from "effector-react";
import {
  addCommentFx,
  $commentsByBugId,
} from "../../../../../../store/comments";

type BugChatProps = {
  reportId: number;
  bugId: number;
};

export const Chat = ({ reportId, bugId }: BugChatProps) => {
  const [newCommentText, setNewCommentText] = useState("");
  const [isAdding, setIsAdding] = useState(false);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const comments = useStoreMap({
    store: $commentsByBugId,
    keys: [bugId],
    fn: (state, [id]) => state[id] ?? [],
  });

  const handleAddComment = () => {
    if (!newCommentText.trim() || isAdding) return;
    setIsAdding(true);
    addCommentFx({ reportId, bugId, text: newCommentText }).finally(() => {
      setIsAdding(false);
      setNewCommentText("");
    });
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault(); // Предотвращаем перенос строки
      handleAddComment();
    }
  };

  // Автоматическая подстройка высоты textarea
  useEffect(() => {
    if (textareaRef.current) {
      textareaRef.current.style.height = "auto";
      textareaRef.current.style.height = `${textareaRef.current.scrollHeight}px`;
    }
  }, [newCommentText]);

  return (
    <div className="flex flex-col w-full h-full">
      <div className="text-xl font-bold mb-4">Комментарии</div>

      <div className="flex-1 overflow-auto mb-4 space-y-2">
        {comments.map((comment, index) => (
          <div
            key={index}
            className="border bg-base-100 border-gray-300 p-2 rounded flex flex-col"
          >
            <div className="font-semibold text-sm">{comment.creator?.name}</div>
            <div className="text-sm whitespace-pre-wrap">{comment.text}</div>
          </div>
        ))}
      </div>

      <div className="flex items-center space-x-2">
        <textarea
          ref={textareaRef}
          value={newCommentText}
          onChange={(e) => setNewCommentText(e.target.value)}
          onKeyDown={handleKeyDown}
          className="flex-1 border border-gray-300 rounded px-2 py-1 bg-base-100 resize-none overflow-hidden"
          placeholder="Напишите комментарий..."
          rows={1}
        />
        <button
          onClick={handleAddComment}
          disabled={isAdding}
          className="btn btn-info text-white px-4 py-1 rounded disabled:opacity-50 flex items-center justify-center"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth="1.5"
            stroke="currentColor"
            className="size-6"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M6 12 3.269 3.125A59.769 59.769 0 0 1 21.485 12 59.768 59.768 0 0 1 3.27 20.875L5.999 12Zm0 0h7.5"
            />
          </svg>
        </button>
      </div>
    </div>
  );
};
