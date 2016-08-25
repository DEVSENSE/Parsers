<?php
if ($a > $b)
{
  /** function doc */
  function & foo(MyClass $arg_1, ?integer $arg_2 = 4, &$arg_n = 1, callable $a): array
  {
      echo "Example function.\n", "hello";
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
                "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"False",
                  "TypeHint":{"DirectTypeRef":{"ClassName":"MyClass"}}},
                "FormalParam":{"Name":"arg_2","PassedByRef":"False","IsVariadic":"False",
                  "TypeHint":{"NullableTypeRef":{"PrimitiveTypeRef":{"QualifiedName":"integer"}}},
                  "InitValue":{"LongIntLiteral":{"Value":"4"}}},
                "FormalParam":{"Name":"arg_n","PassedByRef":"True","IsVariadic":"False",
                  "InitValue":{"LongIntLiteral":{"Value":"1"}}}, 
                "FormalParam":{"Name":"a","PassedByRef":"False","IsVariadic":"False",  
                  "TypeHint":{"PrimitiveTypeRef":{"QualifiedName":"callable"}}}
              },
              "Body":{ 
                "BlockStmt":{
                  "EchoStmt":{"StringLiteral":{"Value":"Example function."},"StringLiteral":{"Value":"hello"}},
                  "JumpStmt":{"Type":"Return","DirectVarUse":{"VarName":"retval"}}
                }
              }, 
              "ReturnType":{
                "PrimitiveTypeRef":{"QualifiedName":"array"}
              }
            }
          }
        }
      }
    }
  }
}