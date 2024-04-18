using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ExpressionHelper
{
    object instance = null;
    MethodInfo method = null;
    public ExpressionHelper(string expression)
    {
        if (expression.IndexOf("return") < 0)
        {
            expression = "return " + expression + ";";
        }
        string className = "ExpressionHelper";
        string methodName = "Compute";
        CompilerParameters p = new CompilerParameters();
        p.GenerateInMemory = true;
        CompilerResults cr = new CSharpCodeProvider().CompileAssemblyFromSource(p, string.Format("using System;sealed class {0}{{public double {1}(double v){{{2}}}}}", className, methodName, expression));
        if (cr.Errors.Count <= 0)
        {
            instance = cr.CompiledAssembly.CreateInstance(className);
            method = instance.GetType().GetMethod(methodName);
        }
    }

    public double Compute(double x)
    {
        if (method != null && instance != null)
        {
            return (double)method.Invoke(instance, new object[] { x });
        }
        else
        {
            return x;
        }
    }
}
