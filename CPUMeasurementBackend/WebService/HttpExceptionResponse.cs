﻿

//using CPUMeasurementBackend.WebService.Account;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Net;
//using System.Threading.Tasks;

//namespace CPUMeasurementBackend.WebService
//{
//    public static class ExceptionHandling
//    {
//        internal static IHttpContextAccessor ResponseContext;

//        public static void AddErrorHandling(this IServiceCollection services)
//        {
//            services.AddHttpContextAccessor();
//            services.AddScoped<HttpExceptionResponse>();
            
//        }
//    }

//    ic class HttpExceptionResponse : Exception
//    {
//        public HttpStatusCode StatusCode {get; set; }
//        public string Value { get; set; }
//        public string Field { get; set; }
//        public readonly IHttpContextAccessor _httpContext;

//        public HttpExceptionResponse(string message, HttpStatusCode statusCode, HttpConfig httpContext) : base(message)
//        {
//            this._httpContext = httpContext;
//            this.StatusCode = statusCode;
//            ExceptionHandling.ResponseContext.Response.StatusCode = (int)statusCode;
//        }

        
//    }

//    public class NotFoundException : HttpExceptionResponse
//    {
//        public NotFoundException(string message, string field, object value) : base(message, HttpStatusCode.NotFound)
//        {
//            this.Field = field;
//            this.Value = value.ToString();
//        }

//        public override string ToString()
//        {
//            return "{\"error\": 1237}";//JObject.FromObject(new { error = this.Message, field = Field, Value = Value  }).ToString();

//        }
//    }
//}
