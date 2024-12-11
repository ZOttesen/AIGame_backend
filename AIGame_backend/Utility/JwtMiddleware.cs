using AIGame_backend.Services;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtService _jwtService;

    public JwtMiddleware(RequestDelegate next, JwtService jwtService)
    {
        _next = next;
        _jwtService = jwtService;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Hent authToken fra cookien
            var token = context.Request.Cookies["authToken"];
            if (!string.IsNullOrEmpty(token))
            {
                // Valider token og hent claims
                var claimsPrincipal = _jwtService.ValidateToken(token);
                if (claimsPrincipal != null)
                {
                    // Sæt claims i HttpContext.Items
                    context.Items["User"] = claimsPrincipal;
                    Console.WriteLine("Token validated successfully.");
                }
                else
                {
                    Console.WriteLine("Token validation failed.");
                }
            }
            else
            {
                Console.WriteLine("No authToken cookie found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in JWT validation: {ex.Message}");
        }

        await _next(context);
    }
}