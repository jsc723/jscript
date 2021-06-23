using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
/*
 * TODO
 * ------2.0.b1------
 * - not() operation                                [DONE]
 * - rules about eval and print                     [DONE]
 * - for                                            [DONE]
 * ------2.0.b2------
 * - var default value                              [DONE]
 * - combine return and return obj                  [DONE]
 * - recursive higher order function call           [DONE]
 * - standardlize syscalls                          [DONE]
 * ------2.0.b3------
 * - eval                                           []
 * - ICallable                                      []
 * - class                                          []
 */
namespace expression

{
    class Program
    {
        static bool __debug__ = false;
        static bool __step__ = false;
        static bool __forcePath__ = false;
        static string __path__ = "tictactoe.jsc";
        static string __version__ = "2.0.beta-2";
        static string __myname__ = "Sicheng Jiang";
        static string __myEmail__ = "jscjsc723723@163.com";
        static string __releaseTime__ = "2018.11.27";
        static public string[] mergeArray(string[] First, string[] Second)
        {
            string[] result = new string[First.Length + Second.Length];
            First.CopyTo(result, 0);
            Second.CopyTo(result, First.Length);
            return filterTokens(result);
        }
        static public string[] mergeArray(string[] Source, string str)
        {
            string[] result = new string[Source.Length + 1];
            Source.CopyTo(result, 0);
            result[Source.Length] = str;
            return result;
        }
        static public string[] mergeArray(string str ,string[] Source)
        {
            string[] result = new string[Source.Length + 1];
            result[0] = str;
            Source.CopyTo(result, 1);
            return result;
        }
        static public string[] filterTokens(string[] tokens)
        {
            int n = 0;
            foreach (var s in tokens)
                if (s.Length == 0)
                    n++;
            string[] result = new string[tokens.Length - n];
            int i = 0;
            foreach (var s in tokens)
            {
                if (s.Length > 0)
                {
                    result[i] = s;
                    i++;
                }
            }
            return result;
        }
        static public string[] subTokens(string[] src, int start, int len = -1)
        {
            if (len == -1) len = src.Length - start;
            if (len < 0) len = 0;
            if (start > src.Length - 1) return new string[0];
            string[] result = new string[len];
            for (int i = 0; i < len; i++)
                result[i] = src[start + i];
            return result;
        }
        static public string[] insertToken(string[] tokens, int index, string t)
        {
            if (index < 0)
                return mergeArray(t, tokens);
            if (index > tokens.Length - 1)
                return mergeArray(tokens, t);
            return mergeArray(mergeArray(subTokens(tokens, 0, index), t), subTokens(tokens, index));
        }

        static public void printTokens(string[] tokens)
        {
            foreach (string s in tokens)
                Console.Write("{0} ", s);
            Console.WriteLine();
        }

        static public int firstIndexInTokens(string s, string[] tokens)
        {
            for (int i = 0; i < tokens.Length; i++)
                if (tokens[i] == s) return i;
            return -1;
        }
        /* 
         * one arg: [a-zA-Z][a-zA-Z0-9]*(\(.*\))*  
         */
        static public int goForwardArg(string[] tokens, int start,string para = "()", bool multiPara = true)
        {
            int i=start;
            string left = para[0].ToString();
            string right = para[1].ToString();
            if (tokens[i] == "=")
                return i + 1;
            if (prioToken(tokens[i]) == 5)
            {
                if (i + 1 >= tokens.Length)
                    return i + 1;
                if (tokens[i + 1] != "(")
                    return i + 1;
                i++;
            }
            if (prioToken(tokens[i]) == 4) i++;
            if (tokens[i] != left) throw new Exception("expect (");
            do
            {
                int count = 1;
                while (count != 0)
                {
                    i++;
                    if (i >= tokens.Length) throw new Exception(para + " not matched");
                    if (tokens[i] == left) count++;
                    else if (tokens[i] == right) count--;
                }
                i++;
            } while (i < tokens.Length && tokens[i] == left && multiPara);
            return i;
        }
        /* get the begining for the arg ends at start (inclusive) */
        static public int goBackArg(string[] tokens, int start, string para = "()")
        {
            int i = start;
            string left = para[0].ToString();
            string right = para[1].ToString();
            if (prioToken(tokens[i]) == 5) return i;
            if (tokens[i] != right) throw new Exception("expect " + right);
            do
            {
                int count = -1;
                while (count != 0)
                {
                    i--;
                    if (i < 0) throw new Exception(para + " not matched");
                    if (tokens[i] == "(") count++;
                    else if (tokens[i] == ")") count--;
                }
                i--;
            } while (i >= 0 && tokens[i] == right);
            i++;
            if (i>0 && prioToken(tokens[i-1]) >= 4)i--;
            return i;
        }

