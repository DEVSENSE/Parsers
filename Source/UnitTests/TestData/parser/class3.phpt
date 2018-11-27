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
    "BaseClassName":{"ClassTypeRef":{"ClassName":"SimpleTrait"}},
    "ImplementsList":{
      "ClassTypeRef":{"ClassName":"SimpleInter1"},
      "ClassTypeRef":{"ClassName":"SimpleInter2"}
    },
    "PHPDoc":{"Comment":"class declaration"}
  }  
}