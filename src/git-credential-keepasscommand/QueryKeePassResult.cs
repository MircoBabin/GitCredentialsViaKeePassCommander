using KeePassCommandDll;
using System;
using System.Collections.Generic;
using System.Text;

namespace git_credential_keepasscommand
{
    public class QueryKeePassResult
    {
        public StringBuilder message { get; }
        public ProgramExitCode? exitcode { get; }

        public string title { get; }
        public List<ApiGetResponse> found { get; }

        private QueryKeePassResult(StringBuilder message, ProgramExitCode? exitcode,
                                   string title, List<ApiGetResponse> found)
        {
            this.message = message;
            this.exitcode = exitcode;

            this.title = title;
            this.found = found;
        }

        public static QueryKeePassResult Error(StringBuilder message, ProgramExitCode? exitcode)
        {
            return new QueryKeePassResult(message, exitcode, null, null);
        }

        public static QueryKeePassResult Success(string title, List<ApiGetResponse> found)
        {
            return new QueryKeePassResult(null, null, title, found);
        }

        public bool IsError()
        {
            return (exitcode != null);
        }

        public bool IsFoundValid()
        {
            return (found != null && found.Count == 1);
        }


        public string GetFoundUsername()
        {
            if (!IsFoundValid())
                throw new Exception("Username: found not valid");

            return found[0].Username;
        }

        public string GetFoundPassword()
        {
            if (!IsFoundValid())
                throw new Exception("Password: password not valid");

            return found[0].Password;
        }
    }
}
