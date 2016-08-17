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
    "PHPDoc":{"Comment":"function doc"},
    "FormalParams":{
      "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"True"}
    },
    "Body":{  
      "BlockStmt":{
        "FunctionDecl":{
          "Name":"bar",
          "IsConditional":"True",
          "FormalParams":{
            "FormalParam":{"Name":"arg_2","PassedByRef":"False","IsVariadic":"False"}
          },
          "Body":{ 
            "BlockStmt":{
              "JumpStmt":{"Type":"Return","DirectVarUse":{"VarName":"retval"}}
            }
          }
        },      
        "JumpStmt":{"Type":"Return","DirectVarUse":{"VarName":"retval"}}
      }
    }
  }
}