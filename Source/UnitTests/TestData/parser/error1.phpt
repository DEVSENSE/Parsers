<?php  
namespace
{    
  echo "hello1"
  echo "hello2"
  echo "world";
}
?>
<<<TEST>>>

ERRORS: 2014

"GlobalCode":{
  "NamingContext":{},
  "NamespaceDecl":{
    "Name":"",
    "SimpleSyntax":"False",
    "NamingContext":{},
    "Body":{
      "BlockStmt":{
        "EchoStmt":{"StringLiteral":{"Value":"hello1"}},
        "EchoStmt":{"StringLiteral":{"Value":"hello2"}},
        "EchoStmt":{"StringLiteral":{"Value":"world"}}
      }
    }
  }
}
