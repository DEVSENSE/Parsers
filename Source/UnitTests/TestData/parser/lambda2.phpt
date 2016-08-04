<?php
/** lambda doc */
$greet = static function($name) use($x, $y)
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
        "FormalParam":{"Name":"x","PassedByRef":"False","IsVariadic":"False","InitValue":{}},
        "FormalParam":{"Name":"y","PassedByRef":"False","IsVariadic":"False","InitValue":{}}
      },
      "FormalParams":{
        "FormalParam":{"Name":"name","PassedByRef":"False","IsVariadic":"False","InitValue":{}}
      },
      "Body":{
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