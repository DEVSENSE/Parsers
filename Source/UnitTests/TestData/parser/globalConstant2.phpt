<?php

define("constA", 'Hello');

namespace MyProject;  

define("constB", 0);

?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "DirectFcnCall" : {
    "Name" : "define",
    "StringLiteral" : {
      "Value" : "constA"
    },
    "StringLiteral" : {
      "Value" : "Hello"
    }
  },
  "NamespaceDecl" : {
    "Name" : "MyProject",
    "SimpleSyntax" : "True",
    "NamingContext" : {
      "Namespace" : "MyProject"
    },
    "DirectFcnCall" : {
      "Name" : "define",
      "StringLiteral" : {
        "Value" : "constB"
      },
      "LongIntLiteral" : {
        "Value" : "0"
      }
    }
  }
}