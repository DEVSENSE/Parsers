<?php
namespace MyProject
{
  echo "hello";
}

namespace MyProject\AnotherProject
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
      "end" : "49"
    },
    "Name" : "MyProject",
    "SimpleSyntax" : "False",
    "NamingContext" : {
      "Namespace" : "MyProject"
    },
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
  },
  "NamespaceDecl" : {
    "Span" : {
      "start" : "53",
      "end" : "110"
    },
    "Name" : "MyProject\AnotherProject",
    "SimpleSyntax" : "False",
    "NamingContext" : {
      "Namespace" : "MyProject\AnotherProject"
    },
    "EchoStmt" : {
      "Span" : {
        "start" : "94",
        "end" : "107"
      },
      "StringLiteral" : {
        "Span" : {
          "start" : "99",
          "end" : "106"
        },
        "Value" : "world"
      }
    }
  }
}