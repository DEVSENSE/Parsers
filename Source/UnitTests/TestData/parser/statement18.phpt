<?php

$x->func();
$x->{$a}();

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectFcnCall":{"Name":"func","OrigianlName":"func","FallbackQualifiedName":"","IsMemberOf":{"DirectVarUse":{"VarName":"x"}}},
  "IndirectFcnCall":{"DirectVarUse":{"VarName":"a"},"IsMemberOf":{"DirectVarUse":{"VarName":"x"}}}
}
