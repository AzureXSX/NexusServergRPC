namespace NexusServergRPC.Models;


public class Message{
    public int Id { get; set; }
    public string? MsgText { get; set; }
    public byte[]? ExtraContent { get; set; }


    public Message(int id, string? msgtext, byte[]? extracontent){
        Id = id;
        MsgText = msgtext;
        ExtraContent = extracontent;
    }

    public Message(string? msgtext, byte[]? extracontent){
        MsgText = msgtext;
        ExtraContent = extracontent;
    }

    public Message(){}
}