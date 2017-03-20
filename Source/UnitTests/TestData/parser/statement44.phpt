<?php

$myVar=&new MyClass();

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "ValueAssignEx":{
    "Operation":"AssignValue",
    "DirectVarUse":{"VarName":"myVar"},
    "NewEx":{
      "ByReference":"True",
      "ClassTypeRef":{"ClassName":"MyClass"}
    }
  }
}
