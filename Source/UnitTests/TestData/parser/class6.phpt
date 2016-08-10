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
  "TypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"classdeclaration"},
    
    "TraitsUse":{ 
      "Traits":{"Trait":"SayWorld"}
    },
    "TraitsUse":{
      "Traits":{"Trait":"A","Trait":"B"},
      "TraitAdaptationPrecedence":{"TraitMemberName":"B::foo","IgnoredTypes":{"IgnoredType":"A"}},
      "TraitAdaptationPrecedence":{"TraitMemberName":"A::bar","IgnoredTypes":{"IgnoredType":"B","IgnoredType":"A"}}
    }
  }
}