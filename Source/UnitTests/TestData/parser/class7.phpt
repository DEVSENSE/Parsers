<?php      
/** class declaration */
class SimpleClass
{
  use HelloWorld {     
    sayHello as myPrivateHello; 
    sayHello as endswitch; 
    sayHello as private myPrivateHello;  
    sayHello as private; 
  }
}
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "TypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"classdeclaration"},

    "TraitsUse":{
      "Traits":{"Trait":"HelloWorld"},
      "TraitAdaptationAlias":{"TraitMemberName":"sayHello","NewName":"myPrivateHello","NewModifier":"Public"},
      "TraitAdaptationAlias":{"TraitMemberName":"sayHello","NewName":"endswitch","NewModifier":"Public"},
      "TraitAdaptationAlias":{"TraitMemberName":"sayHello","NewName":"myPrivateHello","NewModifier":"Private"},
      "TraitAdaptationAlias":{"TraitMemberName":"sayHello","NewName":"","NewModifier":"Private"}
    }
  }
}