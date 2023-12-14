namespace NexusServergRPC.Services;
using Grpc.Core;
using NexusServergRPC.Entity;
using NexusContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;
using NexusServergRPC.Server;
using NexusServergRPC.RequestProcessing;
using Google.Protobuf;
using NexusServergRPC.Models;
using Microsoft.AspNetCore.Hosting.Server;

public class NexusDbService : NexusDb.NexusDbBase{
    private readonly NexusContext _context;
    public NexusDbService(NexusContext context)
    {
        _context = context;
    }

    public override async Task SignUpUser(IAsyncStreamReader<SignUpRequest> request, IServerStreamWriter<SignUpResponse> response, ServerCallContext context){

        try{

            var metadata = context.RequestHeaders;
            User? New_User = new User(metadata.Get("UserName")?.Value, metadata.Get("UserEmail")?.Value, metadata.Get("UserPassword")?.Value);
            User? temp_user = _context.Users.ToList().Find(x => x.UserName == metadata.Get("UserName")?.Value || x.UserEmail == metadata.Get("UserEmail")?.Value);

            if (temp_user != null)
            {
                await response.WriteAsync(new SignUpResponse
                {
                    Response = false,
                    ResponseMessage = "User already exist."
                });

                await ProcessRequest<SignUpRequest, SignUpResponse>.ProcessStreamAsync(request, false);
                return;
            }

            await _context.Users.AddAsync(New_User);

            await _context.SaveChangesAsync();

            await response.WriteAsync(new SignUpResponse
            {
                Response = true,
                ResponseMessage = "Successfully Signed Up."
            });

            await ProcessRequest<SignUpRequest, SignUpResponse>.ProcessStreamAsync(request, false);
            return;
            //NexusServergRPC.Models.User<SignUpRequest, SignUpResponse> user = new NexusServergRPC.Models.User<SignUpRequest, SignUpResponse>(New_User.UserName, New_User.UserEmail, request, response);

            //NexusService.Server?.users.Add(user);
            //await user.SetTask(ProcessRequest<SignUpRequest, SignUpResponse>.ProcessStreamAsync(request, false));
        }
        catch(SqlException){

            Console.Clear();
           
        }
       

    }

    public override async Task LoginUser(IAsyncStreamReader<LoginUserRequest> request, IServerStreamWriter<LoginUserResponse> response, ServerCallContext context)
    {
        try
        {
            var metadata = context.RequestHeaders;
            
            if (metadata != null)
            {

                User? temp_user = _context.Users.ToList<User>().Find(user => user.UserName == metadata.Get("UserName")?.Value && user.UserPassword == metadata.Get("UserPassword")?.Value);


                if (temp_user != null)
                {
                    await Console.Out.WriteLineAsync("LOGGIN == " + (temp_user == null));

                    User<LoginUserRequest, LoginUserResponse>? user = ObjectCaster<LoginUserRequest, LoginUserResponse>.Cast(NexusService.Server).Find(x => x.GetName == temp_user.UserName);

                    User<LoginUserRequest, LoginUserResponse> New_User = new User<LoginUserRequest, LoginUserResponse>(temp_user.UserName, temp_user.UserEmail, request, response);

                    if (user != null)
                    {
                        int index = ObjectCaster<LoginUserRequest, LoginUserResponse>.FindIndex(NexusService.Server, user);

                        ObjectCaster<LoginUserRequest, LoginUserResponse>.Cast(NexusService.Server)?.Find(x => x.GetName == temp_user.UserName).Terminate();

                        NexusService.Server.users.RemoveAt(index);

                        await response.WriteAsync(new LoginUserResponse
                        {
                            Response = true,
                            ResponseMessage = "Successfully logged in",
                            UserAvatar = ByteString.CopyFrom(temp_user.UserAvatar)
                        });

                        await ProcessRequest<LoginUserRequest, LoginUserResponse>.ProcessStreamAsync(request, false);

                        NexusService.Server.users.Add(New_User);
                        await Console.Out.WriteLineAsync("N-Count: " + NexusService.Server.users.Count);
                        await New_User.SetTask(ProcessRequest<LoginUserRequest, LoginUserResponse>.ProcessStreamAsync(request, New_User));
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync("UWU" + NexusService.Server.users.Count);

                        await response.WriteAsync(new LoginUserResponse
                        {
                            Response = true,
                            ResponseMessage = "Successfully logged in",
                            UserAvatar = ByteString.CopyFrom(temp_user.UserAvatar)
                        });

                        await ProcessRequest<LoginUserRequest, LoginUserResponse>.ProcessStreamAsync(request, false);


                        NexusService.Server.users.Add(New_User);
                        await New_User.SetTask(ProcessRequest<LoginUserRequest, LoginUserResponse>.ProcessStreamAsync(request, New_User));
                    }

                  
                }
                else
                {

                    await Console.Out.WriteLineAsync("NULL ");

                    NexusService.Server.users.Add(new User<LoginUserRequest, LoginUserResponse>(metadata.Get("UserName")?.Value, metadata.Get("UserEmail")?.Value, request, response));

                    await response.WriteAsync(new LoginUserResponse { 
                        Response = false,
                        ResponseMessage = "Failed to login"
                    });

                    await ProcessRequest<LoginUserRequest, LoginUserResponse>.ProcessStreamAsync(request, false);
                }


                

            }
            else
            {
                await Console.Out.WriteLineAsync("METADATA NULL");
            }

            
        }
        catch (SqlException)
        {
           
        }
    }


    public override async Task<LogOutResponse> LogOutUser(LogOutRequest request, ServerCallContext context)
    {


        return await Task.FromResult(new LogOutResponse { Response = true });
    }


    public override async Task<GetContactsResponse> GetContacts(GetContactsRequest request, ServerCallContext context)
    {

        var temp_user = _context.Users.ToList().Find(x => x.UserName == request.UserName);

        List<ContactX> contacts = new List<ContactX>();

        _context.Contacts.ToList().ForEach(contact =>
        {
            if(contact.FirstU == temp_user.Id || contact.SecondU == temp_user.Id)
            {
                contacts.Add(new ContactX
                {
                    UserName = temp_user.Id == contact.FirstU ? _context.Users.ToList().Find(x => x.Id == contact.SecondU).UserName :
                    _context.Users.ToList().Find(x => x.Id == contact.FirstU).UserName,
                    UserAvatar = temp_user.Id == contact.FirstU ? ByteString.CopyFrom(_context.Users.ToList().Find(x => x.Id == contact.SecondU).UserAvatar) :
                    ByteString.CopyFrom(_context.Users.ToList().Find(x => x.Id == contact.FirstU).UserAvatar),
                    LastMessage = "Last message"
                });
            }
        });
       
        GetContactsResponse response = new GetContactsResponse();

        response.ContactList.AddRange(contacts);

        return response;
    }
}