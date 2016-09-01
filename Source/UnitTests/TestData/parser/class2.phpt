<?php      
/** class declaration */
class SimpleClass
{
    /** method comment */
    protected function aMemberFunc(boolean $arg_1): int { 
        print 'Inside `aMemberFunc()`'; 
    } 
}
?>

<<<TEST>>>
"GlobalCode" : {
  "NamingContext" : {
  },
  "NamedTypeDecl" : {
    "Name" : "SimpleClass",
    "MemberAttributes" : "Public",
    "IsConditional" : "False",
    "PHPDoc" : {
      "Comment" : "class declaration"
    },
    "MethodDecl" : {
      "Name" : "aMemberFunc",
      "Modifiers" : "Protected",
      "PHPDoc" : {
        "Comment" : "method comment"
      },
      "FormalParams" : {
        "FormalParam" : {
          "Name" : "arg_1",
          "PassedByRef" : "False",
          "IsVariadic" : "False",
          "TypeHint" : {
            "DirectTypeRef" : {
              "ClassName" : "boolean"
            }
          }
        }
      },
      "Body" : {
        "BlockStmt" : {
          "UnaryEx" : {
            "Operation" : "Print",
            "StringLiteral" : {
              "Value" : "Inside `aMemberFunc()`"
            }
          }
        }
      },
      "ReturnType" : {
        "PrimitiveTypeRef" : {
          "QualifiedName" : "int"
        }
      }
    }
  }
}