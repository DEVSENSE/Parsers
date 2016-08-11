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
      "IsUnpack":"False",
      "StringLiteral":{"Value":"hello"}
    }
  },     
  "LabelStmt" : {
    "Name" : "nothrow"
  }
}
