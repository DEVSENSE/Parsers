<?php
// PHP 7+ code
use My\Full\{ClassA, ClassB, ClassC as C};   
use \My\Full\{MyX, MyY as MyZ};
use function My\Full\{fn_a, fn_b, fn_c as fC};
use const My\Full\{ConstA, ConstB, ConstC as cC};
use const \My\Full\{MyA, MyB as MyC};
?>
<<<TEST>>>

"GlobalCode" : {
  "Span" : {
    "start" : "0",
    "end" : "0"
  },
  "NamingContext" : {
    "Aliases" : {
        "ClassA" : "My\Full\ClassA",
        "ClassB" : "My\Full\ClassB",
        "C" : "My\Full\ClassC", 
        "MyX" : "My\Full\MyX", 
        "MyZ" : "My\Full\MyY" 
    },
    "ConstantAliases" : {
        "ConstA" : "My\Full\ConstA",
        "ConstB" : "My\Full\ConstB",
        "cC" : "My\Full\ConstC",    
        "MyA" : "My\Full\MyA", 
        "MyC" : "My\Full\MyB" 
    },
    "FunctionAliases" : {
        "fn_a" : "My\Full\fn_a",
        "fn_b" : "My\Full\fn_b",
        "fC" : "My\Full\fn_c" 
    }
  }
}
