<?php
/** function doc */
function foo($arg_1)
{
  global $a, $b;
  static $x, $y;
}

?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "FunctionDecl":{
    "Name":"foo",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"function doc"},
    "FormalParams":{
      "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"False","InitValue":{}}
    },
    "Body":{
      "GlobalStmt":{"DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
      "StaticStmt":{"StaticVarDecl":{"Name":"x"},"StaticVarDecl":{"Name":"y"}}
    }
  }
}