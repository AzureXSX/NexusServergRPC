namespace NexusServergRPC.Models;
using Grpc.Core;


public class User<Trequest, Tresponse>
{
    private string? UserName { get; set; }
    public string? GetName { get { return UserName; } }
    private string? UserEmail { get; set; }
    public string? GetEmail { get { return UserEmail; } }

    public IAsyncStreamReader<Trequest> request_stream;
    public IServerStreamWriter<Tresponse> response_stream;

    public Task? Task { get; set; }

    public User(string? userName, string? userEmail, IAsyncStreamReader<Trequest> request_stream, IServerStreamWriter<Tresponse> response_stream)
    {
        UserName = userName;
        UserEmail = userEmail;
        this.request_stream = request_stream;
        this.response_stream = response_stream;
    }

    public async Task SetTask(Task task)
    {
        Task = task;
        await Task;
    }

    public async void Terminate()
    {
        await Console.Out.WriteLineAsync("DISPOSED");
        Task = null;
    }

    public User() { }
}