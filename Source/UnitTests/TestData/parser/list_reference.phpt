<?php
list($a, &$b) = $array;
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{
    "Operation":"AssignValue",
    "ArrayEx":{"Item":{"ValueExpr":{"DirectVarUse":{"VarName":"a"}}},"Item":{"RefToGet":{"DirectVarUse":{"VarName":"b"}}}},
    "DirectVarUse":{"VarName":"array"}
  }
}