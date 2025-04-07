import React, { useState, useRef, useEffect } from "react";
import { useStoreMap } from "effector-react";
import {
  addCommentFx,
  $commentsByBugId,
} from "../../../../../../store/comments";
import { SendHorizonal } from "lucide-react";

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

  const parseMessage = (text: string) => {
    const urlRegex = /(https?:\/\/[^\s]+|www\.[^\s]+)/g;
  
    return text.split(urlRegex).map((part, index) => {
      if (part.match(urlRegex)) {
        const href = part.startsWith("http") ? part : `https://${part}`;
        return (
          <a 
            key={index} 
            href={href} 
            target="_blank" 
            rel="noopener noreferrer"
            className="text-blue-500 underline"
          >
            {part}
          </a>
        );
      }
      return part; // Оставляем обычный текст
    });
  };

  return (
    <div className="flex flex-col w-full h-full">
      <div className="flex-1 overflow-auto mb-4 space-y-2">
        {comments.map((comment, index) => (
          <div
            key={index}
            className="border bg-base-100 border-gray-300 p-2 rounded flex flex-col"
          >
            <div className="font-semibold text-sm">{comment.creator?.name}</div>
            <div className="text-sm whitespace-pre-wrap">{parseMessage(comment.text)}</div>
          </div>
        ))}
      </div>

      <div className="flex items-end space-x-2">
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
          className="btn btn-info text-white px-4 py-1 rounded disabled:opacity-50 flex items-center justify-center h-[32px]"
        >
          <SendHorizonal className="size-6" />
        </button>
      </div>
    </div>
  );
};
