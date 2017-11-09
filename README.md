# jscript
a simple interpretive language

This language is first designed to find the mathematical derivatives of a math funcition depending 
on several variables. It can find partial derivatives. Even if f(u,v), u(x,y), v(x,y), it can find 
df/dx, df/dy using chain rule.

This language can also be used for other purposes. It has control structures such as if/while/for. 
It also has strings and arrays (may be multi-dimentional). And it is able to define functions and call them.

This language is functional, but it is not good at error detecting. If the code is not well written,
the interpretor may not give an informative message and may crash. This is due to the insufficient
lexical analysis and parse tree building. However, there is no obvious bug in the interpretor, 
so it is still usable.

see ./expression/bin/Debug for referance (in Chinese) and examples.
