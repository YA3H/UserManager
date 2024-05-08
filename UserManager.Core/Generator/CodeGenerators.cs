using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Core.Generator
{
    public class CodeGenerators
    {
        public static string ActiveCode()
        {
            Random random = new Random();
            return random.Next(100000, 999000).ToString();
        }

    }
}
