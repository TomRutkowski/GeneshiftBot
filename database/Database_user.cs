using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace draftbot
{

    //User database functions
    partial class Database
    {
        
        public bool DoesUserExist(string userId)
        {
            SQLiteCommand userExistsCommand = new SQLiteCommand(_dbConnection);
            userExistsCommand.CommandText = "SELECT UserID FROM User WHERE UserId = '" + userId + "'";
            string userOut = (string)userExistsCommand.ExecuteScalar();
            if (userOut == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string CreateUser(string userId, string userName)
        {
            SQLiteCommand createUserCommand = new SQLiteCommand(_dbConnection);
            string remindString = "Hi " + userName + "! I just created space for your user in the database. Type #new to start a new draft. Type #help to list commands.\n";
            createUserCommand.CommandText = "INSERT INTO User (UserId, State, RemindString) VALUES('" + userId + "', 'Start', '" + remindString +"')";
            createUserCommand.ExecuteNonQuery();
            return remindString;
        }

        public string GetState(string userId)
        {
            SQLiteCommand getStateCommand = new SQLiteCommand(_dbConnection);
            getStateCommand.CommandText = "SELECT State FROM User WHERE UserId = '" + userId + "'";
            string stateOut = (string)getStateCommand.ExecuteScalar(); //State is configured to never be null in the schema.
            return stateOut;
        }

        public void SetState(string userId, string destinationState)
        {
            SQLiteCommand setStateCommand = new SQLiteCommand(_dbConnection);
            setStateCommand.CommandText = "UPDATE User SET State = '" + destinationState + "' WHERE UserId = '" + userId + "'";
            setStateCommand.ExecuteNonQuery();
        }

        public string GetRemind(string userId)
        {
            SQLiteCommand getRemindCommand = new SQLiteCommand(_dbConnection);
            getRemindCommand.CommandText = "SELECT RemindString FROM User WHERE UserId = '" + userId + "'";
            string remind = (string)getRemindCommand.ExecuteScalar(); //RemindString is never null
            return remind;
        }

        public void SetRemind(string userId, string remindString)
        {
            SQLiteCommand setRemindCommand = new SQLiteCommand(_dbConnection);
            setRemindCommand.CommandText = "UPDATE User SET RemindString = '" + remindString + "' WHERE UserId = '" + userId + "'";
            setRemindCommand.ExecuteNonQuery();
        }

    }
}