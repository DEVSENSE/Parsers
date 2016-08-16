<?php

empty(x+5);          
isset($x, $y);
eval("hello".'d');
exit($x);

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "EmptyEx":{"BinaryEx":{"Operation":"Add","GlobalConstUse":{"Name":"x"},"LongIntLiteral":{"Value":"5"}}},
  "IssetEx":{"IndirectVarUse":{"DirectVarUse":{"VarName":"x"}},"IndirectVarUse":{"DirectVarUse":{"VarName":"y"}}},
  "EvalEx":{"BinaryEx":{"Operation":"Concat","StringLiteral":{"Value":"hello"},"StringLiteral":{"Value":"d"}}},
  "ExitEx":{"DirectVarUse":{"VarName":"x"}}
}