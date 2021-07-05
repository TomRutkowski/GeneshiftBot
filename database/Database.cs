using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace draftbot
{
    partial class Database
    {
        private SQLiteConnection _dbConnection;

        public Database(SQLiteConnection connectionIn)
        {
            _dbConnection = connectionIn;
        }

    }

}