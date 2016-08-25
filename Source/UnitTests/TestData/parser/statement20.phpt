<?php

X::$stat;
$x::$stat; 
X::${stat};    
$x::${stat};   
${x}::${stat};

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectStFldUse":{"PropertyName":"stat","DirectTypeRef":{"ClassName":"X"}},
  "DirectStFldUse":{"PropertyName":"stat","IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}},
  "IndirectStFldUse":{"GlobalConstUse":{"Name":"stat","FallbackName":""},"DirectTypeRef":{"ClassName":"X"}},
  "IndirectStFldUse":{"GlobalConstUse":{"Name":"stat","FallbackName":""},"IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}},
  "IndirectStFldUse":{"GlobalConstUse":{"Name":"stat","FallbackName":""},"IndirectTypeRef":{"IndirectVarUse":{"GlobalConstUse":{"Name":"x","FallbackName":""}}}}
}
