﻿using System.IO;
using static System.Console;

namespace dependadot
{
    class Program
    {
        private static string[] s_patterns = new string[]
            {".csproj", ".fsproj", ".vbproj"};

        static void Main(string[] args)
        {
            var path = string.Empty;

            if (IsInputRedirected)
            {
                var line = In.ReadLine();
                if (line is object)
                {
                    path = line;
                }
                else
                {
                    Error();
                    return;
                }
            }
            else if (args.Length == 1 &&
                Directory.Exists(args[0]))
            {
                path = args[0];
            }
            else
            {
                Error();
                return;
            }

            PrintBoilerPlate();

            foreach (var file in Directory.EnumerateFiles(path, "*.*proj", SearchOption.AllDirectories))
            {
                if (IsProject(file))
                {
                    /* pattern:
                    - package_manager: "dotnet:nuget"
                      directory: "/one"
                      update_schedule: "live"
                    */

                    var filename = Path.GetFileName(file);
                    var parentDir = Path.GetDirectoryName(file);
                    var relativeDir = string.Empty;

                    if (parentDir == null ||
                        parentDir.Length == path.Length)
                    {
                        relativeDir = "/";
                    }
                    else
                    {
                        relativeDir = parentDir.Substring(path.Length).Replace('\\','/');
                    }
                    WriteLine(
$@"  - package_manager: ""dotnet:nuget""
    directory: ""{relativeDir}"" # {filename}
    update_schedule: ""live""");
                }
            }
        }

        static void PrintBoilerPlate()
        {
            WriteLine(
@"# Generated by https://github.com/richlander/dependadotnet
version: 1

update_configs:");
        }

        static void Error()
        {
            WriteLine("Must specify a repo root directory as input");
        }

        public static bool IsProject(string filename)
        {
            return
                filename.EndsWith(s_patterns[0]) ||
                filename.EndsWith(s_patterns[1]) ||
                filename.EndsWith(s_patterns[2]);
        }
    }
}