        static public bool tokenIsNumber(string token)
        {
            foreach(var c in token)
            {
                if (!((c >= '0' && c <= '9') || c == '.' || c == '-'))
                    return false;
            }
            return true;
        }

        static public bool isWholeArg(string[] tokens)
        {
            if (tokens[0] != "(")
                return false;
            int i = 0;
            i = goForwardArg(tokens, 0);
            return i == tokens.Length;
        }

        public static List<string[]> getAllArgs(string[] all_args, string delim = ",")
        {
            all_args = Program.addPara(all_args);
            List<string[]> ss = splitTokens(all_args, delim);
            if (ss.Count == 0)
                ss.Add(new string[0]);
            return ss;
        }

        static public int prioToken(string token)
        {
            if (firstIndexInTokens(token, Tools.fours) >= 0) return 4;
            if (firstIndexInTokens(token, Tools.threes) >= 0) return 3;
            if (firstIndexInTokens(token, Tools.twos) >= 0) return 2;
            if (firstIndexInTokens(token, Tools.ones) >= 0) return 1;
            if (firstIndexInTokens(token, Tools.zeros) >= 0) return 0;
            if (firstIndexInTokens(token, Tools.negones) >= 0) return -1;
            if (firstIndexInTokens(token, Tools.negtwos) >= 0) return -2;
            if (firstIndexInTokens(token, Tools.paras) >= 0) return -3;
            if (firstIndexInTokens(token, Tools.negFours) >= 0) return -4;
            return 5;
        }

        static public List<string[]> splitTokens(string[] tokens, string key)
        {
            List<string[]> result = new List<string[]>();
            int i = 0;
            while (i < tokens.Length)
            {
                if (i < tokens.Length && tokens[i] == key)
                {
                    result.Add(subTokens(tokens, 0, i));
                    tokens = subTokens(tokens, i + 1);
                    i = 0;
                }
                i = goForwardArg(tokens, i);
            }
            result.Add(tokens);
            return result;
        }

        static public string[] readCommond()
        {
            string buf;
            string[] tokens = new string[0];
            Console.Write(">> ");
            bool next = false;
            do
            {
                if(next) Console.Write("  ");
                buf = Console.ReadLine();
                if (buf == "END") throw new Exception("END");
                tokens = mergeArray(tokens, toTokens(buf));
                next = true;
            } while (!(paraMatchs(tokens, "{}") && paraMatchs(tokens)));
            return tokens;
        }

        static public string[] readCommondInFile(StreamReader sr)
        {
            string buf;
            string[] tokens = new string[0];
            do
            {
                buf = sr.ReadLine();
                if (buf==null||buf == "END") throw new Exception("END");
                tokens = mergeArray(tokens, toTokens(buf));
            } while (!(paraMatchs(tokens, "{}") && paraMatchs(tokens)));
            return tokens;
        }

        static public string[] processTokens(string[] tokens,ref Frame frame,out IExpression ret,ref bool end ,bool print = true)
        {
            int i = 0;
            string[] sentence = new string[0];
            IExpression r = new Null();
            end = false;
            while(true)
            {
                i = firstIndexInTokens(";", tokens);
                if (i == -1)
                    break;
                sentence = mergeArray(sentence, subTokens(tokens, 0, i));
                tokens = subTokens(tokens, i + 1);
                if (!paraMatchs(sentence, "{}") || !paraMatchs(sentence, "()"))
                {
                    sentence = mergeArray(sentence, ";");
                    continue;
                }
                if (sentence.Length == 0)
                    break;
                r = processSentence(sentence,ref frame,ref end);
                if (r.asString() != "null" && print ) 
                    Console.WriteLine(r.asString());
                sentence = new string[0];
                if (end)
                    break;
            }
            ret = r;
            return tokens;
        }

