<?php
/* example 1 */

$i = 1;
while ($i <= 10) {
    echo $i++;
}

/* example 2 */

$i = 1;
while ($i <= 10):
    echo $i;
    break;
endwhile;
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
    "DirectVarUse" : {
      "VarName" : "i"
    },
    "LongIntLiteral" : {
      "Value" : "1"
    }
  },
  "WhileStmt" : {
    "LoopType" : "While",
    "CondExpr" : {
      "BinaryEx" : {
        "Operation" : "LessThanOrEqual", 
        "DirectVarUse" : {
          "VarName" : "i"
        },
        "LongIntLiteral" : {
          "Value" : "10"
        }
      }
    },     
    "Body" : {
      "BlockStmt" : {
        "EchoStmt" : {
          "IncDecEx" : {
            "Inc" : "True",
            "Post" : "True",
            "DirectVarUse" : {
              "VarName" : "i"
            }
          }
        }
      }
    }
  },
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
    "DirectVarUse" : {
      "VarName" : "i"
    },
    "LongIntLiteral" : {
      "Value" : "1"
    }
  },
  "WhileStmt" : {
    "LoopType" : "While",     
    "CondExpr" : {
      "BinaryEx" : {
        "Operation" : "LessThanOrEqual",
        "DirectVarUse" : {
          "VarName" : "i"
        },
        "LongIntLiteral" : {
          "Value":"10"
        }
      }
    },   
    "Body" : {
      "BlockStmt" : {
        "EchoStmt" : {
          "DirectVarUse":{
            "VarName" : "i"
          }
        },
        "JumpStmt" : {
          "Type" : "Break"
        }
      }
    }
  }
}
