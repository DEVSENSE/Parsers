<?php
goto nothrow;
throw new Exception('hello');
nothrow:
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext":{
  },
  "GotoStmt" : {
    "LabelName" : "nothrow"
  },
  "NewEx":{
    "DirectTypeRef":{
      "ClassName":"Exception",
      "GenericParams":{}
    },
    "ActualParam":{ 
      "StringLiteral":{"Value":"hello"}
    }
  },     
  "LabelStmt" : {
    "Name" : "nothrow"
  }
}
