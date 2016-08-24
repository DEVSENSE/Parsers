<?php
namespace MyProject
{
  echo "hello";
}

namespace MyProject\AnotherProject
{
  use My\ClassA as cls;
  echo "world";
}
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
      "end" : "49"
    },
    "Name" : "MyProject",
    "SimpleSyntax" : "False",
    "NamingContext" : {
      "Namespace" : "MyProject"
    },  
    "Body":{ 
      "BlockStmt" : {
        "EchoStmt" : {
          "Span" : {
            "start" : "33",
            "end" : "46"
          },
          "StringLiteral" : {
            "Span" : {
              "start" : "38",
              "end" : "45"
            },
            "Value" : "hello"
          }
        }
      }
    }
  },
  "NamespaceDecl" : {
    "Span" : {
      "start" : "53",
      "end" : "135"
    },
    "Name" : "MyProject\AnotherProject",
    "SimpleSyntax" : "False",
    "NamingContext" : {
      "Namespace" : "MyProject\AnotherProject",
      "Aliases" : {
          "cls" : "My\ClassA"
      }
    },    
    "Body":{ 
      "BlockStmt" : {
        "EchoStmt" : {
          "Span" : {
            "start" : "119",
            "end" : "132"
          },
          "StringLiteral" : {
            "Span" : {
              "start" : "124",
              "end" : "131"
            },
            "Value" : "world"
          }
        }
      }
    }
  }
}