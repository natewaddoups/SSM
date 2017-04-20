///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Emad Barsoum, repackaged as unit tests by Nate Waddoups
// ExpressionEvaluatorTest.cs
///////////////////////////////////////////////////////////////////////////////
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections;
using System.IO;
using EB.Math;

namespace SsmDisplayTest
{
    [TestClass()]
    public class ExpressionEvaluatorTest
    {
        [TestMethod()]
        public void ExpEvalSimple()
        {
            //-- Example 1: Simple --
            Function fn = new Function();
            fn.Parse("1+2");
            fn.Infix2Postfix();
            fn.EvaluatePostfix();
            double nResult = fn.Result;
            Assert.AreEqual((double) 3, nResult);
        }

        [TestMethod()]
        public void ExpEvalBuiltIn()
        {
            //-- Example 2: How to use built-in function --
            Function fn = new Function();
            fn.Parse("1+2*5-cos(pi)");
            fn.Infix2Postfix();
            fn.EvaluatePostfix();
            double nResult = fn.Result;
            Assert.AreEqual((double) 12, nResult);
        }

        [TestMethod()]
        public void ExpEvalVariables()
        {
            //-- Example 3: How to use the variable --
            Function fn = new Function();
            fn.Parse("x+2*y-4");
            fn.Infix2Postfix();
            ArrayList var = fn.Variables;
            for(int i = 0; i < var.Count; i++)
            {
                Symbol sym = (Symbol) var[i];
                if (sym.m_name == "x")
                {
                    sym.m_value = 1;
                }
                else if (sym.m_name == "y")
                {
                    sym.m_value = 3;
                }
            }
            fn.Variables = var;
            fn.EvaluatePostfix();
            double nResult = fn.Result;
            Assert.AreEqual((double) 3, nResult);
        }

        [TestMethod()]
        public void ExpEvalVariableNames()
        {
            //-- Example 3: How to use the variable --
            Function fn = new Function();
            fn.Parse("P1_foo+P2:bar");
            fn.Infix2Postfix();
            ArrayList var = fn.Variables;
            for (int i = 0; i < var.Count; i++)
            {
                Symbol sym = (Symbol)var[i];
                if (sym.m_name == "P1_foo")
                {
                    sym.m_value = 1;
                }
                else if (sym.m_name == "P2:bar")
                {
                    sym.m_value = 2;
                }
            }
            fn.Variables = var;
            fn.EvaluatePostfix();
            double nResult = fn.Result;
            Assert.AreEqual((double)3, nResult);
        }

        [TestMethod()]
        public void ExpEvalRecurse()
        {
            //-- Example 4: How to use the recursive function call --
            Function fn = new Function();
            fn.Parse("sin(cos(pi)*pi/2)");
            fn.Infix2Postfix();
            fn.EvaluatePostfix();
            double nResult = fn.Result;
            Assert.AreEqual((double) -1, nResult);
        }

        //-- Example 5: How to extend the built-in existing function
        //   using a delegate --

        //-- Let's say you want to add a function called Add that takes
        //   two numbers and returns the sum of both numbers
        //-- The function will be implemented like that --

        public static Symbol EvaluateFnc(string name, params Object[] args)
        {
            Symbol result = new Symbol();
            result.m_name = "";
            result.m_type = EB.Math.Type.Result;
            result.m_value = 0;
            switch (name)
            {
                case "add":
                    if (args.Length == 2)
                    {
                        result.m_name = name + "(" +
                        ((Symbol)args[0]).m_value.ToString() + ")";
                        result.m_value = ((Symbol)args[0]).m_value +
               ((Symbol)args[1]).m_value;
                    }
                    else
                    {
                        result.m_name = "Invalid number of parameters in:" + name + ".";
                        result.m_type = EB.Math.Type.Error;
                    }
                    break;
                default:
                    {
                        result.m_name = "Function: " + name + "not found.";
                        result.m_type = EB.Math.Type.Error;
                    }
                    break;
            }
            return result;
        }

        //-- After that, to register this function in the Function
        //   class, do the following:
        [TestMethod()]
        public void ExpEvalExtension()
        {
            Function fn = new Function();
            fn.DefaultFunctionEvaluation = new
            EvaluateFunctionDelegate(EvaluateFnc);
        }

        [TestMethod()]
        public void ExpEvalBitwiseOperators()
        {
            Function fn = new Function();
            fn.Parse("x&8");
            fn.Infix2Postfix();
            ArrayList var = fn.Variables;
            ((Symbol)var[0]).m_value = 8;
            fn.Variables = var;
            fn.EvaluatePostfix();
            double nResult = fn.Result;
            Assert.AreEqual((double)8, nResult, "x&8, x = 8");

            fn = new Function();
            fn.Parse("x&8");
            fn.Infix2Postfix();
            var = fn.Variables;
            ((Symbol)var[0]).m_value = 9;
            fn.Variables = var;
            fn.EvaluatePostfix();
            nResult = fn.Result;
            Assert.AreEqual((double)8, nResult, "x&8, x = 9");

            fn = new Function();
            fn.Parse("x&8");
            fn.Infix2Postfix();
            var = fn.Variables;
            ((Symbol)var[0]).m_value = 1;
            fn.Variables = var;
            fn.EvaluatePostfix();
            nResult = fn.Result;
            Assert.AreEqual((double)0, nResult, "x&8, x = 1");

            fn = new Function();
            fn.Parse("x&(2^3)");
            fn.Infix2Postfix();
            var = fn.Variables;
            ((Symbol)var[0]).m_value = 9;
            fn.Variables = var;
            fn.EvaluatePostfix();
            nResult = fn.Result;
            Assert.AreEqual((double)8, nResult, "x&(2^3), x = 9");
        }
    }


}
