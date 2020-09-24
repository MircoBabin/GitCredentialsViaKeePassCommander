using KeePassCommandDll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace git_credential_keepasscommand
{
    class Program
    {
        private static ExitCode ShowHelp()
        {
            StringBuilder sb = new StringBuilder();

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            sb.Append("git-credential-keepasscommand ");
            sb.Append(version.Major + "." + version.Minor);
            if (version.Build > 0)
            {
                sb.Append(" [patch " + version.Build + "]");
            }
            sb.AppendLine();
            sb.AppendLine("https://github.com/MircoBabin/GitCredentialsViaKeePassCommander - MIT license");

            sb.AppendLine();

            sb.AppendLine("git-credential-keepasscommand.exe is a credential helper for GIT.");
            sb.AppendLine("It's purpose is to retrieve username/password credentials for GIT from KeePass using plugin KeePassCommander.");

            sb.AppendLine();
            sb.AppendLine("INSTALLATION:");
            sb.AppendLine("*** Place executable and config in same directory as KeePassCommandDll.dll");
            sb.AppendLine("*** See KeePassCommander at https://github.com/MircoBabin/KeePassCommander");
            sb.AppendLine("git config --global credential.helper \"" + Process.GetCurrentProcess().MainModule.FileName.Replace('\\', '/') + "\"");
            sb.AppendLine("git config --system --list --show-origin");
            sb.AppendLine("if there still is a credential.helper=manager listed, remove that");
            sb.AppendLine("e.g. from C:/Program Files/Git/mingw64/etc/gitconfig remove [credential] helper=manager");

            sb.AppendLine();
            sb.AppendLine("UNINSTALL:");
            sb.AppendLine("git config --global credential.helper manager");

            sb.AppendLine();
            sb.AppendLine("KeePass ENTRY:");
            sb.AppendLine("In KeePass an entry containing username and password should be present with the following case-sensitive title (without quotes): \"git [...lowercase last part of directoryname...]\".");
            sb.AppendLine("e.g. the rootdirectory of the GIT project is d:\\projects\\GitCredentialsViaKeePassCommander, then the KeePass title is exactly \"git [gitcredentialsviakeepasscommander]");

            sb.AppendLine();
            sb.AppendLine("DEBUG:");
            sb.AppendLine("    --- Enable git logging");
            sb.AppendLine("    setx GIT_TRACE c:\\incoming\\git.log");
            sb.AppendLine("    setx GCM_TRACE c:\\incoming\\git.log");
            sb.AppendLine("    --- Fiddler as http proxy");
            sb.AppendLine("    git config --global http.proxy http://localhost:8888");
            sb.AppendLine("    git config --global http.sslVerify false");
            sb.AppendLine("    ---");
            sb.AppendLine("    restart dosbox");
            sb.AppendLine();
            sb.AppendLine("UNDEBUG:");
            sb.AppendLine("    --- Disable git logging");
            sb.AppendLine("    reg delete HKCU\\Environment /F /V GIT_TRACE");
            sb.AppendLine("    reg delete HKCU\\Environment /F /V GCM_TRACE");
            sb.AppendLine("    --- Unset http proxy");
            sb.AppendLine("    git config --global --unset http.proxy");
            sb.AppendLine("    git config --global --unset http.sslVerify");
            sb.AppendLine("    ---");
            sb.AppendLine("    restart dosbox");

            sb.AppendLine();
            sb.AppendLine("--- LICENSE ---");
            sb.AppendLine("Git Credentials via KeePassCommander");
            sb.AppendLine("MIT license");
            sb.AppendLine();
            sb.AppendLine("Copyright (c) 2020 Mirco Babin");
            sb.AppendLine();
            sb.AppendLine("Permission is hereby granted, free of charge, to any person");
            sb.AppendLine("obtaining a copy of this software and associated documentation");
            sb.AppendLine("files (the \"Software\"), to deal in the Software without");
            sb.AppendLine("restriction, including without limitation the rights to use,");
            sb.AppendLine("copy, modify, merge, publish, distribute, sublicense, and/or sell");
            sb.AppendLine("copies of the Software, and to permit persons to whom the");
            sb.AppendLine("Software is furnished to do so, subject to the following");
            sb.AppendLine("conditions:");
            sb.AppendLine();
            sb.AppendLine("The above copyright notice and this permission notice shall be");
            sb.AppendLine("included in all copies or substantial portions of the Software.");
            sb.AppendLine();
            sb.AppendLine("THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND,");
            sb.AppendLine("EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES");
            sb.AppendLine("OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND");
            sb.AppendLine("NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT");
            sb.AppendLine("HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,");
            sb.AppendLine("WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING");
            sb.AppendLine("FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR");
            sb.AppendLine("OTHER DEALINGS IN THE SOFTWARE.");

            Console.WriteLine();
            Console.Write(sb.ToString());
            return ExitCode.ShowHelp;
        }

        private static void WriteError(string message)
        {
            Console.Error.Write(Environment.NewLine);
            Console.Error.Write("!!! <Git Credentials via KeePassCommander (git-credential-keepasscommand)>");
            Console.Error.Write(Environment.NewLine);
            Console.Error.Write(Environment.NewLine);
            Console.Error.Write("??? Is KeePass started and the KeePassCommander plugin active ???");
            Console.Error.Write(Environment.NewLine);
            Console.Error.Write(Environment.NewLine);
            Console.Error.Write(message);
            Console.Error.Write(Environment.NewLine);
            Console.Error.Write(Environment.NewLine);
            Console.Error.Write("!!! </Git Credentials via KeePassCommander (git-credential-keepasscommand)>");
            Console.Error.Write(Environment.NewLine);
            Console.Error.Write(Environment.NewLine);
        }


        private static List<NameValue> GetStdinNamesValues()
        {
            var result = new List<NameValue>();

            var curdirFull = Directory.GetCurrentDirectory();
            var curdirName = new DirectoryInfo(curdirFull).Name; //use last directory part of "d:\projects\GitCredentialsViaKeePassCommander"

            result.Add(new NameValue("__CurrentDirectory__", curdirFull));
            result.Add(new NameValue("__CurrentName__", curdirName));

            string stdin = String.Empty;
            try
            {
                var isKeyAvailable = Console.KeyAvailable;
            }
            catch (InvalidOperationException)
            {
                //On a redirected stdin, there can't be a KeyAvailable
                stdin = Console.In.ReadToEnd();
            }

            foreach (var line in stdin.Split('\n'))
            {
                var parts = line.Trim().Split('=');
                if (parts.Length == 2)
                {
                    result.Add(new NameValue(parts[0].Trim(), parts[1].Trim()));
                }
            }

            return result;
        }

        private static ApiGetResponse QueryKeePass(string title)
        {
            var found = KeePassCommandDll.Api.get(new List<string> { title });

            StringBuilder message = new StringBuilder();
            message.AppendLine("Searched in KeePass for case-sensitive title (without quotes):");
            message.AppendLine("\"" + title + "\"");
            message.AppendLine();

            switch (found.Count)
            {
                case 1:
                    return found[0];

                default:
                    message.AppendLine(found.Count + " results found, should be 1 result");
                    foreach (var entry in found)
                    {
                        message.AppendLine(entry.Title);
                    }
                    break;
            }

            WriteError(message.ToString());
            return null;
        }

        private static ExitCode CredentialHelperCommandGet()
        {
            //stdin contains input as name = value pairs.
            //return via stdout 2 lines when found
            //    username=
            //    password=

            try
            {
                NameValue curname = null;
                try
                {
                    var input = GetStdinNamesValues();
                    foreach (var item in input)
                    {
                        var name = item.Name.ToLowerInvariant();
                        if (name == "__currentname__")
                        {
                            curname = item;
                        }
                    }

                    if (curname == null)
                    {
                        StringBuilder message = new StringBuilder();
                        message.AppendLine("GIT failed to provide enough information:");
                        foreach (var item in input)
                        {
                            message.AppendLine(item.Name + "=" + item.Value);
                        }

                        WriteError(message.ToString());
                        return ExitCode.GitInputError;
                    }
                }
                catch (Exception ex)
                {
                    WriteError(ex.ToString());
                    return ExitCode.GitInputError;
                }

                ApiGetResponse found = null;
                try
                {
                    string title = "git [" + curname.Value.ToLowerInvariant() + "]";
                    found = QueryKeePass(title);

                    if (found == null)
                    {
                        //WriteError() is already done in QuerykeePass()
                        return ExitCode.KeePassEntryNotFound;
                    }
                }
                catch (Exception ex)
                {
                    WriteError(ex.ToString());
                    return ExitCode.KeePassError;
                }

                Console.Out.Write("username=" + found.Username + "\n");
                Console.Out.Write("password=" + found.Password + "\n");
                return ExitCode.Success;
            }
            catch (Exception ex)
            {
                WriteError(ex.ToString());
                return ExitCode.UnknownError;
            }
        }

        private static ExitCode CredentialHelperCommandStore()
        {
            //stdin contains input as name = value pairs.
            //no output needed

            try
            {
                //var input = GetStdinNamesValues();
                //ignore command

                return ExitCode.Success;
            }
            catch (Exception ex)
            {
                WriteError(ex.ToString());
                return ExitCode.UnknownError;
            }
        }

        private static ExitCode CredentialHelperCommandErase()
        {
            //stdin contains input as name = value pairs.
            //no output needed

            try
            {
                //var input = GetStdinNamesValues();
                //ignore command

                return ExitCode.Success;
            }
            catch (Exception ex)
            {
                WriteError(ex.ToString());
                return ExitCode.UnknownError;
            }
        }

        enum ExitCode : int
        {
            Success = 0,
            GitInputError = 1,
            KeePassError = 2,
            KeePassEntryNotFound = 3,
            UnknownCommand = 97,
            ShowHelp = 98,
            UnknownError = 99
        }

        static void Main(string[] args)
        {
            ExitCode exitcode;

            try
            {
                string command;
                if (args.Length > 0)
                    command = args[0].ToLowerInvariant();
                else
                    command = "help";

                if (command == "get")
                {
                    exitcode = CredentialHelperCommandGet();
                }
                else if (command == "store")
                {
                    exitcode = CredentialHelperCommandStore();
                }
                else if (command == "erase")
                {
                    exitcode = CredentialHelperCommandErase();
                }
                else if (command == "help")
                {
                    exitcode = ShowHelp();
                }
                else
                {
                    exitcode = ExitCode.UnknownCommand;
                }
            }
            catch (Exception ex)
            {
                WriteError(ex.ToString());
                exitcode = ExitCode.UnknownError;
            }

            Environment.Exit((int)exitcode);
        }
    }
}
