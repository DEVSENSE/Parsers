<?php      
/** class declaration */
class SimpleClass extends MyClass
{
    /** method comment */
    protected function aMemberFunc(parent $arg_1): self {
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
    "BaseClassName":{"ClassTypeRef":{"ClassName":"MyClass"}},
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
            "TranslatedTypeRef":{
              "ClassName":"MyClass",
              "OriginalName":"parent"
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
        "TranslatedTypeRef":{
          "ClassName":"SimpleClass",
          "OriginalName":"self"
        }
      }
    }
  }
}