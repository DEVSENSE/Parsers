<?php

empty(x+5);          
isset($x, $y);
eval("hello".'d');
exit($x);

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "EmptyEx":{"BinaryEx":{"Operation":"Add","GlobalConstUse":{"Name":"x","OrigianlName":"x","FallbackName":""},"LongIntLiteral":{"Value":"5"}}},
  "IssetEx":{"DirectVarUse":{"VarName":"x"},"DirectVarUse":{"VarName":"y"}},
  "EvalEx":{"BinaryEx":{"Operation":"Concat","StringLiteral":{"Value":"hello"},"StringLiteral":{"Value":"d"}}},
  "ExitEx":{"DirectVarUse":{"VarName":"x"}}
}