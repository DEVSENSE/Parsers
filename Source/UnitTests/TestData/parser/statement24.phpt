<?php
       
list($a, $b => $c) = 1; 
[$a, $b => $c] = 1;


?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{"Operation":"AssignValue",
    "ArrayEx":{
      "Item":{"ValueExpr":{"DirectVarUse":{"VarName":"a"}}},
      "Item":{"Index":{"DirectVarUse":{"VarName":"b"}},"ValueExpr":{"DirectVarUse":{"VarName":"c"}}}
    },
    "LongIntLiteral":{"Value":"1"}
  },
  "ValueAssignEx":{"Operation":"AssignValue",
    "ArrayEx":{
      "Item":{"ValueExpr":{"DirectVarUse":{"VarName":"a"}}},
      "Item":{"Index":{"DirectVarUse":{"VarName":"b"}},"ValueExpr":{"DirectVarUse":{"VarName":"c"}}}
    },
    "LongIntLiteral":{"Value":"1"}
  }
}
