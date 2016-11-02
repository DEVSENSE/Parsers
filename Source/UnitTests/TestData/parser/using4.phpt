<?php
// PHP 7+ code
use My\Full\{ClassA, function fn_a, const ConstA, 
ClassB as B, function fn_b as fB, const ConstB as cB};  
  
use \My\Full\{MyX, function MyY, const MyZ};
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
        "B" : "My\Full\ClassB",
        "MyX" : "My\Full\MyX"
    },
    "ConstantAliases" : {
        "ConstA" : "My\Full\ConstA",
        "cB" : "My\Full\ConstB",
        "MyZ" : "My\Full\MyZ"
    },
    "FunctionAliases" : {
        "fn_a" : "My\Full\fn_a",
        "fB" : "My\Full\fn_b",
        "MyY" : "My\Full\MyY" 
    }
  },
  "UseStatement":{"Kind":"Type","Aliases":{"ClassA":{}, "fn_a":{}, "ConstA":{}, "B":{}, "fB":{}, "cB":{}}},
  "UseStatement":{"Kind":"Type","Aliases":{"MyX":{}, "MyY":{}, "MyZ":{}}}
}
