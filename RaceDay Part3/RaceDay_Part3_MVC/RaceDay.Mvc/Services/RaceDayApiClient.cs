using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace RaceDay.Mvc.Services;
public sealed class RaceDayApiClient(HttpClient http, SessionAuth auth)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public async Task<JsonElement> Get(string path, CancellationToken ct=default) => await Send(HttpMethod.Get,path,null,ct);
    public async Task<JsonElement> Post(string path, object body, CancellationToken ct=default) => await Send(HttpMethod.Post,path,JsonContent(body),ct);
    public async Task<JsonElement> Put(string path, object body, CancellationToken ct=default) => await Send(HttpMethod.Put,path,JsonContent(body),ct);
    public async Task<JsonElement> Delete(string path, CancellationToken ct=default) => await Send(HttpMethod.Delete,path,null,ct);
    public async Task<JsonElement> Upload(string path, IFormFile file, CancellationToken ct=default)
    {
        using var form = new MultipartFormDataContent();
        await using var stream=file.OpenReadStream();
        using var fileContent=new StreamContent(stream);
        fileContent.Headers.ContentType=new MediaTypeHeaderValue(file.ContentType);
        form.Add(fileContent,"file",Path.GetFileName(file.FileName));
        return await Send(HttpMethod.Post,path,form,ct);
    }
    private static StringContent JsonContent(object body) => new(JsonSerializer.Serialize(body,JsonOptions),Encoding.UTF8,"application/json");
    private async Task<JsonElement> Send(HttpMethod method,string path,HttpContent? content,CancellationToken ct)
    {
        using var request=new HttpRequestMessage(method,path){Content=content};
        if(!string.IsNullOrWhiteSpace(auth.Token)) request.Headers.Authorization=new AuthenticationHeaderValue("Bearer",auth.Token);
        using var response=await http.SendAsync(request,ct);
        var raw=await response.Content.ReadAsStringAsync(ct);
        if(!response.IsSuccessStatusCode) throw new ApiException(response.StatusCode, string.IsNullOrWhiteSpace(raw) ? "The API request could not be completed." : raw);
        if(string.IsNullOrWhiteSpace(raw)) return JsonDocument.Parse("{}").RootElement.Clone();
        using var doc=JsonDocument.Parse(raw); return doc.RootElement.Clone();
    }
}
public sealed class ApiException(HttpStatusCode status,string message):Exception(message){public HttpStatusCode Status=>status;}
