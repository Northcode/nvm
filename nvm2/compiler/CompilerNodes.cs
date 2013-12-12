using nvm2.compiler.ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvm2.compiler.nodes
{
    class XAssembly
    {
        List<XClass> internalclasses;

        public XAssembly()
        {
            internalclasses = new List<XClass>();
        }

        internal XClass GetClass(string s)
        {
            if(internalclasses.Any((p) => p.name == s))
            {
                return internalclasses.Find((p) => p.name == s);
            }
            else
            {
                return new XClass(s, this);
            }
        }
    }

    class XClass
    {
        public string name;
        public List<XClass> inheritance;

        public List<XMethod> methods;

        public XAssembly assembly;

        public XClass(string name, XAssembly assembly)
        {
            this.name = name;
            this.assembly = assembly;
        }

        public Type GetDotNetType()
        {

        }
    }

    class XNClass : XClass
    {
        public XNClass(class_definition Class,XAssembly assembly) : base(Class.name,assembly)
        {
            this.name = Class.name;
            this.inheritance = Class.interfaces.Select(s => assembly.GetClass(s)).ToList();
        }
    }

    class XMethod
    {
        public string name;

    }
}
