<?php
foreach ($arr as &$value):
    $value = $value * 2;
    continue;
endforeach;

foreach ($arr as $key => $value):
    // $arr[3] will be updated with each value from $arr...
    echo "hello";
    print_r(...$arr);
endforeach;
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "ForeachStmt":{
    "Enumeree":{
      "DirectVarUse":{
        "VarName":"arr"
      }
    },
    "ValueVariable":{
      "IndirectVarUse":{
        "DirectVarUse":{
          "VarName":"value"
        }
      }
    },
    "Body":{
      "BlockStmt":{
        "ValueAssignEx":{
          "Operation":"AssignValue",
          "DirectVarUse":{"VarName":"value"},
          "BinaryEx":{
            "Operation":"Mul",
            "DirectVarUse":{"VarName":"value"},
            "LongIntLiteral":{"Value":"2"}
          }
        },
        "JumpStmt":{
          "Type":"Continue"
        }
      }
    }
  },
  "ForeachStmt":{
    "Enumeree":{
      "DirectVarUse":{
        "VarName":"arr"
      }
    },
    "KeyVariable":{
      "DirectVarUse":{
        "VarName":"key"
      }
    },
    "ValueVariable":{
      "DirectVarUse":{
        "VarName":"value"
      }
    },
    "Body":{
      "BlockStmt":{
        "EchoStmt" : {
          "StringLiteral":{
            "Value":"hello"
          }
        },
        "DirectFcnCall" : {
          "Name" : "print_r", 
          "ActualParam" : {   
            "IsUnpack":"True",
            "DirectVarUse":{
              "VarName":"arr"
            }
          }
        }
      }
    }
  }
}
