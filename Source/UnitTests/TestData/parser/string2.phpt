<?php
echo "{";
echo "
\75";  
echo "\x$x";		
foo('
');
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "EchoStmt":{"StringLiteral":{"Value":"{"}},
  "EchoStmt":{"StringLiteral":{"Value":"="}},
  "EchoStmt":{
    "ConcatEx":{
      "StringLiteral":{"Value":"\x"},
      "DirectVarUse":{"VarName":"x"}
    }
  },
  "DirectFcnCall":{
    "Name":"foo",
    "OrigianlName":"foo",
    "FallbackQualifiedName":"",
    "ActualParam":{
      "IsUnpack":"False",
      "StringLiteral":{"Value":""}
    }
  }
}
