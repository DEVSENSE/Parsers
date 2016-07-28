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
    "ActualParam" : {
      "StringLiteral" : {
        "Value" : "constA"
      }
    },        
    "ActualParam" : {
      "StringLiteral" : {
        "Value" : "Hello"
      }
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
      "ActualParam" : {
        "StringLiteral" : {
          "Value" : "constB"
        }
      },       
      "ActualParam" : {
        "LongIntLiteral" : {
          "Value" : "0" 
        }
      } 
    }
  }
}