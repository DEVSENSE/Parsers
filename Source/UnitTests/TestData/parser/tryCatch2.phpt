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
      }
    },
    "FinallyItem":{} 
  }
}
