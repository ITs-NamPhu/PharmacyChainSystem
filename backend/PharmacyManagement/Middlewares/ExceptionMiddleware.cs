using PharmacyManagement.Exceptions;

namespace PharmacyManagement.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = ex.StatusCode;

                    await context.Response.WriteAsJsonAsync(
                        new
                        {
                            statusCode = ex.StatusCode,
                            errorCode = ex.ErrorCode,
                            EC = -1,
                            EM = ex.Message,
                            data = (object?)null
                        });
                }
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                {
                    await context.Response.WriteAsJsonAsync(
                        new
                        {
                            statusCode = 500,
                            errorCode = "SYSTEM001",
                            message = "Internal server error.",
                            data = (object?)null
                        });
                }
            }
        }
    }
}
