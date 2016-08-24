<?php
/** function doc */
function foo($arg_1)
{
  global $a, $b;
  static $x, $y = 0;
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
      "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"False"}
    },
    "Body":{  
      "BlockStmt":{
        "GlobalStmt":{"DirectVarUse":{"VarName":"a"},"DirectVarUse":{"VarName":"b"}},
        "StaticStmt":{"StaticVarDecl":{"Name":"x"},"StaticVarDecl":{"Name":"y","LongIntLiteral":{"Value":"0"}}}
      }
    }
  }
}