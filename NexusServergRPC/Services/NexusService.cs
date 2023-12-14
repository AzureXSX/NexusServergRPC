namespace NexusServergRPC.Services;
using Grpc.Core;
using NexusServergRPC.Models;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NexusServergRPC.Server;
using NexusServergRPC.RequestProcessing;
using Google.Protobuf;
using NexusContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

public class NexusService : Nexus.NexusBase
{
    private static Nexus_Server? _server;
    private readonly NexusContext _context;

    public NexusService(Nexus_Server server, NexusContext context)
    {
        _server = server;
        _context = context;
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
        foreach (var user in ObjectCaster<LoginUserRequest, LoginUserResponse>.Cast(_server))
        {
            await Console.Out.WriteLineAsync(user.response_stream + "");
            try
            {
                MessageX x = new MessageX { };
                await user.response_stream.WriteAsync(new LoginUserResponse { Message = x });
                await Console.Out.WriteLineAsync(message.ReceiverEmail + " X");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
        return await Task.FromResult(new EmptyX());
    }


    public override async Task<SendResponse> SendMessageToUser(SendRequest request, ServerCallContext context)
    {
        var list = ObjectCaster<LoginUserRequest, LoginUserResponse>.Cast( _server);

        string sender = request.Message.SenderName;
        string receiver = request.Message.ReceiverName;
        foreach (var user in list)
        {
            if(user.GetName == receiver)
            {


                var senderId = _context.Users.ToList().Find(x => x.UserName == sender);

                var receiverId = _context.Users.ToList().Find(x => x.UserName == receiver);

                await user.response_stream.WriteAsync(new LoginUserResponse
                {
                    Message = new MessageX
                    {
                        MsgText = request.Message.MsgText,
                        SenderName = sender,
                        ReceiverName = receiver,
                        ExtraContent = request.Message.ExtraContent
                    },
                    MessageXUX = new MessageXUX
                    {
                        MsgText = request.Message.MsgText,
                        SenderName = sender,
                        ReceiverAvatar = ByteString.CopyFrom(receiverId.UserAvatar),
                        SenderAvatar = ByteString.CopyFrom(senderId.UserAvatar),
                    }
                });


                var msg = new Entity.Message()
                {
                    SenderId = senderId.Id,
                    ReceiverId = receiverId.Id,
                    ExtraContent = request.Message?.ExtraContent.ToByteArray(),
                    MsgText = request.Message?.MsgText
                };

                await _context.Messages.AddAsync(msg);

                await _context.SaveChangesAsync();

                return await Task.FromResult(new SendResponse
                {
                    IsSended = true,
                });
            }
        }

        var listDb = _context.Users.ToList();

        foreach(var user in listDb)
        {
            if(user.UserName == receiver)
            {
                var senderId = _context.Users.ToList().Find(x => x.UserName == sender);

                var receiverId = _context.Users.ToList().Find(x => x.UserName == receiver);

                var msg = new Entity.Message()
                {
                    SenderId = senderId.Id,
                    ReceiverId = receiverId.Id,
                    ExtraContent = request.Message?.ExtraContent.ToByteArray(),
                    MsgText = request.Message?.MsgText
                };

                await _context.Messages.AddAsync(msg);

                await _context.SaveChangesAsync();

                return await Task.FromResult(new SendResponse
                {
                    IsSended = true,
                });
            }
        }

        return await Task.FromResult(new SendResponse
        {
            IsSended = false,
        });

    }


    public override async Task<GetResponse> GetMessagesForUser(GetRequest request, ServerCallContext context)
    {
        var list = new List<Entity.Message>();

        var senderId = _context.Users.ToList().Find(x => x.UserName == request.SenderName);
        var receiverId = _context.Users.ToList().Find(x => x.UserName == request.ReceiverName);

        foreach (Entity.Message message in _context.Messages.ToList())
        {
            if((message.SenderId == senderId.Id && message.ReceiverId == receiverId.Id) || (message.ReceiverId == senderId.Id && message.SenderId == receiverId.Id))
            {
                list.Add(message);
            }
        }

        GetResponse response = new GetResponse();
        
        foreach (Entity.Message message in list)
        {
            response.Messages.Add(new MessageGet
            {
                MsgText = message.MsgText,
                ExtraContent = ByteString.CopyFrom(message.ExtraContent),
                SenderAvatar = ByteString.CopyFrom(senderId.UserAvatar),
                ReceiverAvatar = ByteString.CopyFrom(receiverId.UserAvatar),
                SenderName = message.SenderId == senderId.Id ? senderId.UserName : receiverId.UserName,
            });
        }

        return await Task.FromResult(response);
    }

}