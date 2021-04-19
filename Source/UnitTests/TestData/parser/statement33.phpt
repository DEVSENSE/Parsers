<?php

abc\included;

\abc\included;

namespace\abc\included;

__halt_compiler // test
(/** php 
doc */)
/* comment*/
;


<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "GlobalConstUse":{"Name":"abc\included","OriginalName":"abc\included","FallbackName":""},
  "GlobalConstUse":{"Name":"abc\included","OriginalName":"abc\included","FallbackName":""},
  "GlobalConstUse":{"Name":"abc\included","OriginalName":"abc\included","FallbackName":""},
  "HaltCompiler":{},
  "EchoStmt":{"StringLiteral":{"Value":""}}
}
