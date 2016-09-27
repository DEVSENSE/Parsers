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
			"ConstantAliases":{
				"LCONST":"MySpace\GCONST",
				"lconst":"MySpace\gconst"
			}
		},
		"Body":{
			"BlockStmt":{
				"GlobalConstUse":{"Name":"MySpace\gconst","FallbackName":""},
				"GlobalConstUse":{"Name":"MySpace\GCONST","FallbackName":""}
			}
		}
	}
}
