<?php
($x+1)::$y;
"x"::$y;
$ $x;
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectStFldUse":{"PropertyName":"y","IndirectTypeRef":{"IndirectVarUse":{"BinaryEx":{"Operation":"Add","DirectVarUse":{"VarName":"x"},"LongIntLiteral":{"Value":"1"}}}}},
  "DirectStFldUse":{"PropertyName":"y","IndirectTypeRef":{"IndirectVarUse":{"StringLiteral":{"Value":"x"}}}},
  "IndirectVarUse":{"DirectVarUse":{"VarName":"x"}}
}