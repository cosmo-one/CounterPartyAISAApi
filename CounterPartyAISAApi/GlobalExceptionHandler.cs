using CosmoOne.AADEPortal.API.Dtos.Functionality;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

namespace AADEPortalApi
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
                                                    Exception exception, 
                                                    CancellationToken cancellationToken)
        {

            Log.Error(exception, "An error Ocurred");

            var type = exception.GetType().Name;



            var errorResponse = new ApiResponse();

            errorResponse.IsSuccess = false;
            

       
            var errorMessage = exception.Message;

            if(exception.InnerException != null)
            {
                errorMessage += exception.InnerException.Message;
            }

    
            errorResponse.Data = null;
            errorResponse.ErrorMessage = errorMessage;



            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var response = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Optional: use camel case naming convention
            });

            await httpContext.Response.WriteAsync(response, cancellationToken);

            return true;

          
        }
    }
}
