using Microsoft.AspNetCore.Mvc.Filters;

namespace MarkdownWebApp.Api.Filters;

public class DocumentAccessFilter: Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        
    }
}