using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace git_credential_keepasscommand
{
    public class NameValuePairs
    {
        public ReadOnlyCollection<NameValue> pairs { get; }

        public NameValuePairs(ReadOnlyCollection<NameValue> pairs)
        {
            this.pairs = pairs;
        }

        public static NameValuePairs fromStdin()
        {
            //stdin contains input as name = value pairs.
            var input = new List<NameValue>();

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
                var pos_is = line.IndexOf('=');
                if (pos_is >= 0)
                {
                    string name = String.Empty;
                    string value = String.Empty;

                    if (pos_is > 0) name = line.Substring(0, pos_is).Trim();
                    if (pos_is + 1 < line.Length) value = line.Substring(pos_is + 1).Trim();

                    input.Add(new NameValue(name, value));
                }
            }

            return new NameValuePairs(input.AsReadOnly());
        }
    }
}
