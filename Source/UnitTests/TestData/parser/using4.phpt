<?php
// PHP 7+ code
use My\Full\{ClassA, function fn_a, const ConstA, 
ClassB as B, function fn_b as fB, const ConstB as cB};
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
        "B" : "My\Full\ClassB"
    },
    "ConstantAliases" : {
        "ConstA" : "My\Full\ConstA",
        "cB" : "My\Full\ConstB"
    },
    "FunctionAliases" : {
        "fn_a" : "My\Full\fn_a",
        "fB" : "My\Full\fn_b" 
    }
  }
}
