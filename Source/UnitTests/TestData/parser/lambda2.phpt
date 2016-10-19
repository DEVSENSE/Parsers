<?php
/** lambda doc */
$greet = static function(string $name) use($x, &$y): integer
{
    /** var hint */
    print("Hello world");
};
                                             
?>
<<<TEST>>>
"GlobalCode" : {
  "NamingContext" : {
  },        
  "PHPDocStmt":{"PHPDoc":{"Comment":"lambda doc"}},
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
    "DirectVarUse" : {
      "VarName" : "greet"
    },
    "LambdaFunctionExpr" : {
      "UseParams" : {
        "FormalParam" : {
          "Name" : "x",
          "PassedByRef" : "False",
          "IsVariadic" : "False"
        },
        "FormalParam" : {
          "Name" : "y",
          "PassedByRef" : "True",
          "IsVariadic" : "False"
        }
      },
      "FormalParams" : {
        "FormalParam" : {
          "Name" : "name",
          "PassedByRef" : "False",
          "IsVariadic" : "False",
          "TypeHint" : {
            "PrimitiveTypeRef" : {
              "QualifiedName" : "string"
            }
          }
        }
      },
      "Body" : {
        "BlockStmt" : {  
          "PHPDocStmt":{"PHPDoc":{"Comment":"var hint"}},
          "UnaryEx" : {
            "Operation" : "Print",
            "StringLiteral" : {
              "Value" : "Hello world"
            }
          }
        }
      },
      "ReturnType" : {
        "ClassTypeRef" : {
          "ClassName" : "integer"
        }
      }
    }
  }
}