using Fun.Application.Fun.IServices;


namespace Urb.Plan.v2.Urb.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        IUserService _service;

        public JwtMiddleware(RequestDelegate requestDelegate, IUserService service)
        {
            _service = service;
            _requestDelegate = requestDelegate;
        }
    }
}
