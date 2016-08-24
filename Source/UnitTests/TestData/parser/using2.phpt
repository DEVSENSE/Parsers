<?php
// this is the same as use My\Full\NSname as NSname
use My\Full\GlobalSname;

namespace foo;
use My\Full\Classname as Another;

// this is the same as use My\Full\NSname as NSname
use My\Full\NSname;

// importing a global class
use ArrayObject;

// importing a function (PHP 5.6+)
use function My\Full\functionName;

// aliasing a function (PHP 5.6+)
use function My\Full\functionName as func;

// importing a constant (PHP 5.6+)
use const My\Full\CONSTANT;

use MyX, \My\MyY;

?>
<<<TEST>>>

"GlobalCode" : {
  "Span" : {
    "start" : "0",
    "end" : "0"
  },
  "NamingContext" : {
    "Aliases" : {
        "GlobalSname" : "My\Full\GlobalSname"
    }
  },
  "NamespaceDecl" : {
    "Span" : {
      "start" : "88",
      "end" : "102"
    },
    "Name" : "foo",
    "SimpleSyntax" : "True",
    "NamingContext" : {
      "Namespace" : "foo",
      "Aliases" : {
        "Another" : "My\Full\Classname",
        "NSname" : "My\Full\NSname",
        "ArrayObject" : "ArrayObject",
        "MyX" : "MyX",
        "MyY" : "My\MyY"
      },
      "ConstantAliases" : {
        "CONSTANT" : "My\Full\CONSTANT"
      },
      "FunctionAliases" : {
        "functionName" : "My\Full\functionName",
        "func" : "My\Full\functionName"
      }
    },
    "Body":{
      "BlockStmt":{}
    }
  }
}
