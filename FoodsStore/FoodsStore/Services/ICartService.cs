using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FoodsStore.Services
{
    public interface ICartService
    {
        Task<int> GetCartCountAsync(ClaimsPrincipal user, HttpContext httpContext);
    }
}