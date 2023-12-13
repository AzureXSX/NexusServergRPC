using Grpc.Core;
using Microsoft.AspNetCore.Connections;
using NexusServergRPC.Entity;
using NexusServergRPC.Models;
using NexusServergRPC.Server;
using NexusServergRPC.Services;
using System.Net.Sockets;

namespace NexusServergRPC.RequestProcessing
{
    public static class ProcessRequest<Trequest, Tresponse>
    {
        public static async Task<bool> ProcessStreamAsync(IAsyncStreamReader<ConnectedX> requestStream, User<Trequest, Tresponse> user)
        {
            await Console.Out.WriteLineAsync("PROCESSING <ConnectedX>");
            try
            {
                while (await requestStream.MoveNext())
                {
                    
                    Thread.Sleep(1000);
                }
                await Console.Out.WriteLineAsync("ENDED");
                NexusService.Server?.users.Remove(user);
            }
            catch
            {

            }
            await Console.Out.WriteLineAsync("Connection closed");
            return false;
        }

        public static async Task<bool> ProcessStreamAsync(IAsyncStreamReader<SignUpRequest> requestStream, User<Trequest, Tresponse> user)
        {
            await Console.Out.WriteLineAsync("PROCESSING <SignUpRequest>");
            try
            {
                while (await requestStream.MoveNext())
                {
                    Thread.Sleep(1000);
                }
                await Console.Out.WriteLineAsync("ENDED SIGNUP");
                NexusService.Server?.users.Remove(user);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled || ex.StatusCode == StatusCode.Aborted)
            {

            }
            catch (IOException)
            {

            }
            catch (ConnectionAbortedException)
            {

            }
            catch (SocketException)
            {

            }
            return false;
        }

        public static async Task<bool> ProcessStreamAsync(IAsyncStreamReader<SignUpRequest> requestStream, bool close)
        {
            await Console.Out.WriteLineAsync("PROCESSING <SignUpRequest>");
            try
            {
                while (await requestStream.MoveNext())
                {
                    Thread.Sleep(1000);
                }
                await Console.Out.WriteLineAsync("ENDED SIGNUP");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled || ex.StatusCode == StatusCode.Aborted)
            {

            }
            catch (IOException)
            {

            }
            catch (ConnectionAbortedException)
            {

            }
            catch (SocketException)
            {

            }
            return false;
        }

        public static async Task<bool> ProcessStreamAsync(IAsyncStreamReader<LoginUserRequest> requestStream, User<Trequest, Tresponse> user)
        {
            await Console.Out.WriteLineAsync("PROCESSING <LoginUserRequest>");

       
            try
            {
                while (await requestStream.MoveNext())
                {
                    await foreach(var item in requestStream.ReadAllAsync())
                    {
                        await Console.Out.WriteLineAsync(item.UserName);
                    }
                    Thread.Sleep(1000);
                }
                await Console.Out.WriteLineAsync("ENDED LOGIN");
                NexusService.Server?.users.Remove(user);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled || ex.StatusCode == StatusCode.Aborted)
            {

            }
            catch (IOException)
            {

            }
            catch (ConnectionAbortedException)
            {

            }
            catch (SocketException)
            {

            }
            return false;
        }

        public static async Task<bool> ProcessStreamAsync(IAsyncStreamReader<LoginUserRequest> requestStream, bool close)
        {
            await Console.Out.WriteLineAsync("PROCESSING <LoginUserRequest> CLOSED");
            try
            {
                while (await requestStream.MoveNext())
                {
                    break;
                }
                await Console.Out.WriteLineAsync("CLOSED LOGIN");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled || ex.StatusCode == StatusCode.Aborted)
            {

            }
            catch (IOException)
            {

            }
            catch (ConnectionAbortedException)
            {

            }
            catch (SocketException)
            {

            }
            return false;
        }
    }
}
