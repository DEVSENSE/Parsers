<?php
clone $x;
$x = $y ?? 10;
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext":{
  },
  "UnaryEx" : {
    "Operation" : "Clone",
    "DirectVarUse" : {
      "VarName" : "x"
    }
  },   
  "ValueAssignEx" : {
    "Operation" : "AssignValue", 
    "DirectVarUse" : {
      "VarName" : "x"
    },
    "BinaryEx" : {
      "Operation" : "Coalesce", 
      "DirectVarUse" : {
        "VarName" : "y"
      },
      "LongIntLiteral" : {
        "Value" : "10"
      }
    }
  }
}
