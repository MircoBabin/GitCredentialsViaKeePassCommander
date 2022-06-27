using System;
using System.Text;

namespace git_credential_keepasscommand
{
    class Program
    {
        private static Version GetVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        private static ProgramExitCode CredentialHelperCommandGet()
        {
            //stdin contains input as name = value pairs.
            //return via stdout 2 lines when found
            //    username=
            //    password=

            StringBuilder message;

            try
            {
                NameValuePairs input = null;
                try
                {
                    input = NameValuePairs.fromStdin();
                }
                catch (Exception ex)
                {
                    message = new StringBuilder();
                    message.AppendLine("Error retrieving GIT input parameters from stdin:");
                    message.AppendLine(ex.ToString());
                    return ProgramExitCode.GitInputError;

                }

                QueryKeePassResult result = QueryKeePass.ForGitCredentials(input);
                if (result.exitcode != null)
                {
                    if (result.message != null) ProgramOutput.WriteError(result.message.ToString());
                    return (ProgramExitCode) result.exitcode;
                }

                Console.Out.Write("username=" + result.GetFoundUsername() + "\n");
                Console.Out.Write("password=" + result.GetFoundPassword() + "\n");
                return ProgramExitCode.Success;
            }
            catch (Exception ex)
            {
                ProgramOutput.WriteError(ex.ToString());
                return ProgramExitCode.UnknownError;
            }
        }

        private static ProgramExitCode CredentialHelperCommandStore()
        {
            //stdin contains input as name = value pairs.
            //no output needed

            try
            {
                //ignore command

                return ProgramExitCode.Success;
            }
            catch (Exception ex)
            {
                ProgramOutput.WriteError(ex.ToString());
                return ProgramExitCode.UnknownError;
            }
        }

        private static ProgramExitCode CredentialHelperCommandErase()
        {
            //stdin contains input as name = value pairs.
            //no output needed

            try
            {
                //ignore command

                return ProgramExitCode.Success;
            }
            catch (Exception ex)
            {
                ProgramOutput.WriteError(ex.ToString());
                return ProgramExitCode.UnknownError;
            }
        }

        static void Main(string[] args)
        {
            ProgramExitCode exitcode;

            try
            {
                string command;
                if (args.Length > 0)
                    command = args[0].ToLowerInvariant();
                else
                    command = "help";

                if (args.Length == 1 && args[0].ToLower() == "--version")
                {
                    var version = GetVersion();
                    Console.Write(version.Major + "." + version.Minor);
                    return;
                }

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
                    ProgramOutput.WriteHelp();
                    exitcode = ProgramExitCode.ShowHelp;
                }
                else
                {
                    exitcode = ProgramExitCode.UnknownCommand;
                }
            }
            catch (Exception ex)
            {
                ProgramOutput.WriteError(ex.ToString());
                exitcode = ProgramExitCode.UnknownError;
            }

            Environment.Exit((int)exitcode);
        }
    }
}
