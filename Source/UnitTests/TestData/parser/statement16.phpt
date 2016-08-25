<?php

foo($a);
$foo($a);  
($foo.'f')($a); 
"foo"($a);

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectFcnCall":{"Name":"foo","FallbackQualifiedName":"","ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "IndirectFcnCall":{"DirectVarUse":{"VarName":"foo"},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "IndirectFcnCall":{"BinaryEx":{"Operation":"Concat","DirectVarUse":{"VarName":"foo"},"StringLiteral":{"Value":"f"}},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "IndirectFcnCall":{"StringLiteral":{"Value":"foo"},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}}
}
