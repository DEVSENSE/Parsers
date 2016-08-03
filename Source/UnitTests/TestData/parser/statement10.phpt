<?php
$output = `ls -al`;
print("Hello World");
print 'foo is $foo'; 
?>
<<<TEST>>>

"GlobalCode" : {
  "NamingContext" : {},
  "ValueAssignEx" : {
    "Operation" : "AssignValue",
    "DirectVarUse" : {
      "VarName" : "output"
    },
    "ShellEx" : {
      "StringLiteral":{
        "Value":"ls -al"
      }
    }
  },
  "UnaryEx" : {
    "Operation" : "Print",
    "StringLiteral" : {
        "Value" : "Hello World"
    }
  },
  "UnaryEx" : {
    "Operation" : "Print",
    "StringLiteral" : {
        "Value" : "foo is $foo"
    }
  }
}
