<?php      
/** class declaration */
class SimpleClass
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

"GlobalCode" : {
  "NamingContext" : {},
  "NamedTypeDecl":{
    "Name":"SimpleClass",
    "MemberAttributes":"Public",
    "IsConditional":"False",
    "PHPDoc":{"Comment":"class declaration"},
    "FieldDeclList":{
      "FieldDecl":{"Name":"var","MemberAttributes":"Private","PHPDoc":{"Comment":"property declaration"},"StringLiteral":{"Value":"a default value"}}, 
      "FieldDecl":{"Name":"vara","MemberAttributes":"Private","PHPDoc":{"Comment":"second doc"},"LongIntLiteral":{"Value":"10"}}
    }, 
    "FieldDeclList":{
      "PHPDoc":{"Comment":"default property declaration"},
      "FieldDecl":{"Name":"pvar","MemberAttributes":"Public","PHPDoc":{"Comment":"default property declaration"},"LongIntLiteral":{"Value":"1"}}
    },
    "ConstDeclList":{
      "PHPDoc":{"Comment":"constant declaration"},
      "ClassConstantDecl":{"Name":"cvar","MemberAttributes":"Public","PHPDoc":{"Comment":"constant declaration"},"LongIntLiteral":{"Value":"22"}}
    }
  }  
}