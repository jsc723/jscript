# jscript
a simple interpreted language

This language is first designed to find the mathematical derivatives of a math funcition depending 
on several variables. It can find partial derivatives. Even if f(u,v), u(x,y), v(x,y), it can find 
df/dx, df/dy using chain rule.

This language can also be used for other purposes. It has control structures such as if/while/for. 
It also has strings and arrays (may be multi-dimentional). And it is able to define functions and call them.

This language is functional, but it is not good at error detecting. If the code is not well written,
the interpretor may not give an informative message and may crash. This is due to the insufficient
lexical analysis and parse tree building. However, there is no obvious bug in the interpretor, 
so it is still usable.

There is an example in /examples folder.

The documentation for v1.1 is here (in Chinese), the current version has more than that, 
but I didn't have a change to write a documentation for it. 

    	      JScript 用户手册
--------------------前言--------------------

JScript是我以学习为目的制作的一款类似于python的简单的、开源的脚本语言。功能十分有限。仅供学习交流使用。
本文档介绍了这个语言的语法。看完了本文档后，可以参考一下示例文件tictactoe.jsc。
jscjsc723723@163.com
2017/3/24
--------------------第零节 运行方法--------------------
本语言支持两种运行模式
1. 交互模式 - 即双击程序直接运行
2. 脚本模式 - 用控制台运行程序，后面跟一个想要运行的脚本的文件名（或者直接把脚本拖拽到程序的图标上）

-------------------第一节 变量--------------------

使用var关键字定义变量
1. 定义一个变量，不初始化：
var a;
2. 定义多个变量，不初始化：
var a,b,c;
3. 定义变量，并且初始化，后面可以是任意复杂的表达式：
var a = 1;
var b = 1+3*2, c, d = 4-9;
var e = (a+b)*2; #可以用别的变量的值给新变量赋值
注意每条语句后面都要加“；”。
多句语句可以写在一行。
#表示这一行后面的部分都是注释。

--------------------第二节 字符串--------------------

使用str关键字定义字符串
类似定义变量，以下都是合法的：
str s;
str s1,s2,s3;
str s1 = "aa", s2;

但是字符串不支持+运算符：
str s1 = "aa"+"bb"; #错误

--------------------第三节 运算符--------------------

算数运算符有：
+ - * / % ^
这里注意除法不会向下取整。
另外，^表示乘方，例2^3等于8
乘方运算的优先度要大于*/%。

比较运算符：
==  !=  <  >  <= >=
|| &&
不过没有取反运算符”！“

赋值运算符：
=
例：
a = 1;
s1 = "string";
b = func(1,2,3); #函数在之后的章节里介绍


--------------------第四节 数组--------------------

数组类似python的list，里面可以存放任意类型的数据，包括其他数组，
并且可以通过append关键字动态加长。

使用arr关键字定义数组

1.定义空数组
arr array;
arr array1,array2,array3;
产生的数组长度都为0

2.定义有初始长度的数组
arr array[initialSize];
例：
arr array[10]; #array的初始长度为10，其元素全为null
arr array1[5+8], array2[a+3]; #方括号里可以是任意复杂的表达式（a是一个初始化过的变量）。

3.定义多维数组
例：
arr doubleArray[2,3]; 
#创建一个两行三列的二维数组，元素全为null
#即：{ { null, null, null }, { null, null, null } }
也可以定义更高维的数组，维度用逗号隔开。
arr triArray[4,5,6], quaArray[2,3,4,5];

4.给数组的元素赋值
array[index] = value;
注意元素下标从0开始。
例：
array[2] = 3; #把array的第二个元素改成3
doubleArray[0,1] = 4;
#即：doubleArray 等于 { { null, 4, null }, { null, null, null } }

5.增加数组长度
使用append关键字在数组末尾添加元素
#假设array 等于 {0,1}
append array 2；
#此时array 等于 {0,1,2};
也可以不制定要添加的元素的值，这时会默认添加一个null值
append array;
#此时array 等于 {0,1,2,null};

--------------------第五节 输入输出--------------------

inputNum函数用来接受用户输入的一个数字：
inputNum(a);
则用户的输入会被保持到a中
inputNum(array[index])这种用法现在还不支持

print函数可以显示各种数据的值，接受多个参数和各种数据类型：

打印变量的值的时候必须再加一层eval()函数，否则打印出来的是变量的名字。

#a 是 var，值等于2
print(a);#显示a
print(eval(a));#显示2
print(eval(a+2));#显示4

#s 是 str，等于“abc“
print(s); #显示abc

