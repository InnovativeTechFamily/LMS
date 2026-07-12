/** Base URL of the .NET server (server-dotnet), including the /api/v1 prefix. */
export const API_URL =
  process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:8000/api/v1";

/** localStorage key holding the JWT access token returned by the API. */
export const TOKEN_KEY = "lms_access_token";
