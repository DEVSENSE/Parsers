<?php
/* example 1 */

$i = 1;
do {
    echo ($i > 0)? $i: "hello";
} while ($i <= 10);

/* example 2 */

for ($i = 0; $i <= 10; $i++, $i+= 2):
    echo $i;
    break;
endfor; 
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
    "DirectVarUse" : {"VarName" : "i"},
    "LongIntLiteral" : {"Value" : "1"}
  },
  "WhileStmt" : {
    "LoopType" : "Do",
    "CondExpr" : {
      "BinaryEx" : {
        "Operation" : "LessThanOrEqual", 
        "DirectVarUse" : {"VarName" : "i"},
        "LongIntLiteral" : {"Value" : "10"}
      }
    },     
    "Body" : {
      "BlockStmt" : { 
        "EchoStmt":{
          "ConditionalEx":{
            "CondExpr":{
              "BinaryEx":{
                "Operation":"GreaterThan",
                "DirectVarUse":{"VarName":"i"},
                "LongIntLiteral":{"Value":"0"}
              }
            },
            "TrueExpr":{   
              "DirectVarUse" : {"VarName" : "i"}
            },
            "FalseExpr":{
              "StringLiteral":{"Value":"hello"}
            }
          }
        }
      }
    }
  },
  "WhileStmt":{
    "InitExList":{
      "ValueAssignEx":{
        "Operation":"AssignValue",
        "DirectVarUse":{"VarName":"i"},
        "LongIntLiteral":{"Value":"0"}
      }
    },
    "CondExList":{
      "BinaryEx":{
        "Operation":"LessThanOrEqual",
        "DirectVarUse":{"VarName":"i"},
        "LongIntLiteral":{"Value":"10"}
      }
    },
    "ActionExList":{
      "IncDecEx":{
        "Inc":"True",
        "Post":"True",
        "DirectVarUse":{"VarName":"i"}
      },
      "ValueAssignEx" : {
        "Operation" : "AssignAdd",
        "DirectVarUse" : {"VarName" : "i"},
        "LongIntLiteral" : {"Value" : "2"}
      }
    },
    "Body":{
      "BlockStmt":{
        "EchoStmt":{
          "DirectVarUse":{"VarName":"i"}
        },
        "JumpStmt":{"Type":"Break"}
      }
    }
  }
}
