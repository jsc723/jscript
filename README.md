# J-Script
## Introduction

This is a simple interpreted language that I wrote a few years ago for educational purpose. 

This language is first designed to find the mathematical derivatives (including partial derivatives) of mathematical functions. For example, if there is two variable x and y, and we define functions u:=sin(x)\*y, v:=x\*cos(y), f:=u+v, the language can easily find the mathematical form of df/dx (=y\*cos(x)+cos(y)), df/dy (=sin(x)-x\*sin(y)) using chain rule.

This language can also be used for general purposes. It has control structures such as if/while/for. 
It also has strings and arrays (may be multi-dimensional) and functions!

Some weaknesses of this language: It is not good at detecting syntax error. If the code is not well written, the interpreter may not give any informative message and may simply crash. This is due to the insufficient lexical analysis and parse tree building. The code is also not optimized for performance. 

However, there is no obvious bug in the interpreter, so it is still usable.

There is an example in `/examples` folder.

## Documentation

#### Chapter 0: Run the interperater

1. Interactive mode: simply double click the exe file
2. Script mode: enter command `J-Script.exe <script_name>.jsc`

#### Chapter 1: Variables

Use keyword `var` to define a variable
1. define a variable without initialization：

  ```
  var a;
  ```

2. define multiple variables without initialization：

  ```
  var a,b,c;
  ```

3. define variable with initialization：

  ```
  var a = 1;
  var b = 1+3*2, c, d = 4-9;
  var e = (a+b)*2; #can use other variables to initialize
  ```

  Each statement ends with a semicolon ( `;`).
  You can put multiple statements in a single line.
  contents after `#` in a line is treated as comments.

#### Chapter 2: Strings 

Use keyword `str` to define strings

```
str s;
str s1,s2,s3;
str s1 = "aa", s2;

Node that there is no `+` operations implemented for strings：
str s1 = "aa"+"bb"; #error
```

#### Chapter 3: Operators

```

arithmetic operators：
+ - * / % ^
(note that no integer division for /)。
a^b means raise a to bth power 
^ has higher precedence to */%。

comparation operators：
==  !=  <  >  <= >=

logic operators
|| &&
not()

assignment oprator：
=
examples:
a = 1;
s1 = "string";
b = func(1,2,3); #functions are introduced in later chapters
```

#### Chapter 4: Arrays

```
Arrays can store any type, including other arrays.
Array is 0-indexed.

Use keyword `arr` to define arrays

1.define empty arrays
arr array;
arr array1,array2,array3;
# arrays defined in this way have length 0.

2.define arrays with initial size
arr array[initialSize];
For example：
arr array[10]; #array has 10 null elements
arr array1[5+8], array2[a+3]; #can use expression as array's initial size。

3.define multi-dimensional array
For example：
arr doubleArray[2,3]; 
# create an 2*3 array，whose elements are all null
# = { { null, null, null }, { null, null, null } }

It is also possible to define arrays with more dimensions, split by colon:
arr triArray[4,5,6], quaArray[2,3,4,5];

4.Assign a value to an element in array
array[index] = value;

For example：
array[2] = 3; #assign 3 to the item 2 (0-indexed) in the array
doubleArray[0,1] = 4;


5.Use append to add elements to array at back
#Suppose array = {0,1}
append array 2；
#Now array = {0,1,2};
If you omit the argument after `array`, a `null` will be appended.
append array;
#Now array = {0,1,2,null};
```

#### Chapter 5: Input/Output

```
Use inputNum function to get a numeric input from stdin：
inputNum(a);
Note: There is no support for `inputNum(array[index])` for now

Use print function to output values to stdout：

When use print to display the value of a variable, you need to use with eval. Otherwise, it will simply print the name of the variable.

var a = 2;
print(a);#display a
print(eval(a));#display 2
print(eval(a+2));#display 4

#str s = “abc“;
print(s); #display abc

print can take multiple arguments, seperated by colons. 
print("a = ",eval(a),"\n");
print("array = ",array);
```

