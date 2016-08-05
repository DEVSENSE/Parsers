<?php
try {
    echo 'hello';   
} catch (TestEx $e) {
    echo 'world';
} catch (Exception $e) {
    echo 'all';
} finally {
    echo 'finally';
}
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext":{},
  "TryStmt":{
    "Statements":{
      "EchoStmt":{
        "StringLiteral":{"Value":"hello"}
      }
    },
    "Catches":{
      "CatchItem":{
        "TypeRef":{
          "DirectTypeRef":{
            "ClassName":"TestEx",
            "GenericParams":{}
          }
        },
        "Variable":{
          "DirectVarUse":{"VarName":"e"}
        },
        "Statements":{
          "EchoStmt":{
            "StringLiteral":{"Value":"world"}
          }
        }
      },
      "CatchItem":{
        "TypeRef":{
          "DirectTypeRef":{
            "ClassName":"Exception",
            "GenericParams":{}
          }
        },
        "Variable":{
          "DirectVarUse":{"VarName":"e"}
        },
        "Statements":{
          "EchoStmt":{
            "StringLiteral":{"Value":"all"}
          }
        }
      }
    },
    "FinallyItem":{
      "EchoStmt":{
        "StringLiteral":{"Value":"finally"}
      }
    } 
  }
}
