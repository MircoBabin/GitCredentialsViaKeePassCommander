namespace git_credential_keepasscommand
{
    public class NameValue
    {
        public string Name { get; }
        public string Value { get; }

        public NameValue(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}
