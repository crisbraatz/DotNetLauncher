using Infrastructure.PostgreSql;

namespace Presentation.Middlewares;

public class UnitOfWorkMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
    {
        try
        {
            await next(context);
            await unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}