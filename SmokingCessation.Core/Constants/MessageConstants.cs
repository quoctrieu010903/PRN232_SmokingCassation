

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
        // Payment message
        // Payment & Membership messages
        public const string PAYMENT_REQUIRED = "You must complete payment to use this feature.";
        public const string PAYMENT_SUCCESS = "Payment completed successfully!";
        public const string PAYMENT_FAILED = "Payment failed.";
        public const string ACTIVE_PACKAGE_EXISTS = "You already have an active package.";
        public const string PACKAGE_NOT_FOUND = "Membership package not found.";
        public const string REGISTER_PACKAGE_SUCCESS = "Package registration successful.";
        public const string CANCEL_PACKAGE_SUCCESS = "Membership cancelled successfully.";
        public const string NO_ACTIVE_MEMBERSHIP = "No active membership found.";
        public const string ALREADY_RATED = "You have already rated this blog.";
        public const string INVALID_RATING_VALUE = "Rating value must be between 1 and 5.";


    }
}
