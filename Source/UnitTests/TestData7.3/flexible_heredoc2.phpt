<?php
stringManipulator(<<<END
   a
  b
 c
END
);
 
$values = [<<<END
a
b
c
END, 'd e f'];
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "DirectFcnCall":{
    "Name":"stringManipulator",
    "OriginalName":"stringManipulator",
    "FallbackQualifiedName":"",
    "ActualParam":{"IsUnpack":"False","StringLiteral":{"Value":"abc"}}
  },
  "ValueAssignEx":{
    "Operation":"AssignValue",
    "DirectVarUse":{"VarName":"values"},
    "ArrayEx":{
      "Item":{"ValueExpr":{"StringLiteral":{"Value":"abc"}}},
      "Item":{"ValueExpr":{"StringLiteral":{"Value":"def"}}}
    }
  }
}