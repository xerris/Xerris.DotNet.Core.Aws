using System;
using System.Collections.Generic;
using System.Linq;

namespace Xerris.DotNet.Core.Aws.Secrets
{
    public sealed class SecretConfigCollection
    {
        public IEnumerable<SecretConfig> Items { get; set; }

        public SecretConfig GetConfig(string name)
        {
            return Items.First(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}