print("a = ",eval(a),"\n");
print("array = ",array); #array是数组

--------------------第六节 控制语句--------------------
先列一下条件的例子：
true
winner == X
i<L && winner==0 || false
以上都是合法的条件
注意true和false是关键字

1. if语句
if(条件) 
{
    执行的语句
};

if(条件1){
    执行的语句
}else{
    执行的语句
};

if(条件1){
    执行的语句
}elif(条件2){ #elif块可以有任意个
    执行的语句
}else{
    执行的语句
};
#注意if语句结尾的分号

2.while语句
while(条件)
{
    执行的语句
};

3.for语句
for((初始化),(条件),(每次循环结束后的操作))
{
        执行的语句
};
例：
for((var i = 0),(i<3),(i=i+1))
{
        for((var j = 0),(j<3),(j=j+1))
        {
            board[i,j] = 0;
        };
};
注意for的括号里面的语句即使已经用逗号隔开，还是要再用一对括号隔开
最后，用break关键字可以跳出一层while或for循环。

--------------------第七节 函数--------------------
使用proc关键字定义函数，要求参数列表包含类型，但是不需要写返回类型
proc可以接受的参数类型包括：var, str, arr, proc, func（之后介绍）。
使用return关键字返回一个var类型的值（即不能是一个str，arr，func，proc）
例1：
proc getNum(str prompt,var low,var high)
{
    var result = 0;
    print(prompt);
	while(true)
	{
		inputNum(result);
		if(result>=low && result<=high){break;};
	    print("请输入",eval(low),"到",eval(high),":"); 
	};
    return result; #返回一个数值
};
使用return obj返回str，arr，func，proc
例2：
proc initlines()
{
    arr lines[8,3,2];
    return obj lines; #返回一个数组的引用
};
函数里面可以定义函数，可以当做值返回。
定义在其他函数里面的函数可以访问其父函数里的变量（即和python是一样的）。
例3：
proc make_adder(var a)
{
    proc adder(var b) 
    {
        return a+b;
    };
    return obj adder; #返回一个函数
};
proc add3 = make_adder(3); #注意这里使用proc关键字把add3定义为make_adder(3)的返回值
var five = add3(2);
print(eval(five)); #这里会显示5

另外，JScript内置了一些函数，它们是：
print(...), inputNum(arg),
sin(arg),cos(arg),tan(arg),exp(arg),ln(arg),log(num,base)
deriv(func,var) （下一节介绍）

--------------------第八节 数学函数--------------------

由于写这个语言的一开始的目的是为了写一个能自动求导的程序，所以有这么一个数据类型。

使用func定义数学函数，一条语句只能定义一个。
例：
var x,y;
func f = x^2;
func g = sin(x)+y/2;

当然，func是可以用eval求值的，例：
var x;
func f = x^2;
print(eval(f)); #出错，因为x还没有赋值
x = 3;
print(eval(f)); #显示9

使用deriv对一个func求导，返回一个新的func
用法deriv(要求导的函数，要对哪个自变量求导)
例：
func df = deriv(f,x); #df = 2*x
#前面可以是一个由变量和其他func组成的任意复杂度的表达式
func dg = deriv(sin(x)+y/2 + f, y);  #dg = 0.5

其实，var的本质也是func，所以deriv第一个参数可以是一个var：
deriv(x,x) #=1
deriv(x,y) #=0
这同时也是为什么print var的时候要加eval才会显示值，
因为print默认把var当成func，以显示func的形式显示var，
只有明确告诉print要显示var的值才会显示数字。

注意求导是符合链式法则的，请看如下例子：
var x,y,t; #建立三个变量
func x = 2*t;
func y = t^2; #建立x，y与t的依赖关系
func z = x*y; #则z关于是x，y的函数，也是关于t的函数
此时，
deriv(z,t) 等于 2y+x*(2t)
deriv(z,x) 等于 y
deriv(z,y) 等于 x
这个关系对于x = g(u,v), y = h(u,v)等更复杂的情况也成立，有兴趣的读者可以自行尝试。

--------------------第九节 其他功能--------------------
1. enviro;
enviro;语句显示当前的环境，即所有已经定义的var,str,arr,func,proc

2. pause [提示];
即暂停一下，按任意键继续的功能，后面可以跟一个字符串，也可以不跟，例：
pause "请按任意键继续";

3. delete [想删除的东西的名字];
删除一个东西的定义，如果同时存在叫做一个名字的多种对象，那么所有的对象都被删除，例：
var x = 3;
arr x[2];
delete x; #var x和arr x都被删除了


