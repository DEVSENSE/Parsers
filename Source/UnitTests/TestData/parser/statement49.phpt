<?php
echo ((object)$arr)->p;
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "EchoStmt":{
    "DirectVarUse":{
      "VarName":"p",
      "IsMemberOf":{"UnaryEx":{"Operation":"ObjectCast","DirectVarUse":{"VarName":"arr"}}}
    }
  }
}
