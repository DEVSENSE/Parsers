<?php
echo "hello";
$values = [<<<END
a
b
ENDING, 'd e f'];
/*
Parse error: syntax error, unexpected 'ING' (T_STRING), expecting ']' in %s on line %d
*/
<<<TEST>>>

ERRORS: 2014

"GlobalCode":{
  "NamingContext":{},
  "EchoStmt":{"StringLiteral":{"Value":"hello"}},
  "ValueAssignEx":{
    "Operation":"AssignValue",
    "DirectVarUse":{"VarName":"values"},
    "ArrayEx":{
      "Item":{"ValueExpr":{"StringLiteral":{"Value":"abEND"}}},
      "Item":{"ValueExpr":{"StringLiteral":{"Value":"def"}}}
    }
  }
}