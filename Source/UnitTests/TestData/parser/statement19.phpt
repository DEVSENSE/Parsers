<?php

func;
X::func;
$x::func;

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "GlobalConstUse":{"Name":"func"},
  "ClassConstUse":{"Name":"func","DirectTypeRef":{"ClassName":"X"}},
  "ClassConstUse":{"Name":"func","IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}}
}
