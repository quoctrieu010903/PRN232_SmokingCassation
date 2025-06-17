using Microsoft.AspNetCore.Mvc.Filters;

namespace SmokingCessation.WebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BypassJwtMiddlewareAttribute : Attribute, IFilterMetadata
    {
    }
} 