<?php

if($a > 0)
  include "a.php"; // this will include a.php

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "IfStmt":{
    "CondList":{
      "ConditionalStmt":{
        "Condition":{
          "BinaryEx":{
            "Operation":"GreaterThan",
            "DirectVarUse":{"VarName":"a"},
            "LongIntLiteral":{"Value":"0"}
          }
        },
        "Statement":{"IncludingEx":{
          "InclusionType":"Include",  
          "IsConditional":"True",
          "StringLiteral":{"Value":"a.php"}}
        }
      }
    }
  }
}
