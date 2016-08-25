<?php     
/** namespace1 */
namespace MyProject;  
                    
/** namespace2 */
namespace MyProject\AnotherProject;
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "PHPDocBlockStatement":{"PHPDoc":{"Comment":"namespace1"}},
  "NamespaceDecl":{
    "Name":"MyProject",
    "SimpleSyntax":"True",
    "NamingContext":{"Namespace":"MyProject"},
    "Body":{
      "BlockStmt":{
        "PHPDocBlockStatement":{"PHPDoc":{"Comment":"namespace2"}}
      }
    }
  },
  "NamespaceDecl":{
    "Name":"MyProject\AnotherProject",
    "SimpleSyntax":"True",
    "NamingContext":{"Namespace":"MyProject\AnotherProject"},
    "Body":{
      "BlockStmt":{}
    }
  }
}
