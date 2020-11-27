namespace git_credential_keepasscommand
{
    public enum ProgramExitCode : int
    {
        Success = 0,
        GitInputError = 1,
        KeePassError = 2,
        KeePassEntryNotFound = 3,
        UnknownCommand = 97,
        ShowHelp = 98,
        UnknownError = 99
    }
}
