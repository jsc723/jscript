using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace expression
{
    
    
    public interface IExpression 
    {
        double eval(Frame frame = null);
        IExpression deriv(Variable x,ref Frame frame);
        IExpression simplify();
        string asString();
        string name { get; }
        int primarity { get; }
    }
    public abstract class ExpZero : IExpression
    {
        public abstract double eval(Frame frame);
        public abstract IExpression deriv(Variable v,ref Frame frame);
        public IExpression simplify() { return this; }
        public abstract string asString();
        public abstract string name { get; set; }
        public int primarity { get { return 5; } }
    }
    public class Null : ExpZero
    {
        public override double eval(Frame frame = null) { throw new Exception("Null can not be evaled"); }
        public override IExpression deriv(Variable v,ref Frame frame)
        { throw new Exception("Null cannot be derived"); }
        public override string asString() { return "null"; }
        public override string name { get { return asString(); } set { } }
    }
    public class Arr : ExpZero
    {
        public Arr()
        {
            array = new List<IExpression>();
        }
        public Arr(List<int> dims,int current = 0)
        {
            array = new List<IExpression>();
            if (current == dims.Count - 1)
            {
                for (int i = 0; i < dims[current]; i++)
                    array.Add(new Null());
            }
            else
            {
                for (int i = 0; i < dims[current]; i++)
                    array.Add(new Arr(dims, current + 1));
            }
        }
        public void Add(IExpression e) { array.Add(e); }
        public IExpression getItem(int i) { return array[i]; }
        public void setItem(int i, IExpression value) { array[i] = value; }
        public void setItem(List<int> indexs, int current, IExpression value)
        {
            if(current==indexs.Count-1)
                setItem(indexs[current], value);
            else
                ((Arr)array[indexs[current]]).setItem(indexs, current + 1, value);
        }
        public override double eval(Frame frame = null) { throw new Exception("Array can not be evaled"); }
        public override IExpression deriv(Variable v,ref Frame frame) 
        {
            Arr temp = new Arr();
            foreach (var e in array)
                temp.Add(e.deriv(v,ref frame));
            return temp;
        }
        public override string asString() 
        {
            List<Arr> temp = new List<Arr>();
            return asStringHelper(ref temp);
        }
        private string asStringHelper(ref List<Arr> note)
        {
            string buf = "{ ";
            for (int i = 0; i < array.Count; i++)
            {
                if (i > 0) buf += ", ";
                if (array[i] is Arr)
                {
                    if (note.Contains(array[i]))
                        buf += "{...}";
                    else
                    {
                        note.Add((Arr)array[i]);
                        buf += ((Arr)array[i]).asStringHelper(ref note);
                    }
                }
                else
                {
                    buf += array[i].asString();
                }
            }
            buf += " }";
            return buf;
        }
        public override string name { get; set; }
        public List<IExpression> array;
    }
    public class Str : ExpZero
    {
        public Str(string value = "")
        {
            this.value = value;
        }
        public override double eval(Frame frame = null) { throw new Exception("String cannot be evaled"); }
        public override IExpression deriv(Variable v,ref Frame frame)
        { throw new Exception("String cannot be derived"); }
        public override string asString() { return value; }
        public override string name { get; set; }
        public string value;
    }
    public class Number : ExpZero
    {
        public Number(double num)
        {
            value = num;
        }
        public double eval() { return value; }
        public override double eval(Frame frame = null){ return value; }
        public override IExpression deriv( Variable v ,ref Frame frame) { return new Number(0); }
        public override string asString() { return Math.Round(value,Tools.precise).ToString(); }
        public override string name { get { return asString(); } set { } }
        private double value;
    }
    public class Variable : ExpZero
    {
        public Variable(string name)
        {
            this.name = name;
        }
        public override double eval(Frame frame)
        {
            if (frame.containsVarValue(name))
                return frame.getVarValue(name);
            if (frame.containsFunc(name))
                return frame.getFunc(name).eval(frame);
            throw new Exception("variable\""+name+"\"undefined");
        }
        public override IExpression deriv(Variable v,ref Frame frame)
        {
            if (name == v.name)
                return new Number(1);
            if (frame.containsFunc(name))
            {
                return frame.getFunc(name).deriv(v, ref frame);
            }
            return new Number(0); 
        }
        public override string asString() { return name; }
        public override string name { get; set; }
    }
    
    public class Tools
    {
        static public int precise = 3;
        static public string[] fours = { "sin", "cos", "tan", "cot", "exp", "log", "ln","int","eval", "not"};
        static public string[] threes = { "^" };
        static public string[] twos = { "*","/","%" };
        static public string[] ones = { "+", "-" };
        static public string[] zeros = { "==", ">>", "<<", "!=","<=",">=" };//这里改了记得去toTokens里面加上
        static public string[] negones = { "&&", "||" };
        static public string[] negtwos = { "=" };
        static public string[] paras = { "(", ")","{","}"};
        static public Frame globalFrame = new Frame();
        public static void print(IExpression e) { Console.WriteLine(e.asString()); }
        
        public static bool charIsNumber(char c)
        {
            return c > '0' && c < '9';
        }
        public static Number getCoef(IExpression e)
        {
            e = e.simplify();
            if (e is Number) return (Number)e;
            if (e is Mul)
            {
                Mul m = (Mul)e;
                if (m.u is Number) return (Number)m.u;
            }
            if (e is Div)
            {
                Div d = (Div)e;
                if (d.v is Number) return new Number(1 / d.v.eval());
            }
            return new Number(1);
        }
        public static IExpression getNonCoef(IExpression e)
        {
            if (e is Number) return new Number(1);
            if (e is Mul)
            {
                Mul m = (Mul)e;
                if (m.u is Number) return m.v;
            }
            if (e is Div)
            {
                Div d = (Div)e;
                if (d.v is Number) return d.u;
            }
            return e;
        }
        //equalStruct: cos(u(3x)) == 2cos(u(3x)), cos(u(3x)) == cos(u(3x))/3 , 2 == 3,cos(u(3x)) != cos(u(x))
        //是不是同类项
        public static bool equalStruct(IExpression e1, IExpression e2)
        {
            e1 = e1.simplify(); e2 = e2.simplify();
            if (e1 is ExpZero && e2 is ExpZero)
            {
                if (e1 is Number && e2 is Number) return true;
                if (e1 is Variable && e2 is Variable && e1.asString() == e2.asString()) return true;
            }
            if (e1 is ExpOne && e2 is ExpOne)
            {
                if (e1.name == e2.name) return (equal(((ExpOne)e1).u, ((ExpOne)e2).u));
            }
            if (equal(getNonCoef(e1), getNonCoef(e2))) return true;
            if (e1 is ExpTwo && e2 is ExpTwo)
            {
                if( ((e1 is Mul && e2 is Mul) || (e1 is Add && e2 is Add))
                     &&(equalStruct(((ExpTwo)e1).u, ((ExpTwo)e2).v) && equalStruct(((ExpTwo)e1).v, ((ExpTwo)e2).u)))
                    return true;
                return (equalStruct(((ExpTwo)e1).u, ((ExpTwo)e2).u) && equalStruct(((ExpTwo)e1).v, ((ExpTwo)e2).v));
            }
            return false;
        }
        //是不是完全相等
        public static bool equal(IExpression e1, IExpression e2)
        {
            e1 = e1.simplify(); e2 = e2.simplify();
            if (e1 is ExpZero && e2 is ExpZero)
            {
                if (e1 is Number && e2 is Number) return ((Number)e1).eval() == ((Number)e2).eval();
                if (e1 is Variable && e2 is Variable && e1.name == e2.name) return true;
            }
            if (e1 is ExpOne && e2 is ExpOne)
            {
                if (e1.name == e2.name) return (equal(((ExpOne)e1).u, ((ExpOne)e2).u));
            }
            if (e1 is ExpTwo && e2 is ExpTwo)
            {
                if (((e1 is Mul && e2 is Mul) || (e1 is Add && e2 is Add))
                     && (equal(((ExpTwo)e1).u, ((ExpTwo)e2).v) && equal(((ExpTwo)e1).v, ((ExpTwo)e2).u)))
                    return true;
                if(e1.name==e2.name)
                    return (equal(((ExpTwo)e1).u, ((ExpTwo)e2).u) && equal(((ExpTwo)e1).v, ((ExpTwo)e2).v));
            }
            return false;
        }
        
        public static IExpression makeAdd(IExpression u, IExpression v)
        {
            u = u.simplify(); v = v.simplify();
            if (u is Number && ((Number)u).eval() == 0) return v;
            if (v is Number && ((Number)v).eval() == 0) return u;
            if (v is Number && u is Number) return new Number(v.eval() + u.eval());
            if (equalStruct(u, v))return makeMul(new Number(getCoef(u).eval() + getCoef(v).eval()), getNonCoef(u));
            double vcoef = getCoef(v).eval();
            if (vcoef < 0) return makeSub(u, makeMul(new Number(-1 * vcoef), getNonCoef(v)));
            return new Add(u, v);
        }
        public static IExpression makeSub(IExpression u, IExpression v)
        {
            u = u.simplify(); v = v.simplify();
            if (u is Number && ((Number)u).eval() == 0) return makeMul(new Number(-1), v);
            if (v is Number && ((Number)v).eval() == 0) return u;
            if (u is Number && v is Number) return new Number(u.eval() - v.eval());
            if (equalStruct(u, v)) return makeMul(new Number(getCoef(u).eval() - getCoef(v).eval()), getNonCoef(u));
            double vcoef = getCoef(v).eval();
            if (vcoef < 0) return makeAdd(u, makeMul(new Number(-1 * vcoef), getNonCoef(v)));
            return new Sub(u, v);
        }
        public static IExpression makeMul(IExpression u, IExpression v)
        {
            if (v is Number && !(u is Number)) { IExpression t = u; u = v; v = t; }
            u = u.simplify(); v = v.simplify();
            //print(u); print(v);
            if (u is Number && ((Number)u).eval() == 0) return new Number(0);
            if (v is Number && ((Number)v).eval() == 0) return new Number(0);
            if (u is Number && ((Number)u).eval() == 1) return v;
            if (v is Number && ((Number)v).eval() == 1) return u;
            if (v is Number && u is Number)  return new Number(v.eval() * u.eval());
            double vcoef = getCoef(v).eval();
            if (u is Number && vcoef != 1) 
                return makeMul(new Number(u.eval() * vcoef), getNonCoef(v));
            return new Mul(u, v);
        }
        public static IExpression makeDiv(IExpression u, IExpression v)
        {
            u = u.simplify(); v = v.simplify();
            if (u is Number && u.eval() == 0) return new Number(0);
            if (u is Number && v is Number) return new Number(u.eval() / v.eval());
            if (equal(getNonCoef(u), getNonCoef(v)))
                return new Number(getCoef(u).eval() + getCoef(v).eval());
            return new Div(u, v);
        }
        public static IExpression makeMod(IExpression u, IExpression v)
        {
            u = u.simplify(); v = v.simplify();
            return new Mod(u, v);
        }
        public static IExpression makePow(IExpression u, IExpression v)
        {
            u = u.simplify(); v = v.simplify();
            if (u is Number && v is Number) return new Number(Math.Pow(u.eval() , v.eval()));
            if (u is Number && u.eval() == 0) return new Number(0);
            if (v is Number && v.eval() == 0) return new Number(1);
            if (v is Number && v.eval() == 1) return u;
            return new Pow(u, v);
        }
        public static IExpression makeLog(IExpression u, IExpression v)
        {
            u = u.simplify(); v = v.simplify();
            if (u is Number && v is Number) return new Number(Math.Log(v.eval() , u.eval()));
            if (v is Number && v.eval() == 1) return new Number(0);
            if (equal(u, v)) return new Number(1);
            return new Log(u, v);
        }
        public static IExpression makeExp(IExpression u)
        {
            u = u.simplify();
            if (u is Number && u.eval() == 0) return new Number(1);
            return new Exp(u);
        }
        public static bool isNotExpression(string[] cond)
        {
            if (cond.Length < 4 || cond[0] != "not")
                return false;
            return Program.isWholeArg(Program.subTokens(cond, 1));
        }
        public static bool determine(string[] cond, Frame frame)
        {
            if (Program.isWholeArg(cond))
                return determine(Program.subTokens(cond, 1, cond.Length - 2), frame);
            if (isNotExpression(cond))
                return !determine(Program.subTokens(cond, 2, cond.Length - 3), frame);
            if (cond.Length == 1)
            {
                if (cond[0] == "0" || cond[0] == "false")
                    return false;
                if (cond[0] == "true")
                    return true;
                return (Program.makeExpr(cond, ref frame).eval(frame) != 0) ? true : false;
            }
            int i = Program.goForwardArg(cond, 0);
            string mid = cond[i];
            string[] sleft = Program.subTokens(cond, 0, i),sright = Program.subTokens(cond, i + 1);
            if (mid == "&&")
                return determine(sleft, frame) && determine(sright,frame);
            else if(mid == "||")
                return determine(sleft, frame) || determine(sright,frame);
                
            IExpression left, right;
            left = Program.makeExpr(sleft, ref frame);
            right = Program.makeExpr(sright, ref frame);
            if (left is Str && right is Str)
            {
                Str sl = (Str)left;
                Str sr = (Str)right;
                if (mid == "==")
                    return sl.asString().CompareTo(sr.asString()) == 0;
                else if (mid == "!=")
                    return sl.asString().CompareTo(sr.asString()) != 0;
                else if (mid == "<<")
                    return sl.asString().CompareTo(sr.asString()) < 0;
                else if (mid == ">>")
                    return sl.asString().CompareTo(sr.asString()) > 0;
                else if (mid == "<=")
                    return sl.asString().CompareTo(sr.asString()) <= 0;
                else if (mid == ">=")
                    return sl.asString().CompareTo(sr.asString()) >= 0;
            }
            else if (mid == "==")
            {
                if (equal(left, right)) return true;
                if (left.eval(frame) == right.eval(frame)) return true;
                return false;
            }
            else if (mid == "!=")
            {
                if (equal(left, right)) return false;
                if (left.eval(frame) == right.eval(frame)) return false;
                return true;
            }
            else if (mid == "<<")
                return (left.eval(frame) < right.eval(frame));
            else if (mid == ">>")
                return (left.eval(frame) > right.eval(frame));
            else if (mid == "<=")
                return (left.eval(frame) <= right.eval(frame));
            else if (mid == ">=")
                return (left.eval(frame) >= right.eval(frame));
            return false;
        }
    }
}
