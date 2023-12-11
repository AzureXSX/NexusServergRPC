using NexusServergRPC.Services;

namespace NexusServergRPC.Auth
{
    public class NexusToken
    {
        private readonly string _token;

        private readonly string _userName;

        public string UserName { get {  return _userName; } }

        public string Token { get { return _token; } }

        public NexusToken(string token, string userName, DateTime expiration)
        {
            _token = token;
            _userName = userName;

            TimeSpan timeDifferenceInSeconds = expiration - DateTime.Now;

            Console.WriteLine(timeDifferenceInSeconds.TotalSeconds);

            Task.Delay(TimeSpan.FromSeconds(timeDifferenceInSeconds.TotalSeconds)).ContinueWith(_ =>
            {
                Task t = new Task(() => {
                    NexusService.Server.RemoveToken(_token);
                });
                t.Start();
            });

            Console.WriteLine("TOKEN CREATED");
            Console.WriteLine(token);
        }
    }
}
