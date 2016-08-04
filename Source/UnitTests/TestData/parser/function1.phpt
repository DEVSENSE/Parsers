<?php
/** function doc */
function foo(...$arg_1)
{
    function bar($arg_2)
    {
        return $retval;
    }
    return $retval;
}

?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "FunctionDecl":{
    "Name":"foo",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"functiondoc"},
    "FormalParams":{
      "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"True","InitValue":{}}
    },
    "Body":{
      "FunctionDecl":{
        "Name":"bar",
        "IsConditional":"True",
        "FormalParams":{
          "FormalParam":{"Name":"arg_2","PassedByRef":"False","IsVariadic":"False","InitValue":{}}
        },
        "Body":{
          "JumpStmt":{"Type":"Return","DirectVarUse":{"VarName":"retval"}}
        }
      },      
      "JumpStmt":{"Type":"Return","DirectVarUse":{"VarName":"retval"}}
    }
  }
}