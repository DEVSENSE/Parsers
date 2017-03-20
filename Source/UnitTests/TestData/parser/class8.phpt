<?php  

new MyClass(1, "hello");
    
/** class declaration */
$x = new class(1, "world")
{
    
    private static $var = 'a default value' /** property declaration */, 
            $vara = 10 /** second doc */;
                    
    /** default property declaration */     
    var $pvar = 1; 
                    
    /** constant declaration */     
    public const cvar = 22, cvar = 10;
    
    final $varabc /** second doc */;
}
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NewEx":{
    "ByReference":"False",
    "ClassTypeRef":{"ClassName":"MyClass"},
    "ActualParam":{"IsUnpack":"False","LongIntLiteral":{"Value":"1"}},
    "ActualParam":{"IsUnpack":"False","StringLiteral":{"Value":"hello"}}
  },   
  "PHPDocStmt":{"PHPDoc":{"Comment":"class declaration"}},
  "ValueAssignEx":{
    "Operation":"AssignValue",
    "DirectVarUse":{"VarName":"x"},
    "NewEx":{
      "ByReference":"False",
      "AnonymousTypeDecl":{
        "MemberAttributes":"Public",
        "IsConditional":"False",
        "FieldDeclList":{
          "FieldDecl":{"Name":"var","MemberAttributes":"Error","PHPDoc":{"Comment":"propertydeclaration"},"StringLiteral":{"Value":"adefaultvalue"}},
          "FieldDecl":{"Name":"vara","MemberAttributes":"Error","PHPDoc":{"Comment":"seconddoc"},"LongIntLiteral":{"Value":"10"}}
        },
        "FieldDeclList":{
          "PHPDoc":{"Comment":"defaultpropertydeclaration"},
          "FieldDecl":{"Name":"pvar","MemberAttributes":"Public","LongIntLiteral":{"Value":"1"}}
        },
        "ConstDeclList":{
          "PHPDoc":{"Comment":"constantdeclaration"},
          "ClassConstantDecl":{"Name":"cvar","MemberAttributes":"Public","LongIntLiteral":{"Value":"22"}},
          "ClassConstantDecl":{"Name":"cvar","MemberAttributes":"Public","LongIntLiteral":{"Value":"10"}}
        },
        "FieldDeclList":{
          "FieldDecl":{"Name":"varabc","MemberAttributes":"Final","PHPDoc":{"Comment":"seconddoc"}}
        }
      },
      "ActualParam":{"IsUnpack":"False","LongIntLiteral":{"Value":"1"}},
      "ActualParam":{"IsUnpack":"False","StringLiteral":{"Value":"world"}}
    }
  } 
}
