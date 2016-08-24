<?php
switch ($i):
    case 0:
        echo "i equals 0";
        break;
    case 1:
        echo "i equals 1";
        break;
    default:
        echo "default i";
        break;
endswitch;

switch ($i):
    ;
    case 0:
        echo "i equals 0";
        break;
    case 1:
        echo "i equals 1";
        break;
    default:
        echo "default i";
        break;
endswitch;
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "SwitchStmt":{
    "SwitchValue":{
      "DirectVarUse":{"VarName":"i"}
    },
    "SwitchItems":{
      "CaseItem":{
        "CaseVal":{
          "LongIntLiteral":{"Value":"0"}
        },
        "Statements":{
          "EchoStmt":{
            "StringLiteral":{"Value":"i equals 0"}
          },
          "JumpStmt":{"Type":"Break"}
        }
      }, 
      "CaseItem":{
        "CaseVal":{
          "LongIntLiteral":{"Value":"1"}
        },
        "Statements":{
          "EchoStmt":{
            "StringLiteral":{"Value":"i equals 1"}
          },
          "JumpStmt":{"Type":"Break"}
        }
      },
      "DefaultItem":{
        "Statements":{
          "EchoStmt":{
            "StringLiteral":{"Value":"default i"}
          },
          "JumpStmt":{"Type":"Break"}
        }
      }
    }
  },
  "SwitchStmt":{
    "SwitchValue":{
      "DirectVarUse":{"VarName":"i"}
    },
    "SwitchItems":{
      "CaseItem":{
        "CaseVal":{
          "LongIntLiteral":{"Value":"0"}
        },
        "Statements":{
          "EchoStmt":{
            "StringLiteral":{"Value":"i equals 0"}
          },
          "JumpStmt":{"Type":"Break"}
        }
      }, 
      "CaseItem":{
        "CaseVal":{
          "LongIntLiteral":{"Value":"1"}
        },
        "Statements":{
          "EchoStmt":{
            "StringLiteral":{"Value":"i equals 1"}
          },
          "JumpStmt":{"Type":"Break"}
        }
      },
      "DefaultItem":{
        "Statements":{
          "EchoStmt":{
            "StringLiteral":{"Value":"default i"}
          },
          "JumpStmt":{"Type":"Break"}
        }
      }
    }
  }  
}
