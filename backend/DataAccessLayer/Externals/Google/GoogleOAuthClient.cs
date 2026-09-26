using DataAccessLayer.Constants.Exceptions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using static Google.Apis.Auth.GoogleJsonWebSignature;
namespace DataAccessLayer.Externals.Google;
public sealed record GoogleTokenResult(string Subject, string Email, string AccessToken, string? RefreshToken, DateTime ExpiresAt);

public interface IGoogleOAuthClient
{
    Task<GoogleTokenResult> ExchangeCodeAsync(string code, CancellationToken ct = default);
}

public sealed class GoogleOAuthClient : IGoogleOAuthClient
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _cfg;
    public GoogleOAuthClient(IHttpClientFactory httpFactory, IConfiguration cfg)
    {
        _httpFactory = httpFactory;
        _cfg = cfg;
    }

    public async Task<GoogleTokenResult> ExchangeCodeAsync(string code, CancellationToken ct = default)
    {
        using var res = await _httpFactory.CreateClient().PostAsync("https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = _cfg["Authentication:Google:ClientId"]!,
                ["client_secret"] = _cfg["Authentication:Google:ClientSecret"]!,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = "postmessage" 
            }), ct);
        if (!res.IsSuccessStatusCode)
            throw new AppException(StatusCodes.Status401Unauthorized, "Invalid Google code");
        var json = await res.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        Payload payload;
        try {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                json.GetProperty("id_token").GetString(),
                new ValidationSettings { Audience = [_cfg["Authentication:Google:ClientId"]!] });
        } catch (InvalidJwtException) {
            throw new AppException(StatusCodes.Status401Unauthorized, "Invalid Google token");
        } return new GoogleTokenResult(
            payload.Subject,
            payload.Email,
            json.GetProperty("access_token").GetString()!,
            json.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null,
            DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32()));
    }
}
