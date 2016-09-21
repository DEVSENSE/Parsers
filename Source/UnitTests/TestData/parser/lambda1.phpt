<?php
/** lambda doc */
$greet = function($name)
{
    print("Hello world");
};
                                             
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},   
  "PHPDocStmt":{"PHPDoc":{"Comment":"lambda doc"}},
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
    "DirectVarUse" : {"VarName" : "greet"},
    "LambdaFunctionExpr":{
      "UseParams":{},
      "FormalParams":{
        "FormalParam":{"Name":"name","PassedByRef":"False","IsVariadic":"False"}
      },
      "Body":{  
        "BlockStmt":{
          "UnaryEx" : {
            "Operation" : "Print",
            "StringLiteral" : {
                "Value" : "Hello world"
            }
          }
        }
      }
    }
  }  
}