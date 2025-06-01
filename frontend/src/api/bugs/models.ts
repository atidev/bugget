export interface CreateBugRequest {
    receive?: string | null;
    expect?: string | null;
}

export interface UpdateBugRequest {
    receive?: string;
    expect?: string;
    status?: number;
}

export interface BugResponse {
    id: number;
    receive: string | null;
    expect: string | null;
    createdAt: string;
    updatedAt: string;
    creatorUserId: string;
    status: number;
}

export interface PatchBugResponse {
    id: number;
    receive: string | null;
    expect: string | null;
    updatedAt: string;
    status: number;
}