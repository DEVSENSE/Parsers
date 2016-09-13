<?php      
/** class declaration */
class SimpleClass
{
  use HelloWorld {     
    sayHello as myPrivateHello; 
    HelloWorld::sayHello as endswitch; 
    sayHello as private myPrivateHello;  
    sayHello as private; 
  }     
  use EmptyTrait {}
}
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NamedTypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"classdeclaration"},

    "TraitsUse":{
      "Traits":{"Trait":"HelloWorld"},    
      "TraitAdaptationBlock":{
        "TraitAdaptationAlias":{"TraitMemberName":"sayHello","NewName":"myPrivateHello","NewModifier":"Public"},
        "TraitAdaptationAlias":{"TraitMemberName":"HelloWorld::sayHello","NewName":"endswitch","NewModifier":"Public"},
        "TraitAdaptationAlias":{"TraitMemberName":"sayHello","NewName":"myPrivateHello","NewModifier":"Private"},
        "TraitAdaptationAlias":{"TraitMemberName":"sayHello","NewName":"","NewModifier":"Private"}
      }
    },      
    "TraitsUse":{
      "Traits":{"Trait":"EmptyTrait"},    
      "TraitAdaptationBlock":{}
    }
  }
}