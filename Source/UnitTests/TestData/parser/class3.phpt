<?php      
/** class declaration */
class SimpleClass extends SimpleTrait implements SimpleInter1, SimpleInter2
{
}
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "NamedTypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "BaseClassName":{"Name":"SimpleTrait"},
    "ImplementsList":{
      "Name":"SimpleInter1",
      "Name":"SimpleInter2"
    },
    "PHPDoc":{"Comment":"class declaration"}
  }  
}