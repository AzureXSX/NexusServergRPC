using NexusServergRPC.Models;
using NexusServergRPC.Server;

namespace NexusServergRPC.RequestProcessing
{
    public static class ObjectCaster<Trequest, Tresult>
    {
        public static List<User<Trequest, Tresult>> Cast(Nexus_Server server)
        {
            List<User<Trequest, Tresult>> users = new();

            foreach (var item in server.users)
            {
                User<Trequest, Tresult>? temp = item as User<Trequest, Tresult>;

                if(temp != null) users.Add(temp);
            }
            return users;
        }

        public static List<User<Trequest, Tresult>> Cast(List<object> obj)
        {
            List<User<Trequest, Tresult>> users = new();

            foreach (var item in obj)
            {
                User<Trequest, Tresult>? temp = item as User<Trequest, Tresult>;

                if (temp != null) users.Add(temp);
            }
            return users;
        }

        public static int FindIndex(Nexus_Server server, User<Trequest, Tresult> user)
        {
            int index = 0;
            for (int i = 0; i < server.users.Count; i++)
            {
                User<Trequest, Tresult>? temp = server.users[i] as User<Trequest, Tresult>;

                if (temp != null && temp.GetEmail == user.GetEmail) index = i;

            }

            return index;
        }
    }
}
