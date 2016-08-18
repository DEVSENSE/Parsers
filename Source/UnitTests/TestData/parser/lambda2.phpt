<?php
/** lambda doc */
$greet = static function(string $name) use($x, $y): integer
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
      "UseParams":{
        "FormalParam":{"Name":"x","PassedByRef":"False","IsVariadic":"False"},
        "FormalParam":{"Name":"y","PassedByRef":"False","IsVariadic":"False"}
      },
      "FormalParams":{
        "FormalParam":{"Name":"name","PassedByRef":"False","IsVariadic":"False",
          "TypeHint":{"PrimitiveTypeRef":{"QualifiedName":"string"}}}
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
      },
      "ReturnType":{
        "PrimitiveTypeRef":{"QualifiedName":"integer"}
      }      
    }
  }  
}