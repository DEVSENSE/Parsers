<?php      
/** trait declaration */
trait SimpleTrait
{
    /** method comment */
    function aMemberFunc($arg_1) { 
        print 'Inside `aMemberFunc()`'; 
    } 
}
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "NamedTypeDecl":{
    "Name":"SimpleTrait",
    "MemberAttributes":"Trait",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"trait declaration"},
    "MethodDecl":{
        "Name":"aMemberFunc",
        "Modifiers":"Public",
        "PHPDoc":{"Comment":"method comment"},
        "FormalParams":{
          "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"False"}
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
        }
      }
  }  
}