<?php
/** function doc */
function foo(...$arg_1)
{
    function bar()
    {
        /** varhint */
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
          "FormalParams":{},
          "Body":{ 
            "BlockStmt":{     
              "PHPDocStmt":{"PHPDoc":{"Comment":"varhint"}},
              "JumpStmt":{"Type":"Return","DirectVarUse":{"VarName":"retval"}}
            }
          }
        },      
        "JumpStmt":{"Type":"Return","DirectVarUse":{"VarName":"retval"}}
      }
    }
  }
}