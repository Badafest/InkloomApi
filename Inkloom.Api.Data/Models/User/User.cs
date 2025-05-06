using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace Inkloom.Api.Data.Models;
public enum AuthType { PASSWORD, GOOGLE, FACEBOOK, MAGICLINK };
public partial class User : ModelBase
{
    public bool EmailVerified { get; set; } = false;

    public bool ProfileComplete { get; set; } = true;

    public string? Avatar { get; set; }

    public string? About { get; set; }

    private AuthType[] _AuthTypes = [AuthType.PASSWORD];
    public AuthType[] AuthTypes
    {
        get
        {
            return _AuthTypes;
        }
        set
        {
            _AuthTypes = [.. value.Distinct()];
        }
    }
    public string? FacebookId { get; set; }
    public DateTime? TokenBlacklistTimestamp { get; set; }
    private string ValidEmail = "";
    public string Email
    {
        get
        {
            return ValidEmail;
        }
        set
        {
            if (!IsValidEmail(value))
            {
                throw new ArgumentException("Invalid Email Address");
            }
            ValidEmail = value;

        }
    }
    private string ValidUsername = "";
    public string Username
    {
        get
        {
            return ValidUsername;
        }
        set
        {
            if (!UsernameRegex().IsMatch(value))
            {
                throw new ArgumentException("Username should contain lowercase letters and numbers only");
            }
            ValidUsername = value;
        }
    }
    public string DisplayName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    [NotMapped]
    public string Password
    {
        get
        {
            return PasswordHash;
        }
        set
        {
            // allow empty passwords (for SSO Login)
            if (value.Length == 0)
            {
                PasswordHash = "";
                return;
            }
            if (!PasswordRegex().IsMatch(value))
            {
                throw new ArgumentException("Password must contain at least 8 characters and at least 1 uppercase letter, 1 lowercase letter, and 1 number");
            }

            PasswordHash = hasher.HashPassword(this, value);
        }
    }
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    [GeneratedRegex("^[a-z0-9]+$")]
    private static partial Regex UsernameRegex();

    [GeneratedRegex("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}")]
    private static partial Regex PasswordRegex();

    private static readonly PasswordHasher<User> hasher = new();

    public bool VerifyPassword(string password)
    {
        // do not verify empty passwords
        if (password.Length == 0)
        {
            return false;
        }
        var result = hasher.VerifyHashedPassword(this, Password, password);
        if (result == PasswordVerificationResult.Success)
        {
            return true;
        }
        return false;
    }
    public List<Blog> Blogs { get; set; } = [];
    public List<UserFollower> Followers { get; set; } = [];
    public List<UserFollower> Followings { get; set; } = [];
}