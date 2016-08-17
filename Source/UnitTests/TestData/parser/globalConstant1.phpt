<?php

const constA = 'Hello', ConstB = 0;

namespace MyProject;  

/** global const */
const constC = 'World', ConstD = 11;

?>
<<<TEST>>>

"GlobalCode" : {
  "Span" : {
    "start" : "0",
    "end" : "0"
  },
  "NamingContext" : {},
  "GlobalConstDeclList" : {
    "Span" : {
      "start" : "9",
      "end" : "44"
    },
    "GlobalConstantDecl" : {
      "Span" : {
        "start" : "15",
        "end" : "31"
      },
      "NameIsConditional" : "False",
      "Name" : "constA",
      "StringLiteral" : {
        "Span" : {
          "start" : "24",
          "end" : "31"
        },
        "Value" : "Hello"
      }
    },
    "GlobalConstantDecl" : {
      "Span" : {
        "start" : "33",
        "end" : "43"
      },
      "NameIsConditional" : "False",
      "Name" : "ConstB",
      "LongIntLiteral" : {
        "Span" : {
          "start" : "42",
          "end" : "43"
        },
        "Value" : "0"
      }
    }
  },
  "NamespaceDecl" : {
    "Span" : {
      "start" : "48",
      "end" : "68"
    },
    "Name" : "MyProject",
    "SimpleSyntax" : "True",
    "NamingContext" : {
      "Namespace" : "MyProject"
    },
    "GlobalConstDeclList" : {
      "Span" : {
        "start" : "74",
        "end" : "110"
      },     
      "PHPDoc":{"Comment":"global const"},
      "GlobalConstantDecl" : {
        "Span" : {
          "start" : "80",
          "end" : "96"
        },
        "NameIsConditional" : "False",
        "Name" : "constC",
        "PHPDoc":{"Comment":"global const"},
        "StringLiteral" : {
          "Span" : {
            "start" : "89",
            "end" : "96"
          },
          "Value" : "World"
        }
      },
      "GlobalConstantDecl" : {
        "Span" : {
          "start" : "98",
          "end" : "109"
        },
        "NameIsConditional" : "False",
        "Name" : "ConstD",
        "LongIntLiteral" : {
          "Span" : {
            "start" : "107",
            "end" : "109"
          },
          "Value" : "11"
        }
      }
    }
  }
}