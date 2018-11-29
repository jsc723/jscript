using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace expression
{
    public class Frame
    {
        public Dictionary<string, Variable> vars = new Dictionary<string, Variable>();
        public Dictionary<string, double> varValues = new Dictionary<string, double>();
        public Dictionary<string, IExpression> funcs = new Dictionary<string, IExpression>();
        public Dictionary<string, Procedure> proces = new Dictionary<string, Procedure>();
        public Dictionary<string, Str> strs = new Dictionary<string, Str>();
        public Dictionary<string, Arr> arrs = new Dictionary<string,Arr>();
        public Frame parent;

        public bool containsVar(string key)
        {
            if(Program.stringisKeyInDic(key,vars))
                return true;
            if(parent != null)
                return parent.containsVar(key);
            return false;
        }
        public bool containsVarValue(string key)
        {
            if(Program.stringisKeyInDic<double>(key,varValues))
                return true;
            if(parent != null)
                return parent.containsVarValue(key);
            return false;
        }
        public bool containsFunc(string key)
        {
            if(Program.stringisKeyInDic(key,funcs))
                return true;
            if(parent != null)
                return parent.containsFunc(key);
            return false;
        }
        public bool containsProc(string key)
        {
            if (Program.stringisKeyInDic(key, proces))
                return true;
            if (parent != null)
                return parent.containsProc(key);
            return false;
        }
        public bool containsStr(string key)
        {
            if (Program.stringisKeyInDic(key, strs))
                return true;
            if (parent != null)
                return parent.containsStr(key);
            return false;
        }
        public bool containsArr(string key)
        {
            if (Program.stringisKeyInDic(key, arrs))
                return true;
            if (parent != null)
                return parent.containsArr(key);
            return false;
        }

        public Variable getVar(string key)
        {
            if (Program.stringisKeyInDic(key, vars))
                return vars[key];
            if (parent != null)
                return parent.getVar(key);
            return null;
        }
        public double getVarValue(string key)
        {
            if (Program.stringisKeyInDic(key, varValues))
                return varValues[key];
            if (parent != null)
                return parent.getVarValue(key);
            return 0;
        }
        public void setVarValue(string key,double value)
        {
            if (!Program.stringisKeyInDic(key, vars))
            {
                if (parent != null)
                    parent.setVarValue(key, value);
                else
                    throw new Exception("cannot find：" + key);
            }
            if (!Program.stringisKeyInDic(key, varValues))
                varValues.Add(key, value);
            varValues[key] = value;
        }
        public IExpression getFunc(string key)
        {
            if (Program.stringisKeyInDic(key, funcs))
                return funcs[key];
            if (parent != null)
                return parent.getFunc(key);
            return null;
        }
        public void setFunc(string key, IExpression value)
        {
            if (!Program.stringisKeyInDic(key, funcs))
            {
                if (parent != null)
                    parent.setFunc(key, value);
                else
                    throw new Exception("cannot find：" + key);
            }
            funcs[key] = value;
        }
        public Procedure getProc(string key)
        {
            if (Program.stringisKeyInDic(key, proces))
                return proces[key];
            if (parent != null)
                return parent.getProc(key);
            return null;
        }
        public void setProc(string key, Procedure value)
        {
            if (!Program.stringisKeyInDic(key, proces))
            {
                if (parent != null)
                    parent.setProc(key, value);
                else
                    throw new Exception("cannot find：" + key);
            }
            proces[key] = value;
        }
        public Str getStr(string key)
        {
            if (Program.stringisKeyInDic(key, strs))
                return strs[key];
            if (parent != null)
                return parent.getStr(key);
            return null;
        }
        public void setStr(string key, Str value)
        {
            if (!Program.stringisKeyInDic(key, strs))
            {
                if (parent != null)
                    parent.setStr(key, value);
                else
                    throw new Exception("cannot find：" + key);
            }
            strs[key] = value;
        }
        public Arr getArr(string key)
        {
            if (Program.stringisKeyInDic(key, arrs))
                return arrs[key];
            if (parent != null)
                return parent.getArr(key);
            return null;
        }
        public void setArr(string key, Arr value)
        {
            if (!Program.stringisKeyInDic(key, arrs))
            {
                if (parent != null)
                    parent.setFunc(key, value);
                else
                    throw new Exception("cannot find：" + key);
            }
            arrs[key] = value;
        }

        public void showFrame()
        {
            Console.WriteLine("---------------var----------------");
            foreach (Variable v in vars.Values)
            {
                Console.Write("{0}", v.name);
                if (Program.stringisKeyInDic<double>(v.name, varValues))
                    Console.Write(" = {0}", varValues[v.name]);
                Console.Write("; ");
            }
            Console.WriteLine("\n---------------str----------------");
            foreach (var s in strs)
            {
                Console.Write("{0}： ", s.Key);
                Tools.print(s.Value);
            }
            Console.WriteLine("\n---------------arr----------------");
            foreach (var s in arrs)
            {
                Console.Write("{0}： ", s.Key);
                Tools.print(s.Value);
            }
            Console.WriteLine("\n---------------func----------------");
            foreach (var p in funcs)
            {
                Console.Write("{0} = ", p.Key);
                Tools.print(p.Value);
            }
            Console.WriteLine("\n---------------proc----------------");
            foreach (var p in proces)
                Console.Write("{0}，", p.Key);
            Console.WriteLine();
            if(parent!=null)
                parent.showFrame();
        }
        public Frame()
        {
            parent = null;
        }
        public Frame(ref Frame p)
        {
            parent = p;
        }
    }
    public class Block
    {
        public string[] tokens;
        public Block(string[] ts)
        {
            tokens = ts;
        }
    }
    
    public class IfBlock
    {
        public List<Block> blocks;
        public List<string[]> conditions;
        public IfBlock(ref string[] sentence)
        {
            conditions = new List<string[]>();
            blocks = new List<Block>();
            int i = 0, j = 0;
            do
            {
                i  = j + 1;
                if (sentence[j] == "else")
                {
                    if (j == 0)
                        throw new Exception("expect if before else");
                    string[] t_cond = { "true" };
                    conditions.Add(t_cond);
                    j = i;
                }
                else
                {
                    j = Program.goForwardArg(sentence, i);
                    string[] cond = Program.subTokens(sentence, i + 1, j - i - 2);
                    conditions.Add(cond);
                }
                i = j;
                if (sentence[i] != "{") throw new Exception("if/elif/else expect {}");
                j = Program.goForwardArg(sentence, j,"{}");
                string[] token = Program.subTokens(sentence, i + 1, j - i - 2);
                Block b = new Block(token);
                blocks.Add(b);
            } while (j < sentence.Length);
        }
        public IExpression run(ref Frame frame, ref bool end)
        {
            IExpression ret = new Null();
            for (int i = 0;i<conditions.Count;i++)
            {
                if (Tools.determine(conditions[i],frame))
                {
                    Program.processTokens(blocks[i].tokens, ref frame, out ret, ref end);
                    break;
                }
            }
            return ret;
        }
    }
    public class WhileBlock
    {
        public Block block;
        public string[] condition;
        public WhileBlock(string[] condi, Block b)
        {
            block = b;
            condition = condi;
        }
        public IExpression run(ref Frame frame,ref bool end)
        {
            IExpression ret = new Null();
            try
            {
                while (Tools.determine(condition, frame))
                    Program.processTokens(block.tokens, ref frame, out ret,ref end);
            }
            catch(Exception e)
            {
                if (e.Message != "breakException")
                    throw e;
            }
            return ret;
        }
    }
    public class ForBlock
    {
        public Block block;
        private string[] first, cond, last;
        public ForBlock(string[] head, Block b)
        {
            block = b;
            List<String[]> todos = Program.getAllArgs(Program.subTokens(head,1,head.Length-2), ";");
            if (todos.Count != 3)
                throw new Exception("for syntax error");
            first = Program.insertToken(todos[0], todos[0].Length, ";");
            cond = todos[1];
            last = Program.insertToken(todos[2], todos[2].Length, ";");
        }
        public IExpression run(ref Frame frame,ref bool end)
        {
            IExpression ret = new Null();
            Program.processTokens(first, ref frame, out ret, ref end);
            try
            {
                while (Tools.determine(cond, frame))
                {
                    Program.processTokens(block.tokens, ref frame, out ret, ref end);
                    Program.processTokens(last, ref frame, out ret, ref end);
                }
            }
            catch(Exception e)
            {
                if (e.Message != "breakException")
                    throw e;
            }
            return ret;
        }
    }
    public class Procedure : IExpression
    {
        private Block block;
        private List<string[]> args;
        private Frame parentFrame;
        public Procedure(string name, string[] arg_names,Block b,ref Frame frame)
        {
            this.name = name;
            this.args = Program.splitTokens(arg_names,",");
            this.block = b;
            this.parentFrame = frame;
        }
        public IExpression call(string[] allvalues,ref Frame frame)
        {
            List<string[]> values = Program.getAllArgs(allvalues);
            Frame funcFrame = new Frame(ref parentFrame);
            IExpression ret;
            if(values.Count!=args.Count || args[0].Length==0 && values[0].Length!=0)
                throw new Exception("wrong argument number");
            for(int i=0;i<args.Count&&args[0].Length!=0;i++)
            {
                string[] p = args[i];
                if (p.Length != 2)
                    throw new Exception("wrong parameter list");
                string type = p[0], an = p[1];
                switch (type)
                {
                    case "var":
                        funcFrame.vars.Add(an, new Variable(an));
                        funcFrame.varValues.Add(an, Program.makeExpr(values[i],ref frame).eval(frame));
                        break;
                    case "func":
                        funcFrame.funcs.Add(an, Program.makeExpr(values[i], ref frame));
                        break;
                    case "proc":
                        funcFrame.proces.Add(an, frame.getProc(an));
                        break;
                    case "str":
                        funcFrame.strs.Add(an, (Str)Program.makeExpr(values[i], ref frame));
                        break;
                    case "arr":
                        funcFrame.arrs.Add(an, (Arr)Program.makeExpr(values[i],ref frame));
                        break;
                    default:
                        throw new Exception("unknown type：" + type);
                }
            }
            bool end = false;
            Program.processTokens(block.tokens, ref funcFrame,out ret,ref end,false);
            return ret;
        }
        public double eval(Frame frame = null) { throw new Exception("Proc cannot be evaled"); }
        public IExpression deriv(Variable x, ref Frame frame) { throw new Exception("Proc cannot be derived"); }
        public IExpression simplify() { throw new Exception("Proc cannot be simplified"); }
        public string asString() { return ToString(); }
        public string name { get; set; }
        public int primarity { get { return 4; } }
    }
}
