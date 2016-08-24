<?php

$bla = 666;

echo "$bla"; 

echo "hello${$bla}"; 

echo "hello{$foo[0]()}"; 

echo "hello{$foo()}"; 
 
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{"Operation":"AssignValue","DirectVarUse":{"VarName":"bla"},"LongIntLiteral":{"Value":"666"}},
  "EchoStmt":{"ConcatEx":{"DirectVarUse":{"VarName":"bla"}}},
  "EchoStmt":{"ConcatEx":{"StringLiteral":{"Value":"hello"},"IndirectVarUse":{"DirectVarUse":{"VarName":"bla"}}}},
  "EchoStmt":{"ConcatEx":{"StringLiteral":{"Value":"hello"},"IndirectFcnCall":{"ItemUse":{"Array":{"DirectVarUse":{"VarName":"foo"}},"Index":{"LongIntLiteral":{"Value":"0"}}}}}},
  "EchoStmt":{"ConcatEx":{"StringLiteral":{"Value":"hello"},"IndirectFcnCall":{"DirectVarUse":{"VarName":"foo"}}}}
}
