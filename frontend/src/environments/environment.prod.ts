export const environment = {
  production: true,
  apiBaseUrl: 'https://idea-sparring-partner-api.onrender.com/api',
  // Ping Render before 15 min idle spin-down (13 min = safe margin)
  keepAliveIntervalMs: 13 * 60 * 1000
};