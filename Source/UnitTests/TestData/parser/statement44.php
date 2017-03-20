<?php

$myVar=&new MyClass();

?>
<<<TEST>>>

ERRORS: 181

"GlobalCode":{
  "NamingContext":{},
  "RefAssignEx":{
    "Operation":"AssignRef",
    "DirectVarUse":{"VarName":"myVar"},
    "NewEx":{"ClassTypeRef":{"ClassName":"MyClass"}}
  }
}
