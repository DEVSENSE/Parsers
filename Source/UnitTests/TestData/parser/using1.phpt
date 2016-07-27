<?php
namespace foo;
use My\Full\Classname as Another;

?>

<<<TEST>>>

"GlobalCode" : {
  "Span" : {
    "start" : "0",
    "end" : "0"
  },  
  "NamingContext" : {},
  "NamespaceDecl" : {
    "Span" : {
      "start" : "7",
      "end" : "21"
    },
    "Name" : "foo",
    "SimpleSyntax" : "True",
    "NamingContext" : {
      "Namespace" : "foo",
      "Aliases" : {
        "Another" : "My\Full\Classname"
      }
    },
    "EchoStmt" : {
      "Span" : {
        "start" : "64",
        "end" : "66"
      },
      "StringLiteral" : {
        "Span" : {
          "start" : "64",
          "end" : "66"
        },
        "Value" : ""
      }
    }
  }
}
