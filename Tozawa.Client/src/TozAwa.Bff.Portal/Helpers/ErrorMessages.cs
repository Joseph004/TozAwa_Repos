using System;
using System.Net;

namespace Tozawa.Bff.Portal.Helpers;
public static partial class ErrorMessages
{
    public static string EmailNotExists => "Your email adress does not exist";
    public static string PassWordWrong => "This password is wrong";
    public static string Success => "success";
    public static string UserNameNotExists => "Your user name does not exist";
    public static string NotRootUser => "Unauthorised, not a root user!";
    public static string Error => "Error";
    public static string UserAlreadyExist => "User already exists!";
    public static string UserCreationFailed => "User creation failed! Please check user details and try again.";
    public static string UserCreationSuccess => "User created successfully!";
}