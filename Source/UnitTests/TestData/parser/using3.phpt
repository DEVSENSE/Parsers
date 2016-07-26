<?php
// PHP 7+ code
use My\Full\{ClassA, ClassB, ClassC as C};
use function My\Full\{fn_a, fn_b, fn_c as fC};
use const My\Full\{ConstA, ConstB, ConstC as cC};
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
        "C" : "My\Full\ClassC" 
    },
    "ConstantAliases" : {
        "ConstA" : "My\Full\ConstA",
        "ConstB" : "My\Full\ConstB",
        "cC" : "My\Full\ConstC" 
    },
    "FunctionAliases" : {
        "fn_a" : "My\Full\fn_a",
        "fn_b" : "My\Full\fn_b",
        "fC" : "My\Full\fn_c" 
    }
  }
}
