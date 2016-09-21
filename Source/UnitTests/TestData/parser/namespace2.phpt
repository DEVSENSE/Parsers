<?php     
/** namespace1 */
// empty
namespace MyProject;  

/** free comment */
                    
/** namespace2 */
namespace MyProject\AnotherProject;
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NamespaceDecl":{
    "Name":"MyProject",
    "SimpleSyntax":"True",
    "NamingContext":{"Namespace":"MyProject"},
    "PHPDoc":{"Comment":"namespace1"},
    "Body":{
      "BlockStmt":{
        "PHPDocStmt":{"PHPDoc":{"Comment":"free comment"}}
      }
    }
  },
  "NamespaceDecl":{
    "Name":"MyProject\AnotherProject",
    "SimpleSyntax":"True",
    "NamingContext":{"Namespace":"MyProject\AnotherProject"},
    "PHPDoc":{"Comment":"namespace2"},
    "Body":{
      "BlockStmt":{}
    }
  }
}
