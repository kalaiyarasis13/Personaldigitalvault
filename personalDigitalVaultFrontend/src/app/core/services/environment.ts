// Auto-detects which mode the app is running in:
// - Standalone dev mode (ng serve on :4200) -> talk to the separate API port.
// - Integrated deployment (Angular build copied into wwwroot, served by the API itself)
//   -> same-origin, so a relative "/api" path works without any config change.
export const API_BASE_URL = window.location.port === "4200" ? "http://localhost:5000/api" : "/api";
