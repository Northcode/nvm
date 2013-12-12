using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nvm2.compiler.ast;
using System.IO;
using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics;


namespace nvm2.compiler
{
    public class Context
    {
        public Context parent;
        public string name;
    }

    public class NILGenerator
    {
        string outputFile;

        //Assembly stuff
        AssemblyName name;
        AssemblyBuilder builder;
        ModuleBuilder moduleBuilder;

        TypeBuilder mainType;
        MethodBuilder mainMethod;
        Dictionary<string, LocalBuilder> mainSymbolTable;

        List<string> usings;

        stmt ast;

        public NILGenerator(stmt AST, string OutputFile)
        {
            if (System.IO.Path.GetFileName(OutputFile) != OutputFile)
            {
                throw new Exception("Can only output into current directory");
            }
            outputFile = OutputFile;
            name = new AssemblyName(Path.GetFileNameWithoutExtension(OutputFile));
            builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            moduleBuilder = builder.DefineDynamicModule(OutputFile);

            mainType = moduleBuilder.DefineType("mainType");
            mainMethod = mainType.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public);
            this.mainSymbolTable = new Dictionary<string, LocalBuilder>();

            usings = new List<string>();
            usings.Add("");
            usings.Add("System");

            ast = AST;
        }

        public void SaveAssembly()
        {
            mainType.CreateType();
            builder.SetEntryPoint(mainMethod);
            builder.Save(outputFile);
        }

        public void Generate()
        {
            GenerateIL(ast, null, null, mainSymbolTable);
        }

        public void GenerateClass(class_definition Stmt, Context context)
        {
            //Create type
            TypeBuilder classBuilder = moduleBuilder.DefineType(Stmt.name);
            //Add interface implementations
            if (Stmt.interfaces != null)
            {
                GetILNames(Stmt.interfaces).ToList().ForEach((i) => classBuilder.AddInterfaceImplementation(i));
            }
            //Start genetation of class
            GenerateIL(Stmt.body, null, context, null, null, classBuilder);

            //Finish class, generate it
            classBuilder.CreateType();
        }

        private IEnumerable<Type> GetILNames(string[] typeNames)
        {
            return typeNames.Select(t => GetILType(t));
        }

        public void GenerateMethod(function_definition Stmt, Context context, TypeBuilder Class)
        {

        }

        public void GenerateIL(stmt Stmt, ILGenerator generator, Context context, Dictionary<string,LocalBuilder> symbolTable, MethodBuilder Method = null, TypeBuilder Class = null)
        {
            ILGenerator ilGenerator;
            if (generator == null)
            {
                if (Method == null)
                {
                    ilGenerator = mainMethod.GetILGenerator();
                }
                else
                {
                    ilGenerator = Method.GetILGenerator();
                }
            }
            else
            {
                ilGenerator = generator;
            }
            
            if (Class != null)
            {
                if(Stmt is stmt_list)
                {
                    stmt_list list = Stmt as stmt_list; //Cast stmt
                    foreach (stmt s in list.list)
                    {
                        GenerateIL(s, null, context, null, null, Class);
                    }
                }
            }
            else
            {
                if (Stmt is stmt_list)
                {
                    stmt_list list = Stmt as stmt_list; //Cast stmt
                    foreach (stmt s in list.list)
                    {
                        GenerateIL(s, generator, context, symbolTable, Method, null);
                    }
                }
                else if (Stmt is declarevar)
                {
                    declarevar stmt = Stmt as declarevar;
                    mainSymbolTable[stmt.name] = ilGenerator.DeclareLocal(GetILType(stmt.type));

                    GenerateExpr(stmt.value, ilGenerator);
                    StoreLocal(stmt.name, ilGenerator);
                }
                else if (Stmt is setvar)
                {
                    setvar stmt = Stmt as setvar;
                    GenerateExpr(stmt.value, ilGenerator);
                    StoreLocal(stmt.name, ilGenerator);
                }
                else if (Stmt is function_call)
                {
                    function_call stmt = Stmt as function_call;
                    GenerateExpr(stmt.arg, ilGenerator);
                    //ilGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                    MethodInfo method = GetMethodInfo(stmt.name, Class, null, GetExprTypes(stmt.arg));
                    MethodInfo rmethod = Type.GetType("System.Console").GetMethod("WriteLine", new Type[] { typeof(string) });
                    if(method == rmethod)
                    {
                        Console.WriteLine("MATCH!");
                    }
                    ilGenerator.Emit(OpCodes.Call, method);
                }
                else if (Stmt is class_definition)
                {
                    class_definition stmt = Stmt as class_definition;
                    GenerateClass(stmt, context);
                }
            }
        }

