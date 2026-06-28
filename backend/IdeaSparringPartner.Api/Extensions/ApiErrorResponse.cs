namespace IdeaSparringPartner.Api.Extensions;

public static class ApiErrorResponse
{
    public static object Create(string message) => new { error = message, message };

    public static class Messages
    {
        public const string Unauthorized = "Authentication required. Please log in again.";
        public const string SessionExpired = "Your session has expired. Please log in again.";
        public const string InvalidToken = "Invalid or expired access token. Please log in again.";
        public const string Forbidden = "You do not have permission to access this resource.";
        public const string IdeaNotFound = "Idea not found or you do not have access to it.";
        public const string ThreadNotFound = "Thread not found or you do not have access to it.";
        public const string MemoryNotFound = "Memory not found or already deleted.";
        public const string Unexpected = "An unexpected error occurred. Please try again.";
    }
}
