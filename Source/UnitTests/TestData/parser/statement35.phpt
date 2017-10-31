<?php
foreach($a as list($b, $c))
    echo "hello";
foreach($a as [$b, $c] => $d)
    echo "hello";
?>
<<<TEST>>>

"GlobalCode":{"NamingContext":{},
  "ForeachStmt":{
    "Enumeree":{"DirectVarUse":{"VarName":"a"}},
    "ValueVariable":{"ListEx":{"Item":{"ValueExpr":{"DirectVarUse":{"VarName":"b"}}},"Item":{"ValueExpr":{"DirectVarUse":{"VarName":"c"}}}}},
    "Body":{"EchoStmt":{"StringLiteral":{"Value":"hello"}}}
  },
  "ForeachStmt":{
    "Enumeree":{"DirectVarUse":{"VarName":"a"}},
    "KeyVariable":{"ArrayEx":{"Item":{"ValueExpr":{"DirectVarUse":{"VarName":"b"}}},"Item":{"ValueExpr":{"DirectVarUse":{"VarName":"c"}}}}},
    "ValueVariable":{"DirectVarUse":{"VarName":"d"}},
    "Body":{"EchoStmt":{"StringLiteral":{"Value":"hello"}}}
  }
}
