<?php

X::$stat;
$x::$stat; 
X::${stat};    
$x::${stat};

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectStFldUse":{"PropertyName":"stat","DirectTypeRef":{"ClassName":"X"}},
  "DirectStFldUse":{"PropertyName":"stat","IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}},
  "IndirectStFldUse":{"GlobalConstUse":{"Name":"stat"},"DirectTypeRef":{"ClassName":"X"}},
  "IndirectStFldUse":{"GlobalConstUse":{"Name":"stat"},"IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}}}
}
