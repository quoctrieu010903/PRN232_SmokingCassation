

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
        // Blog messages
        public const string BLOG_APPROVE_SUCCESS = "Blog approved successfully!";
        public const string BLOG_REJECT_SUCCESS = "Blog rejected successfully!";
        public const string BLOG_CREATE_PENDING_APPROVAL = "Blog created successfully and is pending approval.";
        public const string BLOG_ALREADY_APPROVED = "Blog has already been approved.";
        public const string BLOG_ALREADY_REJECTED = "Blog has already been rejected.";
        public const string BLOG_NOT_FOUND = "Blog not found.";
        public const string BLOG_DELETE_SUCCESS = "Blog deleted successfully.";
        public const string BLOG_UPDATE_SUCCESS = "Blog updated successfully.";
        public const string BLOG_CREATE_SUCCESS = "Blog created successfully.";
        public const string BLOG_STATUS_UPDATED = "Blog status updated successfully.";
        public const string BLOG_UNAUTHORIZED = "You are not authorized to modify this blog.";
        public const string BLOG_FORBIDDEN = "You do not have permission to perform this action on the blog.";
        public const string BLOG_INVALID_STATUS = "Invalid blog status.";
        public const string BLOG_ALREADY_EXISTS = "A blog with the same title already exists.";
        public const string BLOG_VIEWCOUNT_INCREASED = "Blog view count increased.";



    }
}
