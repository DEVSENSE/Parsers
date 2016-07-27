<?php
$x = 4 * 5;
$x = 'hello'."world$x";  // TODO string handling must be fixed (tomorrow)
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext":{
  },
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
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
    "BinaryEx" : {
      "Operation" : "Concat",
      "StringLiteral" : {
        "Value" : "hello"
      },
      "NullLiteral" : {
        "Value" : "null"
      }
    }
  }
}
