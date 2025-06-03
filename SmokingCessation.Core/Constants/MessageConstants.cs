

namespace SmokingCessation.Core.Constants
{
    public static class MessageConstants
    {
        // Success messages
        public const string CREATE_SUCCESS = "Create successfully!";
        public const string UPDATE_SUCCESS = "Update successfully!";
        public const string DELETE_SUCCESS = "Delete successfully!";
        public const string GET_SUCCESS = "Get successfully!";
        public const string GETALL_SUCCESS = "Get all successfully!";
        public const string PROCESS_SUCCESS = "Process completed successfully!";

        // Error messages
        public const string NOT_FOUND = "Data not found.";
        public const string CREATE_FAILED = "Create failed.";
        public const string UPDATE_FAILED = "Update failed.";
        public const string DELETE_FAILED = "Delete failed.";
        public const string INVALID_DATA = "Invalid data.";
        public const string UNAUTHORIZED = "Unauthorized access.";
        public const string FORBIDDEN = "Forbidden action.";
        public const string INTERNAL_ERROR = "An unexpected error occurred.";

        // Validation messages
        public const string REQUIRED_FIELD = "This field is required.";
        public const string INVALID_FORMAT = "Invalid format.";
    }
}