        private Type[] GetExprTypes(expr expr)
        {
            if (expr is expr_list)
            {
                return (expr as expr_list).list.Select(e => GetExprType(e)).ToArray();
            }
            else if (expr == null)
            {
                return null;
            }
            else
            {
                return new Type[] { GetExprType(expr) };
            }
        }

        private Type GetExprType(expr e)
        {
            if (e is int_lit)
            {
                return typeof(int);
            }
            else if (e is float_lit)
            {
                return typeof(float);
            }
            else if (e is bool_lit)
            {
                return typeof(bool);
            }
            else if (e is byte_lit)
            {
                return typeof(byte);
            }
            else if (e is string_lit)
            {
                return typeof(string);
            }
            else if (e is getvar)
            {
                return GetVarType(e);
            }
            else if (e is function_call)
            {
                function_call stmt = e as function_call;
                return GetMethodInfo(stmt.name, null, null, GetMethodTypes(stmt.name, null, GetExprTypes(stmt.arg))).ReturnType;
            }
            else if (e == null)
            {
                return null;
            }
            else
            {
                throw new Exception("Cannot find argument type");
            }
        }

        private Type GetVarType(expr e)
        {
            throw new NotImplementedException();
        }

        private Type GetILType(string p)
        {
            if (p == "int")
            {
                return typeof(int);
            }
            else if (p == "float")
            {
                return typeof(float);
            }
            else if (p == "string")
            {
                return typeof(string);
            }
            else if (p == "bool")
            {
                return typeof(bool);
            }
            else if (p == "byte")
            {
                return typeof(byte);
            }
            else if (p == "object")
            {
                return typeof(object);
            }
            else
            {
                return Type.GetType(p);
            }
        }

        private Type[] GetMethodTypes(string methodname, TypeBuilder Class, Type[] parameterTypes)
        {
            return GetMethodInfo(methodname, Class, null, parameterTypes).GetParameters().Select(p => p.ParameterType).ToArray();
        }

        private MethodInfo GetMethodInfo(string p, TypeBuilder Class, Type type, Type[] parameters = null)
        {
            if(Class != null)
            {
                return Class.GetMethod(p);
            }
            else if (type != null)
            {
                return type.GetMethod(p);
            }
            else
            {
                mainType.CreateType();
                MethodInfo minfo = mainType.GetMethod(p);
                if (minfo != null)
                {
                    return minfo;
                }
                else
                {
                    if (p.Contains('.'))
                    {
                        string[] names = p.Split('.');
                        Type t = FindType(string.Join(".", names.Take(names.Length - 1)));

                        if (t != null)
                        {
                            return t.GetMethod(names[names.Length - 1], parameters);
                        }
                    }
                    throw new Exception("Cannot find method");
                }
            }
        }

        Type FindType(string name)
        {
            foreach (string use in usings)
            {
                name = (use != "" ? use + "." : "") + name;
                if (name.Contains('.'))
                {
                    name += ".";
                    int i = 0;
                    i = name.IndexOf('.');
                    string first = name.Substring(0, i);
                    Type t = null;
                    while ((t = Type.GetType(name.Substring(0, i))) == null)
                    {
                        i = name.IndexOf('.', i + 1);
                    }
                    return t;
                }
                else
                {
                    Type t = Type.GetType(name);
                    if (t != null) return t;
                }
            }
            throw new Exception("Type not found: " + name);
        }

        private void GenerateExpr(expr expr, ILGenerator ilGenerator)
        {
            if (expr is int_lit)
            {
                int_lit e = expr as int_lit;
                ilGenerator.Emit(OpCodes.Ldc_I4, e.value);
            }
            else if (expr is float_lit)
            {
                float_lit e = expr as float_lit;
                ilGenerator.Emit(OpCodes.Ldc_R4, e.value);
            }
            else if (expr is bool_lit)
            {
                bool_lit e = expr as bool_lit;
                ilGenerator.Emit(OpCodes.Ldc_I4, (e.value ? 1 : 0));
            }
            else if (expr is string_lit)
            {
                string_lit e = expr as string_lit;
                ilGenerator.Emit(OpCodes.Ldstr, e.value);
            }
            else if (expr is byte_lit)
            {
                byte_lit e = expr as byte_lit;
                ilGenerator.Emit(OpCodes.Ldc_I4, e.value);
            }
            else if (expr is getvar)
            {
                getvar e = expr as getvar;
                LoadLocal(e.name, ilGenerator);
            }
        }

        private void LoadLocal(string name, ILGenerator ilGenerator)
        {
            if (mainSymbolTable.ContainsKey(name))
            {

            }
        }

        private void StoreLocal(string name, ILGenerator il)
        {
            if (this.mainSymbolTable.ContainsKey(name))
            {
                il.Emit(OpCodes.Stloc, mainSymbolTable[name]);
            }
            else
            {
                throw new Exception("undeclared variable '" + name + "'");
            }
        }
    }
}