        static public IExpression processSentence(string[] sentence,ref Frame frame,ref bool end)
        {
            if (__debug__)
            {
                foreach (var s in sentence)
                    Console.Write("{0} ", s);
                Console.WriteLine();
            }
            if (__step__)
            {
                Console.ReadKey();
            }
            if (!paraMatchs(sentence)) throw new Exception("parentheses not matched");
            sentence = addPara(sentence);
            if (isWholeArg(sentence))
                return makeExpr(subTokens(sentence, 1, sentence.Length - 2),ref frame);
            if (sentence[0] == "var")
            {
                string[] args = subTokens(sentence, 1);
                List<string[]> devidedArgs = getAllArgs(args);
                foreach (var term in devidedArgs)
                {
                    frame.vars.Remove(term[0]);
                    frame.vars.Add(term[0], new Variable(term[0]));
                    frame.varValues.Remove(term[0]);
                    if (term.Length >1)
                        frame.varValues.Add(term[0], makeExpr(subTokens(term, 2), ref frame).eval(frame));
                    else
                        frame.varValues.Add(term[0], 0); //default value is 0
                }
            }
            else if (sentence[0] == "arr")
            {
                string[] args = subTokens(sentence, 1);
                List<string[]> devidedArgs = getAllArgs(args);
                foreach (var term in devidedArgs)
                {
                    frame.arrs.Remove(term[0]);
                    if (term.Length == 1)
                        frame.arrs.Add(term[0], new Arr());
                    else
                    {
                        string[] rest = subTokens(term, 2, term.Length - 3);
                        List<string[]> dargs = getAllArgs(rest);
                        List<int> iargs = new List<int>();
                        foreach (var a in dargs)
                            iargs.Add((int)(makeExpr(a, ref frame).eval(frame)));
                        frame.arrs.Add(term[0], new Arr(iargs));
                    }
                }
            }
            else if (sentence[0] == "str")
            {
                string[] args = subTokens(sentence, 1);
                List<string[]> devidedArgs = getAllArgs(args);
                foreach (var term in devidedArgs)
                {
                    frame.strs.Remove(term[0]);
                    if (term.Length == 1)
                        frame.strs.Add(term[0], new Str());
                    else if (term.Length > 1)
                    {
                        frame.strs.Add(term[0], (Str)makeExpr(subTokens(term,2),ref frame));
                    }
                    else
                        throw new Exception("str format error");
                }
            }
            else if (sentence[0] == "func")
            {
                IExpression e = makeExpr(subTokens(sentence, 3), ref frame);
                frame.funcs.Remove(sentence[1]);
                frame.varValues.Remove(sentence[1]);
                frame.funcs.Add(sentence[1], e);
            }
            else if (sentence[0] == "proc")
            {
                string funcName = sentence[1];
                if (sentence[2] == "=")
                {
                    IExpression e = makeExpr(subTokens(sentence, 3), ref frame);
                    if (!(e is Procedure))
                        throw new Exception("expect return a proc object");
                    frame.proces.Remove(sentence[1]);
                    frame.proces.Add(sentence[1], (Procedure)e);
                    return new Null();
                }
                if (sentence[2] != "(") throw new Exception("expect a parameter list");
                int i = goForwardArg(sentence, 2);
                string[] args = subTokens(sentence, 3, i - 4);
                if (sentence[i] != "{") throw new Exception("expect {}");
                int j = goForwardArg(sentence, i, "{}");
                string[] token = subTokens(sentence, i + 1, j - i - 2);
                Block b = new Block(token);
                frame.proces.Remove(funcName);
                frame.proces.Add(funcName, new Procedure(funcName, args, b,ref frame));
            }
            else if (sentence[0] == "if")
            {
                return new IfBlock(ref sentence).run(ref frame, ref end);
            }
            #region for/while
            else if (sentence[0] == "while" || sentence[0] == "for")
            {
                if (sentence[1] != "(") throw new Exception("while/for expect ()");
                int i = goForwardArg(sentence, 1);
                string[] cond = subTokens(sentence, 1, i - 1);
                if (sentence[i] != "{") throw new Exception("while/for expect {}");
                int j = goForwardArg(sentence, i, "{}");
                string[] token = subTokens(sentence, i + 1, j - i - 2);
                Block b = new Block(token);
                if (sentence[0] == "while")
                {
                    return new WhileBlock(cond, b).run(ref frame, ref end);
                }
                else
                {
                    return new ForBlock(cond, b).run(ref frame, ref end);
                }
            }
            #endregion
            else if (sentence[0] == "print")
            {
                if (sentence.Length < 1 || sentence[1] != "(") throw new Exception("print expect ()");
                int k = goForwardArg(sentence, 1);
                string[] args = subTokens(sentence, 2, k - 3);
                List<string[]> devidedArgs = getAllArgs(args);
                foreach (var a in devidedArgs)
                {
                    //string b = makeExpr(a, ref frame).asString().Replace("\\n", "\n");
                    IExpression e = makeExpr(a, ref frame);
                    string b = "";
                    try
                    {
                        b = e.eval(frame).ToString();
                    }
                    catch
                    {
                        b = e.asString().Replace("\\n", "\n");
                    }
                    Console.Write("{0}", b);
                }
            }
            else if (sentence[0] == "inputNum")
            {
                if (sentence[1] != "(")
                    throw new Exception("inputNum expect ()");
                List<string[]> args = splitTokens(subTokens(sentence, 2, sentence.Length - 3), ",");
                if (args.Count != 1)
                    throw new Exception("inputNum wrong argument number");
                if (tokenIsNumber(args[0][0]))
                    throw new Exception("inputNum argument must be a variable");
                if (frame.containsVar(args[0][0]))
                    frame.setVarValue(args[0][0], Convert.ToDouble(Console.ReadLine()));
                else throw new Exception("variable\"" + args[0][0] + "\"is not defined");
            }
            else if (sentence.Length >= 3 && sentence[1] == "=")
            {
                if (frame.containsVar(sentence[0]))
                {
                    frame.setVarValue(sentence[0], makeExpr(subTokens(sentence, 2), ref frame).eval(frame));
                    if (frame.containsFunc(sentence[0]))
                    {
                        frame.funcs.Remove(sentence[0]);
                    }
                }
                else if (frame.containsStr(sentence[0]))
                {
                    frame.setStr(sentence[0], (Str)makeExpr(subTokens(sentence, 2), ref frame));
                }
                else if (frame.containsArr(sentence[0]))
                {
                    if (sentence[1] == "=")
                    {
                        frame.setArr(sentence[0], (Arr)makeExpr(subTokens(sentence, 2), ref frame));
                    }
                    else
                    {
                        int k = firstIndexInTokens("=", sentence);
                        string[] args = subTokens(sentence, 2, k - (2 + 1));
                        string[] value = subTokens(sentence, k + 1);
                        List<string[]> devidedArgs = getAllArgs(args);
                        List<int> iargs = new List<int>();
                        foreach (var a in devidedArgs)
                            iargs.Add((int)(makeExpr(a, ref frame).eval(frame)));
                        IExpression temp = makeExpr(value, ref frame);
                        double t = 0;
                        bool is_num = false;
                        try
                        {
                            t = makeExpr(value,ref frame).eval(frame);
                            is_num = true;
                        }
                        catch { }
                        if (is_num)
                            frame.getArr(sentence[0]).setItem(iargs, 0, new Number(t));
                        else
                            frame.getArr(sentence[0]).setItem(iargs, 0, temp);
                    }
                }
                else if (frame.containsProc(sentence[0]))
                {
                    IExpression e = makeExpr(subTokens(sentence, 2), ref frame);
                    if (!(e is Procedure))
                        throw new Exception("expect return a proc object");
                    frame.setProc(sentence[0], (Procedure)e);
                }
            }

            else if (sentence[0] == "append")//把一个对象加到数组后面，如果没有指定加什么，默认加Null TODO
            {
                if (!frame.containsArr(sentence[1]))
                    throw new Exception("can not find array：" + sentence[1]);
                IExpression temp;
                if (sentence.Length > 2)
                    temp = makeExpr(subTokens(sentence, 2), ref frame);
                else
                    temp = new Null();
                frame.arrs[sentence[1]].Add(temp);
            }
            else if (sentence[0] == "return")
            {
                end = true;
                string[] sub = subTokens(sentence, 1);
                if (sub.Length == 1 && frame.containsFunc(sub[0]))
                    return frame.getFunc(sub[0]);
                IExpression r;
                try
                {
                    r = new Number(makeExpr(sub, ref frame).eval(frame));
                }
                catch
                {
                    r = makeExpr(sub, ref frame);
                }
                return r;
            }
            else if (sentence[0] == "break")
            {
                throw new Exception("breakException");
            }
            else if (sentence[0] == "determine")
            {
                if (sentence.Length == 1 || !isWholeArg(subTokens(sentence, 1)))
                    throw new Exception("syntax error in determine: expect determine(...)");
                Console.WriteLine("{0}", Tools.determine(subTokens(sentence, 2, sentence.Length - 3), frame));
            }
            else if (sentence[0] == "enviro") 
            {
                if(sentence.Length == 1 || !isWholeArg(subTokens(sentence, 1)))
                    throw new Exception("syntax error in enviro: expect enviro()");
                frame.showFrame();
            }
            else if (sentence[0] == "deriv" || sentence[0] == "eval")
            {
                return makeExpr(sentence, ref frame);
            }
            else if (sentence[0] == "pause") 
            {
                if (sentence.Length == 1 || !isWholeArg(subTokens(sentence, 1)))
                    throw new Exception("syntax error in pause: expect pause(<expr>)");
                sentence[0] = "print";
                processSentence(sentence, ref frame, ref end);
                Console.ReadKey();
            }
            else if (sentence[0] == "breakpoint")
            {

            }
            else if (sentence[0] == "delete") //TODO
            {
                if (sentence.Length != 4 || sentence[1] != "(" || sentence[3] != ")")
                    throw new Exception("syntax error in delete: expect delete(<name>)");
                string name = sentence[2];
                frame.vars.Remove(name);
                frame.varValues.Remove(name);
                frame.strs.Remove(name);
                frame.arrs.Remove(name);
                frame.funcs.Remove(name);
                frame.proces.Remove(name);
            }
            else if (frame.containsVar(sentence[0]))
                return new Number(frame.getVar(sentence[0]).eval(frame));
            else if (frame.containsStr(sentence[0]))
                return frame.getStr(sentence[0]);
            else if (frame.containsFunc(sentence[0]))
            {
                if (sentence.Length == 1)
                    return frame.getFunc(sentence[0]);
                return makeExpr(sentence, ref frame);
            }
            else if (frame.containsProc(sentence[0]))
            {
                return makeExpr(sentence, ref frame);
            }
            else if (frame.containsArr(sentence[0]))
            {
                return makeExpr(sentence, ref frame);
            }
            else
            {
                throw new Exception("can not recognize the command：" + sentence[0]);
            }
            return new Null();
        }

