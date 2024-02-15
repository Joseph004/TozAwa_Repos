using System;
using System.ComponentModel.DataAnnotations;

namespace TozawaNGO.Auth.Models.Authentication
{
    public class CreatedNotModified
    {
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = "";

    }
    public class UserLog : CreatedNotModified
    {
        [Key]
        public Guid Id { get; set; }
        public LogEventType Event { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string UserName { get; set; } = "";
    }
    public enum LogEventType
    {
        AddUser = 0,
        RemoveUser = 1,
        ChangePassword = 2,
        UpdateUserRoles = 3
    }
}