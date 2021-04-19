<?php
namespace test{
	
  use const MySpace\GCONST as LCONST;
  use const MySpace\gconst as lconst;
  
  lconst;
  LCONST;
}
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "NamespaceDecl":{
    "Name":"test",
    "SimpleSyntax":"False",
    "NamingContext":{
      "Namespace":"test",
      "Aliases" : {},
      "ConstantAliases":{
        "LCONST":"MySpace\GCONST",
        "lconst":"MySpace\gconst"
      },
      "FunctionAliases":{}
    },
    "Body":{
      "BlockStmt":{
        "UseStatement":{"Kind":"Constant","Aliases":{"LCONST":{}}},
        "UseStatement":{"Kind":"Constant","Aliases":{"lconst":{}}},
        "GlobalConstUse":{"Name":"MySpace\gconst","OriginalName":"lconst","FallbackName":""},
        "GlobalConstUse":{"Name":"MySpace\GCONST","OriginalName":"LCONST","FallbackName":""}
      }
    }
  }
}
