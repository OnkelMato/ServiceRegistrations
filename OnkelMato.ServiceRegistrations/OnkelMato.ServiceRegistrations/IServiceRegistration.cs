namespace OnkelMato.ServiceRegistrations;

public interface IServiceRegistration : IRegistration
{
    public WebApplicationBuilder Configure(WebApplicationBuilder builder);
}