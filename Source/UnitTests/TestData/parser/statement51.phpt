<?php

function ab(){}
("a"."b")();
("foo$a")();
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "FunctionDecl":{"Name":"ab","IsConditional":"False","FormalParams":{},"Body":{"BlockStmt":{}}},
  "IndirectFcnCall":{"BinaryEx":{"Operation":"Concat","StringLiteral":{"Value":"a"},"StringLiteral":{"Value":"b"}}},
  "IndirectFcnCall":{"ConcatEx":{"StringLiteral":{"Value":"foo"},"DirectVarUse":{"VarName":"a"}}}
}