﻿using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace SmokingCessation.Core.CustomExceptionss
{
    public class CoreException : Exception
    {
        public CoreException(string code, string message = "", int statusCode = StatusCodes.Status500InternalServerError)
            : base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }


        public string Code { get; }

        public int StatusCode { get; set; }

        /*[Newtonsoft.Json.JsonExtensionData]*/
        public Dictionary<string, object> AdditionalData { get; set; }

    }
    public class ErrorException : Exception
    {
        public int StatusCode { get; }

        public ErrorDetail ErrorDetail { get; }

        public ErrorException(int statusCode, string errorCode, string message = null)
        {
            StatusCode = statusCode;
            ErrorDetail = new ErrorDetail
            {
                ErrorCode = errorCode,
                ErrorMessage = message
            };
        }

        public ErrorException(int statusCode, ErrorDetail errorDetail)
        {
            StatusCode = statusCode;
            ErrorDetail = errorDetail;
        }
    }

    public class ErrorDetail
    {
        [JsonPropertyName("errorCode")] public string ErrorCode { get; set; }

        [JsonPropertyName("errorMessage")] public object ErrorMessage { get; set; }
    }
}
