<?php      
/** class declaration */
class SimpleClass extends int implements int
{
}

interface SimpleClass extends int
{
}

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NamedTypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "BaseClassName":{"ClassTypeRef":{"ClassName":"int"}},
    "ImplementsList":{"ClassTypeRef":{"ClassName":"int"}},
    "PHPDoc":{"Comment":"classdeclaration"}
  },
  "NamedTypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Interface",
    "IsConditional":"False",
    "ImplementsList":{"ClassTypeRef":{"ClassName":"int"}}
  }
}

