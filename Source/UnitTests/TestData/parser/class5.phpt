<?php      
/** interface declaration */
interface SimpleInterface extends SimpleInter1, SimpleInter2
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
    "Name":"SimpleInterface",
    "MemberAttributes":"Interface",
    "IsConditional":"False",    
    "ImplementsList":{
      "Name":"SimpleInter1",
      "Name":"SimpleInter2"
    },
    "PHPDoc":{"Comment":"interface declaration"},
    "MethodDecl":{
        "Name":"aMemberFunc",
        "Modifiers":"Public",
        "PHPDoc":{"Comment":"method comment"},
        "FormalParams":{
          "FormalParam":{"Name":"arg_1","PassedByRef":"False","IsVariadic":"False"}
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