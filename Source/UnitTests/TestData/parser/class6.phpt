<?php      
/** class declaration */
class SimpleClass
{
    use SayWorld;
    use A, B {
        B::foo insteadof A;
        A::bar insteadof B,A;
    }
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
      "Traits":{"Trait":"SayWorld"},    
      "TraitAdaptationBlock":{}
    },
    "TraitsUse":{
      "Traits":{"Trait":"A","Trait":"B"},
      "TraitAdaptationBlock":{
        "TraitAdaptationPrecedence":{"TraitMemberName":"B::foo","IgnoredTypes":{"IgnoredType":"A"}},
        "TraitAdaptationPrecedence":{"TraitMemberName":"A::bar","IgnoredTypes":{"IgnoredType":"B","IgnoredType":"A"}}
      }
    }
  }
}