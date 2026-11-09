using System.Text.Json;
namespace RaceDay.Mvc.Services;
public sealed class SessionAuth(IHttpContextAccessor accessor)
{
    private ISession Session => accessor.HttpContext!.Session;
    public bool SignedIn => !string.IsNullOrWhiteSpace(Session.GetString("AccessToken"));
    public string? Token => Session.GetString("AccessToken");
    public string? Role => Session.GetString("Role");
    public string? Name => Session.GetString("Name");
    public void SignIn(JsonElement payload, string email)
    {
        // Supports common API response shapes: {token, role, name} or {accessToken, user:{...}}.
        var token = Read(payload, "token") ?? Read(payload, "accessToken") ?? Read(payload, "jwt");
        if (string.IsNullOrWhiteSpace(token)) throw new InvalidOperationException("The API login response did not contain a token.");
        var user = payload.TryGetProperty("user", out var u) ? u : payload;
        Session.SetString("AccessToken", token);
        Session.SetString("Role", Read(user, "role") ?? Read(user, "userRole") ?? "Participant");
        Session.SetString("Name", Read(user, "name") ?? Read(user, "fullName") ?? email);
        Session.SetString("Email", Read(user, "email") ?? email);
    }
    public void SignOut() => Session.Clear();
    private static string? Read(JsonElement e, string name) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var v) ? v.ToString() : null;
}
