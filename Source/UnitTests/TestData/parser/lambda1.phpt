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
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
    "DirectVarUse" : {"VarName" : "greet"},
    "LambdaFunctionExpr":{
      "PHPDoc":{"Comment":"lambda doc"},
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