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
  "DirectStFldUse":{"PropertyName":"stat","ClassTypeRef":{"ClassName":"X"}},
  "DirectStFldUse":{"PropertyName":"stat","IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}},
  "IndirectStFldUse":{"GlobalConstUse":{"Name":"stat","OrigianlName":"stat","FallbackName":""},"ClassTypeRef":{"ClassName":"X"}},
  "IndirectStFldUse":{"GlobalConstUse":{"Name":"stat","OrigianlName":"stat","FallbackName":""},"IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}},
  "IndirectStFldUse":{"GlobalConstUse":{"Name":"stat","OrigianlName":"stat","FallbackName":""},"IndirectTypeRef":{"IndirectVarUse":{"GlobalConstUse":{"Name":"x","OrigianlName":"x","FallbackName":""}}}}
}
