using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace expression
{
    
    public abstract class ExpOne : IExpression
    {
        public string asString() { return name + "(" + u.asString() + ")"; }
        public abstract double eval(Frame frame);
        public abstract IExpression deriv(Variable v,ref Frame frame);
        public abstract IExpression simplify();
        public IExpression u;
        public string name{get;set;}
        public int primarity { get { return 4; } }
    }
    public class Exp :ExpOne
    {
        public Exp(IExpression e) { u = e; name = "exp"; }
        public override double eval(Frame frame) { return Math.Exp(u.eval(frame)); }
        public override IExpression deriv(Variable v,ref Frame frame)
        { return Tools.makeMul(u.deriv(v,ref frame), new Exp(u)); }
        public override IExpression simplify(){return new Exp(u.simplify()); }
    }
    public class Ln : ExpOne
    {
        public Ln(IExpression e) { u = e; name = "ln"; }
        public override double eval(Frame frame) { return Math.Log(u.eval(frame)); }
        public override IExpression deriv(Variable v,ref Frame frame)
        { return Tools.makeMul(Tools.makeDiv(new Number(1),u),u.deriv(v,ref frame)); }
        public override IExpression simplify() { return new Ln(u.simplify()); }
    }
    public class Sin : ExpOne
    {
        public Sin(IExpression e) { u = e; name = "sin"; }
        public override double eval(Frame frame) { return Math.Sin(u.eval(frame)); }
        public override IExpression deriv(Variable v,ref Frame frame)
        { return Tools.makeMul(u.deriv(v,ref frame), new Cos(u)); }
        public override IExpression simplify() { return new Sin(u.simplify()); }
    }
    public class Cos : ExpOne
    {
        public Cos(IExpression e) { u = e; name = "cos"; }
        public override double eval(Frame frame) { return Math.Cos(u.eval(frame)); }
        public override IExpression deriv(Variable v,ref Frame frame) 
        { return Tools.makeMul(new Number(-1), Tools.makeMul(u.deriv(v,ref frame), new Sin(u))); }
        public override IExpression simplify() { return new Cos(u.simplify()); }
    }
    public class Tan : ExpOne
    {
        public Tan(IExpression e) { u = e; name = "tan"; }
        public override double eval(Frame frame) { return Math.Tan(u.eval(frame)); }
        public override IExpression deriv(Variable v,ref Frame frame) 
        {
            IExpression sec = Tools.makeDiv(new Number(1), new Cos(v));
            IExpression m = Tools.makePow(sec, new Number(2));
            return Tools.makeMul(u.deriv(v,ref frame), m);
        }
        public override IExpression simplify() { return new Tan(u.simplify()); }
    }
    public class Int : ExpOne
    {
        public Int(IExpression e) { u = e; name = "int"; }
        public override double eval(Frame frame) { double i = u.eval(frame); return (int)i; }
        public override IExpression deriv(Variable v,ref Frame frame) { throw new Exception("int函数不能求导"); }
        public override IExpression simplify() { return new Int(u.simplify()); }
    }
}
