<?php      
/** class declaration */
class SimpleClass
{
    /** method comment */
    function aMemberFunc(boolean $arg_1) { 
        print 'Inside `aMemberFunc()`'; 
    } 
}
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "TypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"class declaration"},
    "MethodDecl":{
        "Name":"aMemberFunc",
        "Modifiers":"Public",
        "PHPDoc":{"Comment":"method comment"},
        "FormalParams":{
          "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"False","InitValue":{}}
        },
        "Body":{
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