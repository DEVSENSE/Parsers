<?php
($x)::$y;
"x"::$y;
$ $x;
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectStFldUse":{"PropertyName":"y","IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}},
  "DirectStFldUse":{"PropertyName":"y","IndirectTypeRef":{"StringLiteral":{"Value":"x"}}},
  "IndirectVarUse":{"DirectVarUse":{"VarName":"x"}}
}