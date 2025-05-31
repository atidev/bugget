/**
 * Экспоненциальный бэк-офф с jitter.
 * Попытки будут: 0ms, 2s, 5s, 10s, 30s.
 * При успехе resolve-ит, при фатальной ошибке — выбрасывает ошибку.
 */
export async function backoff<T>(
    fn: () => Promise<T>,
    retries = [0, 2000, 5000, 10000, 30000]
  ): Promise<T> {
    let attempt = 0;
    while (true) {
      try {
        return await fn();
      } catch (err) {
        if (attempt >= retries.length - 1) {
          throw err;
        }
        const delay = retries[attempt];
        await sleep(delayWithJitter(delay));
        attempt++;
      }
    }
  }
    
  function delayWithJitter(ms: number): number {
    const jitter = Math.random() * (ms * 0.2); // ±20% от задержки
    return ms + jitter - ms * 0.1;
  }
  
  function sleep(ms: number): Promise<void> {
    return new Promise((res) => setTimeout(res, ms));
  }