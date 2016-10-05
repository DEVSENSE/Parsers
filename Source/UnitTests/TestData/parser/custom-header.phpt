<?php
function foo() {
//?
if ($a):
elseif ($b):
endif;
}
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "FunctionDecl":{
    "Name":"foo",
    "IsConditional":"False",
    "FormalParams":{},
    "Body":{
      "BlockStmt":{
        "IfStmt":{
          "CondList":{
            "ConditionalStmt":{
              "Condition":{"DirectVarUse":{"VarName":"a"}},
              "Statement":{"BlockStmt":{}}},
              "ConditionalStmt":{"Condition":{"DirectVarUse":{"VarName":"b"}},
              "Statement":{"BlockStmt":{}}
            }
          }
        }
      }
    }
  }
}
