<?php

$bla = 666;

echo "$bla"; 

echo "hello${$bla}"; 
 
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{"Operation":"AssignValue","DirectVarUse":{"VarName":"bla"},"LongIntLiteral":{"Value":"666"}},
  "EchoStmt":{"ConcatEx":{"DirectVarUse":{"VarName":"bla"}}},
  "EchoStmt":{"ConcatEx":{"StringLiteral":{"Value":"hello"},"IndirectVarUse":{"DirectVarUse":{"VarName":"bla"}}}}
}
