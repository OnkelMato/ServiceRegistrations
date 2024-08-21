namespace OnkelMato.ServiceRegistrations;

public interface IMiddlewareRegistration : IRegistration
{
    public WebApplication Use(WebApplication app);
}