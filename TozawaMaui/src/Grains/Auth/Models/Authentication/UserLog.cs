using System;
using System.ComponentModel.DataAnnotations;

namespace Grains.Auth.Models.Authentication
{
    public class CreatedNotModified
    {
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = "";
        public DateTime ModifiedDate { get; set; }
        public string ModifieddBy { get; set; } = "";

    }
    public class UserLog : CreatedNotModified
    {
        [Key]
        public Guid Id { get; set; }
        public LogEventType Event { get; set; }
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
        public string OrganizationName { get; set; } = "";
        public string FeatureName { get; set; } = "";
        public Guid TextId { get; set; }
        public Guid DescriptionTextId { get; set; }
    }
    public enum LogEventType
    {
        AddUser = 0,
        RemoveUser = 1,
        ChangePassword = 2,
        UpdateUserRoles = 3,
        UpdateUser = 4,
        DeleteUser = 5,
        AddOrganization = 6,
        UpdateOrganization = 7,
        AddFeature = 8,
        UpdateFeature = 9,
        AddRole = 10,
        UpdateRole = 11
    }
}