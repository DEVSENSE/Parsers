<?php

if ($a > $b)
  echo "a is bigger than b";

if ($a > $b) {
  echo "a is bigger than b";
  $b = $a;
}

if ($a > $b)
  echo "a is bigger than b";
else 
  $b = $a;
  
if ($a > $b)
  echo "a is bigger than b";
elseif ($a > 0) 
  $b = $a;


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
          "EchoStmt":{
            "StringLiteral":{"Value":"aisbiggerthanb"}
          }
        }
      }
    }
  },  
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
          "BlockStmt" : { 
            "EchoStmt":{
              "StringLiteral":{"Value":"aisbiggerthanb"}
            },
            "ValueAssignEx":{
              "Operation":"AssignValue",
              "DirectVarUse":{"VarName":"b"},
              "DirectVarUse":{"VarName":"a"}
            }
          }
        }
      }
    }
  }, 
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
          "EchoStmt":{
            "StringLiteral":{"Value":"aisbiggerthanb"}
          }
        }
      },
      "ConditionalStmt":{
        "Condition":{
        },
        "Statement":{
          "ValueAssignEx":{
            "Operation":"AssignValue",
            "DirectVarUse":{"VarName":"b"},
            "DirectVarUse":{"VarName":"a"}
          }
        }
      }
    }
  }, 
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
          "EchoStmt":{
            "StringLiteral":{"Value":"aisbiggerthanb"}
          }
        }
      },
      "ConditionalStmt":{
        "Condition":{
          "BinaryEx":{
            "Operation":"GreaterThan",
            "DirectVarUse":{"VarName":"a"},
            "LongIntLiteral":{"Value":"0"}
          }
        },
        "Statement":{
          "ValueAssignEx":{
            "Operation":"AssignValue",
            "DirectVarUse":{"VarName":"b"},
            "DirectVarUse":{"VarName":"a"}
          }
        }
      }
    }
  }
}
