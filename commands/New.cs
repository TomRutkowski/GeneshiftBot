using System;
using System.Collections.Generic;
using Discord.WebSocket;

namespace draftbot
{
    class New: Command
    {
        public string Execute(Database database, SocketUser user, string state, string param)
        {
            database.ClearDraft(user.Id.ToString());
            // database.ClearPreviousDraftChoices(user.Id.ToString()); <- Implied, but will be done later in the code automatically
            database.SetState(user.Id.ToString(), "Start");
            Draft draftCommand = new Draft();
            return draftCommand.Execute(database, user, "Start", null);
        }

        public string ShortDescription()
        {
            return "Starts a new draft.";
        }
    }

}