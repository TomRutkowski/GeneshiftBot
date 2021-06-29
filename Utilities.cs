using Discord;
using Discord.WebSocket;
using System;
using System.Text.RegularExpressions;

namespace draftbot
{
    static class Utilities
    {
        public static string getDisplayName(SocketUser user)
        {
            if (user is SocketGuildUser)
            {
                SocketGuildUser guildUser = (user as SocketGuildUser);
                if (guildUser.Nickname != null)
                {
                    return guildUser.Nickname;
                }
            }
            return user.Username;
        }

        public static string CleanInput(string strIn)
        {
        // Replace invalid characters with empty strings.
            try 
            {
                return Regex.Replace(strIn, @"[^\w\d\.-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException) 
            {
                return String.Empty;
            }
        }
    }
}