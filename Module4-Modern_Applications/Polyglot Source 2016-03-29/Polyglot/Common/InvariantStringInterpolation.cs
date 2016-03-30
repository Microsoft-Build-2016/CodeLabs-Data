using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class InvariantString
    {
        public static string Invariant(IFormattable formattable)
        {
            Argument.CheckIfNull(formattable, nameof(formattable));
            return formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
