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
  "GlobalConstUse":{"Name":"abc\included"},
  "GlobalConstUse":{"Name":"abc\included"},
  "GlobalConstUse":{"Name":"abc\included"},
  "HaltCompiler":{},
  "EchoStmt":{"StringLiteral":{"Value":""}}
}
