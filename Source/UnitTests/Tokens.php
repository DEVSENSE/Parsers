<?php
$tokens = token_get_all(file_get_contents($argv[1]));

foreach ($tokens as $token) {
    if (is_array($token)) {
        echo $token[0], "-", token_name($token[0]), "-", trim(preg_replace('/\s\s+/', ' ', $token[1])), PHP_EOL;
    }
    else  {
        echo ord($token), "-", $token, "-", PHP_EOL;
    }
}
?>       