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
    "OrigianlName":"define", 
    "FallbackQualifiedName":"",
    "ActualParam" : {  
      "IsUnpack":"False",
      "StringLiteral" : {
        "Value" : "constA"
      }
    },        
    "ActualParam" : {  
      "IsUnpack":"False",
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
    "Body":{
      "BlockStmt":{
        "DirectFcnCall" : {
          "Name" : "MyProject\define",
          "OrigianlName":"define", 
          "FallbackQualifiedName":"define",
          "ActualParam" : {    
            "IsUnpack":"False",
            "StringLiteral" : {
              "Value" : "constB"
            }
          },       
          "ActualParam" : {    
            "IsUnpack":"False",
            "LongIntLiteral" : {
              "Value" : "0" 
            }
          } 
        }
      }
    }
  }
}