<?php

X::func($a);
X::$funcname($a);
X::{$a}($a);
$classname::func($a);  
$x::{$a}($a);

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectStMtdCall":{"ClassName":"X","MethodName":"func","ClassTypeRef":{"ClassName":"X"},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "IndirectStMtdCall":{"ClassTypeRef":{"ClassName":"X"},"DirectVarUse":{"VarName":"funcname"},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "IndirectStMtdCall":{"ClassTypeRef":{"ClassName":"X"},"DirectVarUse":{"VarName":"a"},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "DirectStMtdCall":{"ClassName":"","MethodName":"func","IndirectTypeRef":{"DirectVarUse":{"VarName":"classname"}},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "IndirectStMtdCall":{"IndirectTypeRef":{"DirectVarUse":{"VarName":"x"}},"DirectVarUse":{"VarName":"a"},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}}
}
