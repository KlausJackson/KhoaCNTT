
using KhoaCNTT.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KhoaCNTT.API.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Instance = context.HttpContext.Request.Path
            };

            // 1. Xử lý lỗi Không tìm thấy (404)
            if (context.Exception is NotFoundException notFoundEx)
            {
                details.Title = "Không tìm thấy tài nguyên";
                details.Status = StatusCodes.Status404NotFound;
                details.Detail = notFoundEx.Message;
                context.Result = new NotFoundObjectResult(details);
            }
            // 2. Xử lý lỗi Nghiệp vụ (400)
            else if (context.Exception is BusinessRuleException businessEx)
            {
                details.Title = "Lỗi nghiệp vụ";
                details.Status = StatusCodes.Status400BadRequest;
                details.Detail = businessEx.Message;
                context.Result = new BadRequestObjectResult(details);
            }
            // 3. Xử lý lỗi chưa xác định (500)
            else
            {
                details.Title = "Lỗi hệ thống";
                details.Status = StatusCodes.Status500InternalServerError;
                details.Detail = context.Exception.Message; // hiện message
                context.Result = new ObjectResult(details)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            context.ExceptionHandled = true;
        }
    }
}