<?
class ClassXYZ2
{
	var $productCode = 4; //!$productCode!

	function method()
	{
		echo "products/$this->productCode/pricingconfigurations/"; //!productCode!
	}
}

$c = new ClassXYZ2;
$c->method();<<<TEST>>>

"GlobalCode":{
  "NamingContext":{},
  "EchoStmt":{
    "StringLiteral":{
      "Value":"<?classClassXYZ2{	var$productCode=4;//!$productCode!	functionmethod()	{		echo"products/$this->productCode/pricingconfigurations/";//!productCode!	}}$c=newClassXYZ2;$c->method();"
    }
  }
}