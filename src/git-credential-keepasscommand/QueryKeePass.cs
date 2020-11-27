using KeePassCommandDll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace git_credential_keepasscommand
{
    public static class QueryKeePass
    {
        private static QueryKeePassResult QueryKeePass_Exec(string title)
        {
            StringBuilder message;

            List<ApiGetResponse> found = null;
            try
            {
                found = KeePassCommandDll.Api.get(new List<string> { title });
            }
            catch (Exception ex)
            {
                message = new StringBuilder();
                message.AppendLine("Error querying KeePass for \"" + title + ":");
                message.AppendLine(ex.ToString());
                return QueryKeePassResult.Error(message, ProgramExitCode.KeePassError);
            }

            return QueryKeePassResult.Success(title, found);
        }

        public static QueryKeePassResult ForGitCredentials(NameValuePairs input)
        {
            StringBuilder message;

            NameValue curname = null;
            NameValue computername = null;
            try
            {
                var curdirFull = Directory.GetCurrentDirectory();
                var curdirName = new DirectoryInfo(curdirFull).Name; //use last directory part of "d:\projects\GitCredentialsViaKeePassCommander"

                curname = new NameValue("__CurrentName__", curdirName);
                computername = new NameValue("__ComputerName__", Environment.MachineName);
            }
            catch (Exception ex)
            {
                message = new StringBuilder();
                message.AppendLine("Error retrieving current directory and computername:");
                message.AppendLine(ex.ToString());
                return QueryKeePassResult.Error(message, ProgramExitCode.UnknownError);
            }

            List<QueryKeePassResult> execResults = new List<QueryKeePassResult>();
            QueryKeePassResult execResult;

            execResult = QueryKeePass_Exec("git [" + curname.Value.ToLowerInvariant() + "][" + computername.Value.ToLowerInvariant() + "]");
            if (execResult.IsError() || execResult.IsFoundValid()) return execResult;
            execResults.Add(execResult);

            execResult = QueryKeePass_Exec("git [" + curname.Value.ToLowerInvariant() + "]");
            if (execResult.IsError() || execResult.IsFoundValid()) return execResult;
            execResults.Add(execResult);

            message = new StringBuilder();
            message.AppendLine("Searched in KeePass for case-sensitive title (without quotes):");

            foreach (var result in execResults)
            {
                message.AppendLine("\"" + result.title + "\"");
                message.AppendLine(result.found.Count + " results found, should be 1 result");
                foreach (var entry in result.found)
                {
                    message.AppendLine(entry.Title);
                }
            }

            return QueryKeePassResult.Error(message, ProgramExitCode.KeePassEntryNotFound);
        }
    }
}
