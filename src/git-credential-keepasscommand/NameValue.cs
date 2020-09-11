using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace git_credential_keepasscommand
{
    public class NameValue
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public NameValue(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}
