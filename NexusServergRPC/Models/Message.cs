namespace NexusServergRPC.Models;


public class Message{
    public int Id { get; set; }
    public string? MsgText { get; set; }
    public byte[]? ExtraContent { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }


    public Message(int id, string? msgtext, byte[]? extracontent, int senderId, int receiverId)
    {
        Id = id;
        MsgText = msgtext;
        ExtraContent = extracontent;
        SenderId = senderId;
        ReceiverId = receiverId;
    }

    public Message(string? msgtext, byte[]? extracontent, int senderId, int receiverId)
    {
        MsgText = msgtext;
        ExtraContent = extracontent;
        SenderId = senderId;
        ReceiverId = receiverId;
    }

    public Message(){}
}