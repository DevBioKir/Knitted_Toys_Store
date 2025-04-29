using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Knitted_Toys_Store.Infrastructure.Middleware
{
    // Класс обработчика базовой аутентификации, наследуется от AuthenticationHandler
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        // Конструктор класса, принимает необходимые зависимости
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,  // Опции схемы аутентификации
            ILoggerFactory logger,                    
            UrlEncoder encoder,                       // Кодировщик URL
            ISystemClock clock) : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() // Переопределенный метод, который вызывается при каждом запросе к API для проверки аутентификации
        {
            //проверка наличия заголовка Authorization
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.Fail("Отсутствует заголовок Authorization"));

            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult(AuthenticateResult.Fail("Неверный тип авторизации"));

                var token = authHeader.Substring("Basic ".Length).Trim();
                var credentialBytes = Convert.FromBase64String(token);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length != 2)
                    return Task.FromResult(AuthenticateResult.Fail("Некорректный формат заголовка"));

                var (username, password) = (credentials[0], credentials[1]);

                if (username != "admin" || password != "admin")
                    return Task.FromResult(AuthenticateResult.Fail("Неверный логин или пароль"));

                //если все ок
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Administrator") //роль
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Ошибка обработки заголовка Authorization"));
            }
        }
    }
}
