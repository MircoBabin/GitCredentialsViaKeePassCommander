using System;
using System.Diagnostics;
using System.Text;

namespace git_credential_keepasscommand
{
    public class ProgramOutput
    {
        public static void WriteHelp()
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
            sb.AppendLine("In KeePass an entry containing username and password should be present with the following case-sensitive title (without quotes): \"git [...lowercase last part of directoryname...][...lowercase computername...]\".");
            sb.AppendLine("If this entry is not found, the following case-sensitive title (without quotes) should be present: \"git [...lowercase last part of directoryname...]\".");
            sb.AppendLine();
            sb.AppendLine("e.g. The rootdirectory of the GIT project is");
            sb.AppendLine("     d:\\projects\\GitCredentialsViaKeePassCommander");
            sb.AppendLine("     And the computername is MIRCONB.");
            sb.AppendLine();
            sb.AppendLine("     Then the KeePass title must be exactly (without quotes):");
            sb.AppendLine("     \"git [gitcredentialsviakeepasscommander][mirconb]\", or");
            sb.AppendLine("     \"git [gitcredentialsviakeepasscommander]\".");

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
        }

        public static void WriteError(string message)
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
    }
}
