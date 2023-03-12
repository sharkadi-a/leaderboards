using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AndreyGames.Leaderboards.Tests
{
    public class TestConfigFileBuilder
    {
        private readonly Dictionary<string, string> _users = new();
        private string _connectionString;
        private string _vector;

        private class NamingPolicy: JsonNamingPolicy
        {
            public override string ConvertName(string name) => name;
        }

        public TestConfigFileBuilder AddApiUser(string userName, string userPassword)
        {
            _users[userName] = userPassword;
            return this;
        }

        public TestConfigFileBuilder UseCryptoVectorString(string value)
        {
            _vector = value;
            return this;
        }

        public TestConfigFileBuilder UseConnectionString(string value)
        {
            _connectionString = value;
            return this;
        }

        public Stream Build()
        {
            var obj = new
            {
                Auth = _users.Select(x => new
                {
                    UserName = x.Key,
                    Password = x.Value,
                }),
                ConnectionStrings = new
                {
                    Default = _connectionString
                },
                CryptoVectorString = _vector
            };

            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new NamingPolicy(),
            });

            var stream = new MemoryStream();
            stream.Write(Encoding.Default.GetBytes(json));
            stream.Flush();
            stream.Position = 0;

            return stream;
        }
        
    }
}