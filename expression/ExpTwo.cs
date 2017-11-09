using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace expression
{
    public abstract class ExpTwo : IExpression
    {
        public virtual string asString()
        {
            bool omit = false,needpr = false;
            if (u is Number && (v is Variable || v is ExpOne)) omit = true;
            string left = u.asString(), right = v.asString(),mid = this.name;
            if (needpr || u.primarity < this.primarity) left = "(" + left + ")";
            if (needpr || v.primarity <= this.primarity) right = "(" + right + ")";
            if (mid == "*" && omit && !(Tools.charIsNumber(left[left.Length - 1]) && Tools.charIsNumber(right[0])))
                mid = "";
            return left + mid + right;
        }
        public abstract double eval(Frame frame);
        public abstract IExpression deriv(Variable x,ref Frame frame);
        public abstract IExpression simplify();
        public IExpression u,v;
        public string name { get; set; }
        public abstract int primarity { get; }
    }
    public class Add : ExpTwo
    {
        public Add(IExpression e1, IExpression e2)
        {
            u = e1; v = e2; name = "+"; 
        }
        public override double eval(Frame frame = null)
        { return u.eval(frame) + v.eval(frame); }
        public override int primarity { get { return 1; } }
        public override IExpression deriv(Variable x ,ref Frame frame)
        { return Tools.makeAdd(u.deriv(x,ref frame), v.deriv(x,ref frame)); }
        public override IExpression simplify()
        {
            return Tools.makeAdd(u, v);
        }
    }
    public class Sub : ExpTwo
    {
        public Sub(IExpression e1, IExpression e2)
        { u = e1; v = e2; name = "-"; }
        public override double eval(Frame frame = null)
        { return u.eval(frame) - v.eval(frame); }
        public override int primarity { get { return 1; } }
        public override IExpression deriv(Variable x,ref Frame frame)
        { return Tools.makeSub(u.deriv(x,ref frame), v.deriv(x,ref frame)); }
        public override IExpression simplify()
        {
            return Tools.makeSub(u, v);
        }
    }
    public class Mul : ExpTwo
    {
        public Mul(IExpression e1, IExpression e2)
        { u = e1; v = e2; name = "*"; }
        public override double eval(Frame frame = null)
        { return u.eval(frame) * v.eval(frame); }
        public override int primarity { get { return 2; } }
        public override IExpression deriv(Variable x,ref Frame frame) 
        { return Tools.makeAdd(Tools.makeMul(u.deriv(x,ref frame),v), Tools.makeMul(u, v.deriv(x,ref frame))); }
        public override IExpression simplify()
        {
            return Tools.makeMul(u, v);
        }
    }
    public class Div : ExpTwo
    {
        public Div(IExpression e1, IExpression e2)
        { u = e1; v = e2; name = "/"; }
        public override double eval(Frame frame = null)
        { return u.eval(frame) / v.eval(frame); }
        public override int primarity { get { return 2; } }
        public override IExpression deriv(Variable x, ref Frame frame)
        {
            IExpression t1 = u.deriv(x, ref frame);
            IExpression n = Tools.makeSub(Tools.makeMul(u.deriv(x, ref frame), v), Tools.makeMul(u, v.deriv(x, ref frame)));
            IExpression d = Tools.makePow(v, new Number(2));
            return Tools.makeDiv(n, d);
        }
        public override IExpression simplify()
        {
            return Tools.makeDiv(u, v);
        }
    }
    public class Mod : ExpTwo
    {
        public Mod(IExpression e1, IExpression e2)
        { u = e1; v = e2; name = "%"; }
        public override double eval(Frame frame = null)
        { return (int)u.eval(frame) % (int)v.eval(frame); }
        public override int primarity { get { return 2; } }
        public override IExpression deriv(Variable x, ref Frame frame)
        {
            throw new Exception("求余运算不能求导");
        }
        public override IExpression simplify()
        {
            return Tools.makeMod(u, v);
        }
    }
    public class Pow : ExpTwo
    {
        public Pow(IExpression e1, IExpression e2)
        { u = e1; v = e2; name = "^"; }
        public override double eval(Frame frame = null)
        { return  Math.Pow(u.eval(frame) , v.eval(frame)); }
        public override int primarity { get { return 3; } }
        public override IExpression deriv(Variable x,ref Frame frame)
        {
            if (v is Number)
            {
                Number k = new Number(v.eval() - 1);
                IExpression d = Tools.makeMul(v,Tools.makePow(u,k));
                return Tools.makeMul(d, u.deriv(x,ref frame));
            }
            else if (u is Number)
            {
                return Tools.makeMul(Tools.makeMul(new Number(Math.Log(u.eval())), this), v.deriv(x,ref frame));
            }
            else
            {
                IExpression t1 = Tools.makeMul(v.deriv(x,ref frame), new Ln(u));
                IExpression t2 = Tools.makeMul(Tools.makeDiv(v, u), u.deriv(x,ref frame));
                return Tools.makeMul(this, Tools.makeAdd(t1, t2));
            }
        }
        public override IExpression simplify()
        {
            return Tools.makePow(u, v);
        }
    }
    public class Log : ExpTwo
    {
        public Log(IExpression e1, IExpression e2)
        { u = e1; v = e2; name = "log"; }
        public override double eval(Frame frame = null)
        { return Math.Log(v.eval(frame),u.eval(frame)); }
        public override int primarity { get { return 4; } }
        public override IExpression deriv(Variable x,ref Frame frame)
        {
            if (u is Number)
            {
                IExpression t = Tools.makeMul(v, new Ln(u));
                IExpression t1 = Tools.makeDiv(v.deriv(x, ref frame), t);
                return t1;
            }
            else
            {
                IExpression t = Tools.makeDiv(new Ln(v), new Ln(u));
                return t.deriv(x,ref frame);
            }
        }
        public override string asString()
        {
            bool omit = false;
            if (u is Number && (v is Variable || v is ExpOne)) omit = true;

            string left = "log("+u.asString(), right = v.asString()+")", mid = ", ";
            if (mid == "*" && omit && !(Tools.charIsNumber(left[left.Length - 1]) && Tools.charIsNumber(right[0])))
                mid = "";
            return left + mid + right;
        }
        public override IExpression simplify()
        {
            return Tools.makeLog(u, v);
        }
    }

    
    
}
