using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OrderCreationForm.Startup))]
namespace OrderCreationForm
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
