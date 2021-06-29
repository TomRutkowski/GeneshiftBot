using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Data.SQLite;
using Discord;
using Discord.WebSocket;

namespace draftbot
{
    class Program
    {

        private DiscordSocketClient _client;
        private DiscordSocketConfig _config;
        private SQLiteConnection _dbConnection;
        private Database _database;

        private HashSet<string> _channelIds;

        static void Main(string[] args)
        {
            Console.WriteLine("Running Draft Bot!");
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private HashSet<string> allowedChannels
        {
            get
            {
                if (_channelIds == null)
                {
                    try
                    {
                        _channelIds = new HashSet<string>();
                        using (StreamReader reader = new StreamReader("validchannels.txt"))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                _channelIds.Add(line);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("The file could not be read:\n" + e.Message);
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                }
                return _channelIds;
            }
        }

        public async Task MainAsync()
        {
            _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);

            _dbConnection = new SQLiteConnection("Data Source=draftbot.db;Version=3;");

            try
            {
                _dbConnection.Open();
                _database = new Database(_dbConnection);
            }
            catch (Exception e)
            {
                Console.WriteLine("Starqi sabotaged the code: " + e.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }

            _client.MessageReceived += MessageReceivedAsync;

            var token = Environment.GetEnvironmentVariable("DiscordBotToken");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            Console.ReadLine();

            await _client.StopAsync();
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            Console.WriteLine("Message Received");
            Console.WriteLine(message.ToString());
            string outString = "";
            if ((allowedChannels.Contains(message.Channel.Id.ToString()) || message.Channel.GetType() == typeof(SocketDMChannel)) && !message.Author.IsBot)
            {
                if (message.Content.StartsWith("#"))
                {

                    if (!_database.DoesUserExist(message.Author.Id.ToString()))
                    {
                        outString += _database.CreateUser(message.Author.Id.ToString(), Utilities.getDisplayName(message.Author)) + "\n";
                    }

                    string state = _database.GetState(message.Author.Id.ToString()); //commands are conditional on user's state

                    string[] commandParts = message.Content.Split(" ", 2);
                    string commandName = commandParts[0].Substring(1); //ignoring hash for resolution of command name
                    commandName = commandName.ToLower();

                    bool commandFound = false;
                    foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                    {
                        if (t.Name.ToLower() == commandName)
                        {
                            if (t.GetInterface("Command") != null)
                            {
                                commandFound = true;
                                try
                                {
                                    Command command = (Command)Activator.CreateInstance(t);
                                    if (commandParts.Length == 2)
                                    {
                                        outString += command.Execute(_database, message.Author, state, Utilities.CleanInput(commandParts[1]));
                                    }
                                    else
                                    {
                                        outString += command.Execute(_database, message.Author, state, null);
                                    }
                                }
                                catch (Exception e)
                                {
                                    outString = "Timevir took coding lessons from Bencelot, he should check the console!";
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                    if (commandFound == false)
                    {
                        outString = "Invalid command. Type #remind if you want to repeat draft bot's last message to you.";
                    }

                    await message.Channel.SendMessageAsync(outString);
                }
            }
        }

    }
}