#### Chapter 6: Control Statements

Conditions

The expression below are all examples of valid conditions.

```
true
false
winner == X
i<L && winner==0 || false
not(1 == 2)
```

`true` and `false` are keywords.

Use `not(condition)` to inverse a condition

##### If block

```
if(condition) 
{
    statements
};

if(condition){
    statements
}else{
   	statements
};

if(condition_1){
    statements
}elif(condition_2){ #there can be any numbers of elif blocks
    statements
}else{
    statements
};
#Note the semicolon at the end of the blocks

```

##### while block

```
while(condition)
{
    statements
};
```

##### for block

```
for(initialization;(condition);end_of_block)
{
        statements
};

For example:
for(var i = 0;(i<3);i=i+1)
{
        for((var j = 0),(j<3),(j=j+1))
        {
            board[i,j] = 0;
        };
        break;
};
```

Note that you must use an extra pair of parathesis on the condition.

And you can use the statement `break;` to jump out from the inner-most while/for block.

#### Chapter 7: Functions

Use keyword `proc` to define functions. You need to specify the type of input arguments in the function's definition, but you don't need to specify the return type.

`proc` can take `var, str, arr, proc, func (introduced later)` types as arguments.

use `return <expression>` to return a number from function.

use return obj <expression> to return an `str，arr，func，proc` from function

```
Example 1:
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

Example 2:
proc initlines()
{
    arr lines[8,3,2];
    return obj lines; #return an reference to an array 
    				  #note that it is not destroyed when function ends
};

You can define function in function, and return function from function
A function defined in an other function can access arguments of its parent function。
Example 3:
proc make_adder(var a)
{
    proc adder(var b) 
    {
        return a+b;
    };
    return obj adder; 
};
proc add3 = make_adder(3); #Use keyword proc to initialize add3 to the return value 
var five = add3(2);
print(eval(five)); #display 5


```

##### Built-in functions

 ```
 print(...), inputNum(arg),
 sin(arg),cos(arg),tan(arg),exp(arg),ln(arg),log(num,base)
 deriv(func,var) #introduced in later chapter
 ```

#### Chapter 8: Mathematical functions

The initial purpose of this language is to compute the mathematical derivitives of multivariable functions. 

Use keyword `func ` to define a mathematical function. You can only define one math function in a single statement. 

```
var x,y;
func f = x^2;
func g = sin(x)+y/2;
```

You can use function `eval` to compute the value of a function:

```
var x;
func f = x^2;
print(eval(f)); #error: x is not initialized
x = 3;
print(eval(f)); #display 9
```

Use function `deriv` to find the mathematical derivitive of a function, which returns a new `func` object. Usage: `deriv(f, x)` #computes df/dx

```
func df = deriv(f,x); #df = 2*x
# the first entry can be any function expression
func dg = deriv(sin(x)+y/2 + f, y);  #dg = 0.5
```

In fact, var is a special case of `func`, therefore the first entry of deriv can also be a `var`

```
deriv(x,x) #=1
deriv(x,y) #=0
```

This is why you must add `eval` when you want to display the value of a `var`, because `var` is treated the same as `func` by default. 

`deriv` uses chain rule when there is multiple layers of dependence:

```
var x,y,t; 
func x = 2*t;
func y = t^2; 
func z = x*y; 

Now,
deriv(z,t) is 2y+x*(2t)
deriv(z,x) is y
deriv(z,y) is x
```

#### Chapter 9: Miscellaneous

```
1. enviro();
print the current enviroment，(all defined var,str,arr,func,proc objects)

2. pause(<prompt>);
pause with a prompt, wait for a key to continue, for example:
pause("press any key to continue");

3. delete(<object>);
delete an object from enviroment, if there are multiple objects with the same name, 
then all of them are deleted. Example：
var x = 3;
arr x[2];
delete(x); #delete both var x and arr x
```

