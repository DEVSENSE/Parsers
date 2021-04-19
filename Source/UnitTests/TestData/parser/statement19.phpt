<?php

func;
X::func;
$x::func;

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "GlobalConstUse":{"Name":"func","OriginalName":"func","FallbackName":""},
  "ClassConstUse":{"Name":"func","ClassTypeRef":{"ClassName":"X"}},
  "ClassConstUse":{"Name":"func","IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}}
}
