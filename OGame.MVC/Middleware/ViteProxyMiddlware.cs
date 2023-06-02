using Yarp.ReverseProxy.Model;

public class ViteProxyMiddlware
{
    private readonly RequestDelegate _next;

    public ViteProxyMiddlware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IReverseProxyFeature reverseProxy)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            // Use YARP for WebSocket requests.
            reverseProxy.ProxiedDestination = new DestinationState("vite");
            await _next(context);
        }
        else
        {
            // Use next middleware for non-WebSocket requests.
            await _next(context);
        }
    }
}
