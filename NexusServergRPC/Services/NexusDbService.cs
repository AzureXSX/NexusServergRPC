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
                    Response = false
                });

                await ProcessRequest<SignUpRequest, SignUpResponse>.ProcessStreamAsync(request, false);
                return;
            }

            await _context.Users.AddAsync(New_User);

            await _context.SaveChangesAsync();

            await response.WriteAsync(new SignUpResponse
            {
                Response = true
            });

            NexusServergRPC.Models.User<SignUpRequest, SignUpResponse> user = new NexusServergRPC.Models.User<SignUpRequest, SignUpResponse>(New_User.UserName, New_User.UserEmail, request, response);

            NexusService.Server?.users.Add(user);

            await ProcessRequest<SignUpRequest, SignUpResponse>.ProcessStreamAsync(request, user);
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

                await Console.Out.WriteLineAsync("Metadata == " + (metadata == null));
                if (temp_user != null)
                {

                    User<LoginUserRequest, LoginUserResponse>? user = ObjectCaster<LoginUserRequest, LoginUserResponse>.Cast(NexusService.Server).Find(x => x.GetName == temp_user.UserName);

                    User<LoginUserRequest, LoginUserResponse> New_User = new User<LoginUserRequest, LoginUserResponse>(temp_user.UserName, temp_user.UserEmail, request, response);
                    if (user != null)
                    {
                        int index = ObjectCaster<LoginUserRequest, LoginUserResponse>.FindIndex(NexusService.Server, user);

                        NexusService.Server.users[index] = New_User;
                    }
                    else
                    {
                        NexusService.Server.users.Add(New_User);
                    }

                    await response.WriteAsync(new LoginUserResponse { Response = true });

                    await ProcessRequest<LoginUserRequest, LoginUserResponse>.ProcessStreamAsync(request, New_User);
                }
                else
                {

                    await Console.Out.WriteLineAsync("NULL ");

                    NexusService.Server.users.Add(new User<LoginUserRequest, LoginUserResponse>(metadata.Get("UserName")?.Value, metadata.Get("UserEmail")?.Value, request, response));

                    await response.WriteAsync(new LoginUserResponse { Response = false });

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
}