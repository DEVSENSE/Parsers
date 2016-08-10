<?php

X::func($a);
X::$funcname($a);
X::{$a}($a);
$classname::func($a);

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectStMtdCall":{"ClassName":"X","MethodName":"func","DirectTypeRef":{"ClassName":"X","IsNullable":"False","GenericParams":{}},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "IndirectStMtdCall":{"DirectTypeRef":{"ClassName":"X","IsNullable":"False","GenericParams":{}},"DirectVarUse":{"VarName":"funcname"},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "IndirectStMtdCall":{"DirectTypeRef":{"ClassName":"X","IsNullable":"False","GenericParams":{}},"DirectVarUse":{"VarName":"a"},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}},
  "DirectStMtdCall":{"ClassName":"","MethodName":"func","IndirectTypeRef":{"DirectVarUse":{"VarName":"classname"}},"ActualParam":{"IsUnpack":"False","DirectVarUse":{"VarName":"a"}}}
}