        static public bool stringisKeyInDic<T>(string key , Dictionary<string,T> dic)
        {
            foreach (var p in dic)
                if (key == p.Key) return true;
            return false;
        }

        static public string[] addPara(string[] sentence)
        {
            int i = 0,j = 0;
            List<string[]> allKeys = new List<string[]>();
            allKeys.Add(Tools.fours);
            allKeys.Add(Tools.threes);
            allKeys.Add(Tools.twos);
            allKeys.Add(Tools.ones);
            allKeys.Add(Tools.zeros);
            allKeys.Add(Tools.negones);
            foreach (var level in allKeys)
            {
                foreach (var key in level)
                {
                    i = j = 0;
                    while (i < sentence.Length)
                    {
                        if (sentence[i] == key)
                        {
                            if (firstIndexInTokens(key, Tools.fours) < 0)
                                j = goBackArg(sentence, i - 1);
                            else
                                j = i;
                            int k = goForwardArg(sentence, i + 1);
                            if (j-1 >= 0 && sentence[j-1] == "(" && k<sentence.Length && sentence[k]==")" )
                            {
                                i++;
                                continue;
                            }
                            sentence = insertToken(sentence, j, "(");
                            i++;
                            j = goForwardArg(sentence, i + 1);
                            sentence = insertToken(sentence, j, ")");
                        }
                        i++;
                    }
                }
            }
            return sentence;
        }
        static public IExpression makeExpr(string[] sentence,ref Frame frame)
        {
            if (!paraMatchs(sentence)) throw new Exception("() not matched");
            
            IExpression arg1, arg2;
            int i,j;
            if (sentence.Length==1)
            {
                if (sentence[0][0] == '\"')
                {
                    string temp = sentence[0].Substring(1, sentence[0].IndexOf('\"', 1) - 1);
                    return new Str(temp);
                }
                if(prioToken(sentence[0]) != 5)
                    throw new Exception("syntax error");
                if(tokenIsNumber(sentence[0]))
                    return new Number(Convert.ToDouble(sentence[0]));
                if (frame.containsVar(sentence[0]))
                    return frame.getVar(sentence[0]);
                else if (frame.containsFunc(sentence[0]))
                    return frame.getFunc(sentence[0]);
                else if (frame.containsStr(sentence[0]))
                    return frame.getStr(sentence[0]);
                else if (frame.containsArr(sentence[0]))
                    return frame.getArr(sentence[0]);
                else if (frame.containsProc(sentence[0]))
                    return frame.getProc(sentence[0]);
                return new Variable(sentence[0]);
            }
            if (isWholeArg(sentence))
            {
                return makeExpr(subTokens(sentence, 1, sentence.Length - 2), ref frame);
            }
            if (sentence[0] == "deriv")
            {
                if (sentence.Length<1||sentence[1] != "(") throw new Exception("deriv expect ()");
                int k = goForwardArg(sentence, 1);
                string[] args = subTokens(sentence, 2, k - 3);
                List<string[]> devidedArgs = getAllArgs(args);
                if (devidedArgs.Count != 2)
                    throw new Exception("deriv takes 2 arguments");
                if (devidedArgs[1].Length != 1)
                    throw new Exception("deriv 2nd arguments have to be a single var");
                if (!frame.containsVar(devidedArgs[1][0]))
                    throw new Exception("variable\"" + devidedArgs[1][0] + "\"is not defined");
                if (devidedArgs.Count == 2)
                    return makeExpr(devidedArgs[0], ref frame).deriv(frame.getVar(devidedArgs[1][0]),ref frame);
            }
            if (sentence[0] == "eval")
            {
                if (sentence.Length < 1 || sentence[1] != "(") throw new Exception("eval expect ()");
                int k = goForwardArg(sentence, 1);
                string[] args = subTokens(sentence, 2, k - 3);
                List<string[]> devidedArgs = getAllArgs(args);
                //TODO: extend eval's frame
                return new Number(makeExpr(devidedArgs[0], ref frame).eval(frame));
            }
            if (sentence[0] == "return")
            {
                throw new Exception("return syntax error: try 'return(<expr>)'");
            }
            if (frame.containsProc(sentence[0]) && goForwardArg(sentence, 1) >= sentence.Length)
            {
                if (sentence[1] != "(") throw new Exception("proc expect ()");
                int k = 1, m;
                IExpression r = frame.getProc(sentence[0]);
                do
                {
                    Procedure proc = (Procedure)r;
                    m = goForwardArg(sentence, k, "()", false);
                    string[] args = subTokens(sentence, k + 1, m - k - 2);
                    r = proc.call(args, ref frame);
                    k = m;
                } while (k < sentence.Length);
                return r;
            }
            else if(frame.containsFunc(sentence[0]) && isWholeArg(subTokens(sentence, 1)))
            {
                if (sentence[1] != "(") throw new Exception("func expect ()");
                int k = goForwardArg(sentence, 1);
                string[] args = subTokens(sentence, 2, k - 3);
                Frame funcFrame = new Frame(ref frame, args);
                return new Number(frame.getFunc(sentence[0]).eval(funcFrame));
            }
            else if (frame.containsArr(sentence[0]) && isWholeArg(subTokens(sentence, 1)))
            {
                int k = goForwardArg(sentence, 1);
                string[] args = subTokens(sentence, 2, k - 3);
                List<string[]> devided = getAllArgs(args);
                int[] indexs = new int[devided.Count];
                for (k = 0; k < devided.Count; k++)
                    indexs[k] = (int)(makeExpr(devided[k], ref frame).eval(frame));
                IExpression current = frame.getArr(sentence[0]);
                for (k = 0; k < indexs.Length; k++)
                {
                    current = ((Arr)current).array[indexs[k]];
                }
                return current;
            }
            else if (firstIndexInTokens(sentence[0], Tools.fours) >= 0)
            {
                if (sentence[1] != "(")
                    throw new Exception("expect (");
                if (sentence[0] == "log")
                {
                    i = goForwardArg(sentence, 2);
                    arg1 = makeExpr(subTokens(sentence, 2, i - 2), ref frame);
                    j = goForwardArg(sentence, i + 1);
                    arg2 = makeExpr(subTokens(sentence, i + 1, j - i - 1), ref frame);
                    return Tools.makeLog(arg1, arg2);
                }
                else
                {
                    i = goForwardArg(sentence, 1);
                    arg1 = makeExpr(subTokens(sentence, 2, i - 3), ref frame);
                    string s = sentence[0];
                    if (s == "sin")
                        return new Sin(arg1);
                    if (s == "cos")
                        return new Cos(arg1);
                    if (s == "tan")
                        return new Tan(arg1);
                    if (s == "exp")
                        return Tools.makeExp(arg1);
                    if (s == "ln")
                        return new Ln(arg1);
                    if (s == "int")
                        return new Int(arg1);
                    else
                        throw new Exception("unknown function：" + s);
                }
            }
            else
            {
                i = goForwardArg(sentence, 0);
                j = goForwardArg(sentence, i + 1);
                arg1 = makeExpr(subTokens(sentence, 0, i), ref frame);
                arg2 = makeExpr(subTokens(sentence, i + 1, j - i - 1), ref frame);
                string s = sentence[i];
                if (s == "+")
                    return Tools.makeAdd(arg1, arg2);
                if (s == "-")
                    return Tools.makeSub(arg1, arg2);
                if (s == "*")
                    return Tools.makeMul(arg1, arg2);
                if (s == "/")
                    return Tools.makeDiv(arg1, arg2);
                if (s == "%")
                    return Tools.makeMod(arg1, arg2);
                if (s == "^")
                    return Tools.makePow(arg1, arg2);
                else
                    throw new Exception("unknown operation：" + s);
            }
        }
        static public bool paraMatchs(string[] sentence , string para = "()")
        {
            int count = 0;
            foreach (string s in sentence)
            {
                if (s.Length > 0)
                {
                    if (s[0] == para[0]) count++;
                    if (s[0] == para[1]) count--;
                }
            }
            return count == 0;
        }
        static public string[] toTokens(string buf)
        {
            string[] result = new string[0];
            buf = processComment(buf);
            List<string> quotes = processQuote(buf, out buf);
            string[] range = new string[] { "(", ")", "+", "-", "*", "/","%", "^","=&", ",", ";",
                "{", "}",",","<<",">>","==","!=","<=",">=","&&","||","\""};
            char[] cbuf = buf.ToCharArray();
            for (int i = 0; i < cbuf.Length-1; i++)
            {
                if (cbuf[i] == '-')
                {
                    if (i == 0)
                        cbuf[i] = '~';
                    else if (cbuf[i + 1] != ' ')
                    {
                        if (!(cbuf[i - 1] >= '0' && cbuf[i - 1] <= '9' || cbuf[i - 1] >= 'a' && cbuf[i - 1] <= 'z'))
                            cbuf[i] = '~';
                    }
                }
                else if (cbuf[i] == '<' && cbuf[i + 1] != '=')
                    cbuf[i] = '@';
                else if (cbuf[i] == '>' && cbuf[i + 1] != '=')
                    cbuf[i] = '$';
                else if (cbuf[i] == '=' && cbuf[i - 1] != '=' && cbuf[i + 1] != '='&&
                    cbuf[i - 1] != '<' && cbuf[i - 1] != '>' && cbuf[i - 1] != '!')
                    cbuf[i] = '`';
            }
            buf = new string(cbuf);
            buf = buf.Replace("@", "<<").Replace("$", ">>").Replace('[','(').Replace(']',')').Replace("`","=&");
            foreach (string s in range)
            {
                buf = buf.Replace(s, " " + s + " ");
            }
            buf = buf.Replace('~', '-').Replace("=&","=");
            while (buf.Length > 0)
            {
                buf = buf.Trim();
                int i = 0;
                while (i < buf.Length && buf[i] != ' ') i++;
                string token = buf.Substring(0, i);
                buf = buf.Substring(i);
                result = mergeArray(result, token);
            }
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].Length>5 && result[i].Substring(0, 5) == "_REP_")
                {
                    int k = Convert.ToInt32(result[i].Substring(5));
                    result[i] = quotes[k];
                }
            }
            return result;
        }
        public static string processComment(string buf)
        {
            int i = 0;
            string temp;
            while (true)
            {
                i = buf.IndexOf('#');
                if (i < 0) break;
                temp = buf.Substring(i, buf.Length - i);
                buf = buf.Replace(temp, "");
            }
            return buf;
        }
        public static List<string> processQuote(string buf, out string doneBuf)
        {
            int i = 0, j = 0,count = 0;
            List<string> record = new List<string>();
            string temp;
            doneBuf = buf;
            while (true)
            {
                i = buf.IndexOf('\"',j);
                if (i < 0) break;
                j = buf.IndexOf('\"', i + 1);
                if (j < 0) throw new Exception("quotes not matched");
                temp = buf.Substring(i, j - i + 1);
                doneBuf = doneBuf.Replace(temp, "_REP_" + count.ToString());
                record.Add(temp);
                j++;
                count++;
            }
            return record;
        }
        
        static void Main(string[] args)
        {
            string[] tokens = new string[0];
            IExpression nul;
            if (!__forcePath__ && args.Length == 0)
            {
                Console.WriteLine("J-Script {0} ({1}) | All rights reserved to {2} ({3})",
                    __version__,__releaseTime__, __myname__,__myEmail__);
                while (true)
                {
                    bool end = true;
                    try
                    {
                        tokens = mergeArray(tokens, readCommond());
                        tokens = processTokens(tokens, ref Tools.globalFrame, out nul,ref end);
                    }
                    catch (Exception e)

                    {
                        if (e.Message == "END")
                            break;
                        Console.WriteLine(e.Message);
                        tokens = new string[0];
                    }
                }
            }
            else
            {
                string path = args[0];
                //string path = "helloworld.jsc";
                //string path = __path__;
                if (!File.Exists(path))
                    Console.WriteLine("file not found");
                StreamReader sr = File.OpenText(path);
                while (true)
                {
                    bool end = true;
                    try
                    {
                        tokens = mergeArray(tokens, readCommondInFile(sr));
                        tokens = processTokens(tokens, ref Tools.globalFrame, out nul,ref end);
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "END")
                            break;
                        Console.WriteLine(e.Message);
                        tokens = new string[0];
                    }
                }
            }

            //Console.WriteLine(term.eval(dic));
            Console.ReadKey();
        }
    }
}
