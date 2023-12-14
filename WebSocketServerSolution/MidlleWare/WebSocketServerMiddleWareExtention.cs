namespace WebSocketServerSolution.MidlleWare
{
    public static class WebSocketServerMiddleWareExtention
    {
        public static IApplicationBuilder UseWebSocketMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketServerMiddleWare>();
        }
        public static IServiceCollection AddWebSocketMiddleWare(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketServerConnectionManager>();
            return services;
        }
    }
}
