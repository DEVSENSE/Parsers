<?php

declare(ticks="hello");

declare(x=0, y=1){
    $b = 2;
} //closing curly bracket tickable  

declare(x=0, y=1):
    $b = 2;
enddeclare;

?>
<<<TEST>>>

"GlobalCode" : {    
  "NamingContext":{},
  "DeclareStmt":{},
  "DeclareStmt":{  
    "BlockStmt":{ 
      "ValueAssignEx" : {
        "Operation" : "AssignValue", 
        "DirectVarUse" : {
          "VarName" : "b"
        },
        "LongIntLiteral" : {
          "Value" : "2"
        }
      }
    }
  } ,
  "DeclareStmt":{  
    "BlockStmt":{ 
      "ValueAssignEx" : {
        "Operation" : "AssignValue", 
        "DirectVarUse" : {
          "VarName" : "b"
        },
        "LongIntLiteral" : {
          "Value" : "2"
        }
      }
    }
  }
}
