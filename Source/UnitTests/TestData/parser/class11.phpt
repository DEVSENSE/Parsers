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

ERRORS: 1022, 1022, 1022

"GlobalCode":{
  "NamingContext":{},
  "NamedTypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "BaseClassName":{"Name":"int"},
    "ImplementsList":{"Name":"int"},
    "PHPDoc":{"Comment":"classdeclaration"}
  },
  "NamedTypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Interface",
    "IsConditional":"False",
    "ImplementsList":{"Name":"int"}
  }
}

