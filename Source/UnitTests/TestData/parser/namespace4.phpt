<?php
namespace
{
  echo "hello";
}

namespace MyProject
{
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
      "end" : "39"
    },
    "SimpleSyntax" : "False",
    "NamingContext" : {
    },
    "EchoStmt" : {
      "Span" : {
        "start" : "23",
        "end" : "36"
      },
      "StringLiteral" : {
        "Span" : {
          "start" : "28",
          "end" : "35"
        },
        "Value" : "hello"
      }
    }
  },
  "NamespaceDecl" : {
    "Span" : {
      "start" : "43",
      "end" : "85"
    },
    "Name" : "MyProject",
    "SimpleSyntax" : "False",
    "NamingContext" : {
      "Namespace" : "MyProject"
    },
    "EchoStmt" : {
      "Span" : {
        "start" : "69",
        "end" : "82"
      },
      "StringLiteral" : {
        "Span" : {
          "start" : "74",
          "end" : "81"
        },
        "Value" : "world"
      }
    }
  }
}