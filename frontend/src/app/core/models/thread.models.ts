export interface Message {
  id: string;
  threadId: string;
  role: string;
  content: string;
  createdAt: string;
}

export interface ThreadItem {
  id: string;
  ideaId: string;
  persona: string;
  status: string;
  createdAt: string;
}

export interface PostMessageResponse {
  userMessage: Message;
  assistantMessage: Message;
  extractedMemory: unknown | null;
}
