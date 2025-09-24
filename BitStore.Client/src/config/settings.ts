export const APP_SETTINGS = {
  orderBook: {
    aggregationCount: 10
  }
} as const;

export type AppSettings = typeof APP_SETTINGS;
