<?php

$x->func();
$x->{$a}();

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectFcnCall":{"Name":"func","DirectVarUse":{"VarName":"x"}},
  "IndirectFcnCall":{"DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"x"}}
}
