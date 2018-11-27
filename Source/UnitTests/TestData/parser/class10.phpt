<?php      
echo new class extends myA implements ArrayA
{
  
};
?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "EchoStmt":{
    "NewEx":{
      "AnonymousTypeDecl":{
        "MemberAttributes":"Public",
        "IsConditional":"False",
        "BaseClassName":{"ClassTypeRef":{"ClassName":"myA"}},
        "ImplementsList":{"ClassTypeRef":{"ClassName":"ArrayA"}}
      }
    }
  }
}
