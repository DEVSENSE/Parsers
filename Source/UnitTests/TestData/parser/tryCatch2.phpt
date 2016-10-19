<?php
try {
    echo 'hello';   
} catch (TestEx $e) {
    echo 'world';
}
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext":{},
  "TryStmt":{
    "Body":{
      "BlockStmt":{
        "EchoStmt":{
          "StringLiteral":{"Value":"hello"}
        }
      }
    },
    "Catches":{
      "CatchItem":{
        "TypeRef":{
          "ClassTypeRef":{
            "ClassName":"TestEx"
          }
        },
        "Variable":{
          "DirectVarUse":{"VarName":"e"}
        },
        "Body":{
          "BlockStmt":{
            "EchoStmt":{
              "StringLiteral":{"Value":"world"}
            }
          }
        }
      }
    }
  }
}
