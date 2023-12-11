namespace NexusServergRPC.Services;
using Grpc.Core;
using NexusServergRPC.Models;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NexusServergRPC.Server;
using NexusServergRPC.RequestProcessing;
using Google.Protobuf;

public class NexusService : Nexus.NexusBase
{
    private static Nexus_Server? _server;

    public NexusService(Nexus_Server server)
    {
        _server = server;
    }

    public static Nexus_Server? Server { get { return _server; } }
    
    public override Task<CreateMsgResponse> CreateMsg(CreateMsgRequest request, ServerCallContext context)
    {
        return Task.FromResult(new CreateMsgResponse
        {
            Content = request.MsgText,
            ExtraContent = request.ExtraContent,
            ReceiverEmail = request.ReceiverEmail,
            ReceiverName = request.ReceiverName,
        });
    }

    public override async Task<EmptyX> TestConnection(EmptyX request, ServerCallContext context)
    {
        await Console.Out.WriteLineAsync("Count = " + _server.users.Count);
        return await Task.FromResult(new EmptyX());
    }


    public override async Task Connect(IAsyncStreamReader<ConnectedX> requestStream, IServerStreamWriter<CreateMsgRequest> responseStream, ServerCallContext context)
    {
        try
        {
            var metadata = context.RequestHeaders;
            var user = ObjectCaster<ConnectedX, CreateMsgRequest>.Cast(_server).Find(u => u.GetEmail == metadata.Get("UserEmail")?.Value);
            if (user == null)
            {
                User<ConnectedX, CreateMsgRequest> New_User = new User<ConnectedX, CreateMsgRequest>(metadata.Get("UserName")?.Value, metadata.Get("UserEmail")?.Value, requestStream, responseStream);

                _server.users.Add(New_User);

                Console.WriteLine("Connected " + ObjectCaster<ConnectedX, CreateMsgRequest>.Cast(_server).Count);
                Console.WriteLine("NotConnected " + ObjectCaster<LoginUserRequest, LoginUserResponse>.Cast(_server).Count);
        
                await responseStream.WriteAsync(new CreateMsgRequest { ExtraContent = ByteString.CopyFrom(new byte[16]), MsgText = "UWU", ReceiverEmail = "UWU", ReceiverName = "UWU" });

                await ProcessRequest<ConnectedX, CreateMsgRequest>.ProcessStreamAsync(requestStream, New_User);

            }
        }
        catch (Exception)
        {
        }
    }


    public override async Task Broadcast(IAsyncStreamReader<MessageRequest> requestStream, IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext())
        {
            var message = requestStream.Current.Message;

            await responseStream.WriteAsync(new MessageResponse { Message = $"Broadcasting: {message}" });
        }
    }

    public override async Task<EmptyX> SendMessageToStreams(CreateMsgRequest message, ServerCallContext context)
    {
        await Console.Out.WriteLineAsync("Count = " + _server.users.Count);
        foreach (var user in ObjectCaster<ConnectedX, CreateMsgRequest>.Cast(_server))
        {
            await Console.Out.WriteLineAsync(user.response_stream + "");
            try
            {
                await user.response_stream.WriteAsync(message);
                await Console.Out.WriteLineAsync(message.ReceiverEmail + " X");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
        return await Task.FromResult(new EmptyX());
    }

}