using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FluffMuff.Utils
{
    static class FluffExtensions
    {
        private static string pattern = @"[@'>*]";

        public static string Sanitize(this String input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return Regex.Replace(writer.ToString(), pattern, "");

                }
            }
        }
    }
}
