using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace shop.Extension
{

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                switch (context.Response.StatusCode)
                {
                    case 400:
                    case 401:
                    case 403:
                        //{
                        //    await Handle403To401(context);
                        //    break;
                        //}
                    case 404:
                    case 409:
                    case 405:// methode send error =>> post get put delete 
                    case 422:
                    case 500:
                        {
                            await HandleExceptionAsync(context);
                            break;
                        }
                    default:
                        {
                            break;

                        }
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 400;
                await HandleExceptionAsync(context, ex.Message);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, string message = "")
        {
            int code = context.Response.StatusCode;
            var result = JsonConvert.SerializeObject(new { code = code, message = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
        private static Task Handle403To401(HttpContext context, string message = "")
        {
            int code = 401;
            var result = JsonConvert.SerializeObject(new { code = code, message = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        //public async Task Invoke(HttpContext context)
        //{
        //    try
        //    {
        //        await _next(context);
        //    }
        //    catch (Exception ex)
        //    {
        //        await HandleExceptionAsync(context, ex);
        //    }
        //}
        //private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        //{
        //    int code = context.Response.StatusCode;
        //    var result = JsonConvert.SerializeObject(new { code = code, error = ex.Message });
        //    context.Response.ContentType = "application/json";
        //    context.Response.StatusCode = (int)code;
        //    return context.Response.WriteAsync(result);
        //}
    }



}