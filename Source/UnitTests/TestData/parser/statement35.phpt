<?php

$bla = 666;

echo "$bla";  
 
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{"Operation":"AssignValue","DirectVarUse":{"VarName":"bla"},"LongIntLiteral":{"Value":"666"}},
  "EchoStmt":{"ConcatEx":{"DirectVarUse":{"VarName":"bla"}}}
}
