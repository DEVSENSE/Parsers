<?php
try {
    echo 'hello';   
} catch (TestEx|MyEx $e) {
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
          "MultipleTypeRef":{
            "DirectTypeRef":{
              "ClassName":"TestEx"
            },         
            "DirectTypeRef":{
              "ClassName":"MyEx"
            }
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
      },
      "CatchItem":{
        "TypeRef":{
          "DirectTypeRef":{
            "ClassName":"Exception"
          }
        },
        "Variable":{
          "DirectVarUse":{"VarName":"e"}
        },
        "Body":{
          "BlockStmt":{
            "EchoStmt":{
              "StringLiteral":{"Value":"all"}
            }
          }
        }
      }
    },
    "FinallyItem":{
      "Body":{
        "BlockStmt":{
          "EchoStmt":{
            "StringLiteral":{"Value":"finally"}
          }
        }
      }
    } 
  }
}
