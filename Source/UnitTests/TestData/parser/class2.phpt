<?php      
/** class declaration */
class SimpleClass
{
    /** method comment */
    function aMemberFunc(boolean $arg_1): int { 
        print 'Inside `aMemberFunc()`'; 
    } 
}
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "NamedTypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"class declaration"},
    "MethodDecl":{
      "Name":"aMemberFunc",
      "Modifiers":"Public",
      "PHPDoc":{"Comment":"method comment"},
      "FormalParams":{
        "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"False",
          "TypeHint":{"PrimitiveTypeRef":{"QualifiedName":"boolean"}}}
      },
      "Body":{
        "BlockStmt":{
          "UnaryEx" : {
            "Operation" : "Print",
            "StringLiteral" : {
                "Value" : "Inside `aMemberFunc()`"
            }
          }
        }
      },
      "ReturnType":{
        "DirectTypeRef":{
          "ClassName":"int"
        }
      } 
    }
  }  
}