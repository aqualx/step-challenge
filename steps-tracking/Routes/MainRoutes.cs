using System.Text;

public static class MainRoutes
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/", () =>
        {
            string htmlContent = @"
                <!DOCTYPE html>
                <html>
                    <head>
                        <title>Steps Challenge</title>
                    </head>
                    <body>
                        <h1>Go to <a href='/scalar/v1'>OpenAPI</a> documentation</h1>
                    </body>
                </html>
            ";

            return Results.Text(htmlContent, "text/html", Encoding.UTF8);
        });
    }
}