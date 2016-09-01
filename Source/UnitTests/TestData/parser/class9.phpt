<?php      
/** class declaration */
class SimpleClass
{
    /** method comment */
    abstract function aMemberFunc(boolean $arg_1): int;
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
      "Modifiers" : "Abstract",
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
      "ReturnType" : {
        "PrimitiveTypeRef" : {
          "QualifiedName" : "int"
        }
      }
    }
  }
}