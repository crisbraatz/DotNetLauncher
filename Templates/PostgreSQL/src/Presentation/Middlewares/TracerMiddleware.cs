using Application.Observability;

namespace Presentation.Middlewares;

public class TracerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        using (Tracer.Instance.StartActivity())
            await next(context);
    }
}