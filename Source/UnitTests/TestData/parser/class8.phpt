<?php  

new MyClass(1, "hello");
    
/** class declaration */
$x = new class(1, "world")
{
    
    private $var = 'a default value' /** property declaration */, 
            $vara = 10 /** second doc */;
                    
    /** default property declaration */     
    var $pvar = 1; 
                    
    /** constant declaration */     
    private const cvar = 22;
}
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NewEx":{"DirectTypeRef":{"ClassName":"MyClass"},
    "ActualParam":{"IsUnpack":"False","LongIntLiteral":{"Value":"1"}},
    "ActualParam":{"IsUnpack":"False","StringLiteral":{"Value":"hello"}}
  },
  "ValueAssignEx":{"Operation":"AssignValue","DirectVarUse":{"VarName":"x"},
    "NewEx":{
      "AnonymousTypeDecl":{
        "MemberAttributes":"Public",
        "IsConditional":"True",
        "PHPDoc":{"Comment":"classdeclaration"},
        "FieldDecl":{"Name":"var","MemberAttributes":"Private","PHPDoc":{"Comment":"propertydeclaration"},
          "StringLiteral":{"Value":"adefaultvalue"}
        },
        "FieldDecl":{"Name":"vara","MemberAttributes":"Private","PHPDoc":{"Comment":"seconddoc"},
          "LongIntLiteral":{"Value":"10"}
        },
        "FieldDecl":{"Name":"pvar","MemberAttributes":"Public","PHPDoc":{"Comment":"defaultpropertydeclaration"},
          "LongIntLiteral":{"Value":"1"}
        },
        "ClassConstantDecl":{"Name":"cvar","MemberAttributes":"Public","PHPDoc":{"Comment":"constantdeclaration"},
          "LongIntLiteral":{"Value":"22"}
        }
      },
      "ActualParam":{"IsUnpack":"False","LongIntLiteral":{"Value":"1"}},
      "ActualParam":{"IsUnpack":"False","StringLiteral":{"Value":"world"}}
    }
  }
}
