<?php
$x = 4 * 5;
$x = 'hello'."world$x";  // complex string
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext":{
  },
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
    "DirectVarUse" : {
      "VarName" : "x"
    },
    "BinaryEx" : {
      "Operation" : "Mul",
      "LongIntLiteral" : {
        "Value" : "4"
      },
      "LongIntLiteral" : {
        "Value" : "5"
      }
    }
  },   
  "ValueAssignEx" : {
    "Operation" : "AssignValue", 
    "DirectVarUse" : {
      "VarName" : "x"
    },
    "BinaryEx" : {
      "Operation" : "Concat", 
      "StringLiteral" : {
        "Value" : "hello"
      },
      "ConcatEx" : {
        "StringLiteral" : {
          "Value" : "world"
        },
        "DirectVarUse" : {
          "VarName":"x"
        }
      }
    }
  }
}
