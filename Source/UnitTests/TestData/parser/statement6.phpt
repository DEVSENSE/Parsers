<?php
foreach ($arr as &$value)
    $value = $value * 2;

foreach ($arr as $key => $value) {
    // $arr[3] will be updated with each value from $arr...
    echo "hello";
    print_r($arr);
}
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
    "KeyVariable":{},
    "Body":{
      "ValueAssignEx":{
        "Operation":"AssignValue",
        "DirectVarUse":{"VarName":"value"},
        "BinaryEx":{
          "Operation":"Mul",
          "DirectVarUse":{"VarName":"value"},
          "LongIntLiteral":{"Value":"2"}
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
            "IsUnpack":"False",
            "DirectVarUse":{
              "VarName":"arr"
            }
          }
        }
      }
    }
  }
}
