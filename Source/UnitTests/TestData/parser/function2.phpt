<?php
if ($a > $b)
{
  /** function doc */
  function foo($arg_1, $arg_2 = 4, &$arg_n): int
  {
      echo "Example function.\n";
      return $retval;
  }
}

?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "IfStmt":{
    "CondList":{
      "ConditionalStmt":{
        "Condition":{
          "BinaryEx":{
            "Operation":"GreaterThan",
            "DirectVarUse":{"VarName":"a"},
            "DirectVarUse":{"VarName":"b"}
          }
        },
        "Statement":{
          "BlockStmt":{
            "FunctionDecl":{
              "Name":"foo",  
              "IsConditional":"True",
              "PHPDoc":{"Comment":"function doc"},
              "FormalParams":{
                "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"False","InitValue":{}},
                "FormalParam":{"Name":"arg_2","PassedByRef":"False","IsVariadic":"False","InitValue":{"LongIntLiteral":{"Value":"4"}}},
                "FormalParam":{"Name":"arg_n","PassedByRef":"True","IsVariadic":"False","InitValue":{}}
              },
              "Body":{
                "EchoStmt":{"StringLiteral":{"Value":"Examplefunction."}},
                "JumpStmt":{"Type":"Return","DirectVarUse":{"VarName":"retval"}}
              }
            }
          }
        }
      }
    }
  }
}