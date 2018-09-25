<?php
$values = [<<<END
  a
 b
  END, 'd e f'];
/*
Parse error: syntax error, different indentation from line 3
*/
<<<TEST>>>

ERRORS: 2014

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{
    "Operation":"AssignValue",
    "DirectVarUse":{"VarName":"values"},
    "ArrayEx":{
      "Item":{"ValueExpr":{"StringLiteral":{"Value":"ab"}}},
      "Item":{"ValueExpr":{"StringLiteral":{"Value":"def"}}}
    }
  }
}