using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace CliHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: No argument.");
                return;
            }
            else if (args.Length > 1)
            {
                Console.WriteLine("Error: A single argument is expected.");
                return;
            }

            var cmd = args[0];

            using (StreamReader r = new StreamReader("cli-helper.json"))
            {
                string json = r.ReadToEnd();
                var parsed = JsonConvert.DeserializeObject<Settings>(json);
                if (parsed == null || parsed.scripts == null)
                {
                    Console.WriteLine("Error: cli-helper.json does not match expected schema.");
                    return;
                }
                var scripts = parsed.scripts;
                string script;
                if (scripts.TryGetValue(cmd, out script))
                {
                    Bash(script);
                }
                else 
                {
                    Console.WriteLine("Error: Script not found.");
                }
            }
        }

        static void Bash(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
