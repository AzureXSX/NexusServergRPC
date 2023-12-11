
namespace NexusServergRPC.Entity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

public class User
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPassword { get; set; }
    public byte[]? UserAvatar { get; set; }

    private readonly DirectoryInfo? dir = Directory.CreateDirectory(Directory.GetCurrentDirectory()).Parent;

    public User(string? userName, string? userEmail, string? userPassword)
    {
        UserName = userName;
        UserEmail = userEmail;
        UserPassword = userPassword;
        UserAvatar = File.ReadAllBytes(dir.FullName + "\\NexusServergRPC\\Images\\Avatar.png");        
    }





    //[NotMapped]

    //[Required]

    //[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
}