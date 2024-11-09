using System;
using System.Text.RegularExpressions;

namespace OneLastSong.Utils
{
    public class AuthUtils
    {
        public static string ValidateUsername(string username)
        {
            if (username.Length == 0)
            {
                return "Username cannot be empty";
            }
            if (username.Length < 3)
            {
                return "Username must be at least 3 characters long";
            }
            if (username.Length > 20)
            {
                return "Username must be at most 20 characters long";
            }
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            {
                return "Username can only contain letters, numbers, and underscores";
            }
            return "";
        }

        public static string ValidatePassword(string password)
        {
            if(password.Length == 0)
            {
                return "Password cannot be empty";
            }
            if (password.Length < 8)
            {
                return "Password must be at least 8 characters long";
            }
            if (password.Length > 20)
            {
                return "Password must be at most 20 characters long";
            }
            if (!Regex.IsMatch(password, @"[a-zA-Z]"))
            {
                return "Password must contain at least one letter";
            }
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return "Password must contain at least one number";
            }
            return "";
        }

        public static string ValidateConfirmPassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return "Passwords do not match";
            }
            return "";
        }
    }
}