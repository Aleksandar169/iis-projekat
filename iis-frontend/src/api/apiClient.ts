const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5227/api';

async function readErrorMessage(response: Response): Promise<string> {
    try {
        const data = await response.json();
        if (typeof data?.message === 'string') return data.message;
        return `HTTP ${response.status}`;
    } catch {
        return `HTTP ${response.status}`;
    }
}

export async function apiRequest<T>(
    path: string,
    options?: RequestInit,
): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${path}`, {
        headers: {
            'Content-Type': 'application/json',
            ...(options?.headers ?? {}),
        },
        ...options,
    });

    if (!response.ok) {
        throw new Error(await readErrorMessage(response));
    }

    const text = await response.text();

    if (!text) {
        return undefined as T;
    }

    return JSON.parse(text) as T;
}