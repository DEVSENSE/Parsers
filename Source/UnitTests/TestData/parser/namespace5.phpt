<?php
namespace MyProject;

echo "hello";

namespace MyProject\AnotherProject;

use My\ClassA as cls;
echo "world";  

namespace Incomple
{
  echo "end";
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
      "end" : "27"
    },
    "Name" : "MyProject",
    "SimpleSyntax" : "True",
    "NamingContext" : {
      "Namespace" : "MyProject"
    },
    "EchoStmt" : {
      "Span" : {
        "start" : "31",
        "end" : "44"
      },
      "StringLiteral" : {
        "Span" : {
          "start" : "36",
          "end" : "43"
        },
        "Value" : "hello"
      }
    }
  },
  "NamespaceDecl" : {
    "Span" : {
      "start" : "48",
      "end" : "83"
    },
    "Name" : "MyProject\AnotherProject",
    "SimpleSyntax" : "True",
    "NamingContext" : {
      "Namespace" : "MyProject\AnotherProject",
      "Aliases" : {
          "cls" : "My\ClassA"
      }
    },
    "EchoStmt" : {
      "Span" : {
        "start" : "110",
        "end" : "123"
      },
      "StringLiteral" : {
        "Span" : {
          "start" : "115",
          "end" : "122"
        },
        "Value" : "world"
      }
    },
    "NamespaceDecl" : {
      "Span" : {
        "start" : "129",
        "end" : "168"
      },
      "Name" : "Incomple",
      "SimpleSyntax" : "False",
      "NamingContext" : { 
        "Namespace" : "Incomple"
      },   
      "BlockStmt" : {
        "EchoStmt" : {
          "Span" : {
            "start" : "154",
            "end" : "165"
          },
          "StringLiteral" : {
            "Span" : {
              "start" : "159",
              "end" : "164"
            },
            "Value" : "end"
          }
        }
      }
    }
  }
}