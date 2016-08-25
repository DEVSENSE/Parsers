<?php
__LINE__;
__FILE__;
__DIR__;
__FUNCTION__;
__METHOD__;
__CLASS__;
__TRAIT__;
__NAMESPACE__;   

echo <<<HEREDOC
HEREDOC;

echo <<<HEREDOC
My name is $name.
HEREDOC;

echo <<<'NOWDOC'

NOWDOC;

echo <<<'DOCDOC'
My name is $name.
DOCDOC;

?>
<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "EchoStmt":{"StringLiteral":{"Value":""}},
  "EchoStmt":{"ConcatEx":{"StringLiteral":{"Value":"My name is "},"DirectVarUse":{"VarName":"name"},"StringLiteral":{"Value":"."}}},
  "EchoStmt":{"StringLiteral":{"Value":""}},
  "EchoStmt":{"StringLiteral":{"Value":"Mynameis$name."}}
}
