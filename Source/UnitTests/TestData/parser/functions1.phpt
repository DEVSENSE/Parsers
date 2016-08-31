<?php
function sb_cabinet_button(){
	global $wp_admin_bar;
    $args = array(
        'id' => 'sb_cabinet',
        'title' => 'Управление займами',
        'href' => '/sb_cabinet'
    );
    $wp_admin_bar->add_node($args);
}
function sb_back_to_site_button(){
	global $wp_admin_bar;
    $args = array(
        'id' => 'back_to_site',
        'title' => 'Назад на сайт',
        'href' => '/'
    );
    $wp_admin_bar->add_node($args);
}

class mfunny_cabs_funcs {
function __construct() {	
	$this->site_url = site_url();
	$this->m_url = $this->site_url.'/sb_cabinet/';
	$this->admin_url = $this->site_url.'/wp-admin/admin.php?page=mfunny_cabs';
	$this->m_dir = str_replace('\\', '/', dirname(__FILE__)).'/';
	$this->site_dir = str_replace('wp-content/plugins/mfunny-cabs/', '', $this->m_dir);
	$this->cur_link = $this->site_url . $_SERVER["REQUEST_URI"];
	$this->site_url = $this->site_url.'/';
	$this->pl_url = $this->site_url.'wp-content/plugins/mfunny-cabs/';
	$this->edit2 = array(
		'card',
		'tariff'
	);
	$this->moderate = '';
	$this->moderate2 = '';
	$this->is_sb = current_user_can('loan_view_sb');
	$this->is_manager = current_user_can('loan_view_manager');
	$this->is_buh = current_user_can('loan_view_buh');
	$this->is_investor = current_user_can('loan_view_investor');
	$this->is_collector = current_user_can('loan_view_collector');
	$this->is_admin = current_user_can('administrator');
	$this->user_id = get_current_user_id();
	$this->date = gmdate('Y-m-d H:i:s');
	$this->date_local = $this->utc_to_local($this->date);
	$this->date_local_short = explode(' ', $this->date_local);
	$this->date_local_short = $this->date_local_short[0];
	$this->stats = array();
	$this->stats_full = array();
	
	$this->can_edit['sb'] =
	$this->can_edit['manager'] =
	$this->can_edit['buh'] =
	array(
	'tariff',
	'amount',
	'term',
	'date',
	'manager_calls2',
	'pay_date',
	'sname',
	'fname',
	'tname',
	'sex',
	'birth',
	'propiska',
	'address',
	'seconddoc',
	'branch',
	'photo',
	'images',
	'paspser',
	'paspnom',
	'paspkem',
	'paspdate',
	'birthaddr',
	'fulladdr',
	'phone1',
	'phone2',
	'email',
	'relative',
	'relsname',
	'relfname',
	'reltname',
	'relphone',
	'card',
	'last_comment4',
	'loan_method',
	'manager_calls2_comment',
	'street1',
	'house_number1',
	'city1',
	'apartment1',
	'street2',
	'house_number2',
	'city2',
	'apartment2',
	'pasp_location',
	'street_type1',
	'street_type2',
	'idea',
	'force_percent',
	'corp1',
	'corp2',
	'structure1',
	'structure2',
	'address_same'
	);
	
	$this->can_edit['manager'][] = 'manager_calls';
	$this->can_edit['manager'][] = 'purpose';
	
	$this->can_edit['admin'] =
	array(
	'sb_id',
	'manager_id',
	'buh_id',
	'collector_id'
	);
	
	$this->can_edit['collector'] = array();
	
	$this->privileges = array();
	if ($this->is_sb) {
		$this->privileges = array_merge($this->privileges, $this->can_edit['sb']);
	}
	if ($this->is_manager) {
		$this->privileges = array_merge($this->privileges, $this->can_edit['manager']);
	}
	if ($this->is_buh) {
		$this->privileges = array_merge($this->privileges, $this->can_edit['buh']);
	}
	if ($this->is_collector) {
		$this->privileges = array_merge($this->privileges, $this->can_edit['collector']);
	}
	if ($this->is_admin) {
		$this->privileges = array_merge($this->privileges, $this->can_edit['admin']);
	}
	
	$this->privileges = array_unique($this->privileges);
	if (($this->is_admin) or ($this->is_sb) or ($this->is_manager) or ($this->is_buh) or ($this->is_collector)) {
		add_action('admin_bar_menu', 'sb_cabinet_button', 31);
		add_action('admin_bar_menu', 'sb_back_to_site_button', 31);
	}
	
	// закрытие на тех-обслуживание
	//if (($this->user_id != 21) and (!$_POST['ajaxing'])) {
	//	$this->error_tpl($capture='Система на техобслуживании', $content='В настоящий момент в системе управления займами ведутся технические работы. Попробуйте повторить попытку чуть позже.');
	//}
	
	if ($this->user_id == 21) {
		ini_set('display_startup_errors',1);
		ini_set('display_errors',1);
		error_reporting(E_ERROR | E_WARNING | E_PARSE);
		
		if (!$_POST['ajaxing']) {
			ini_set('upload_max_filesize', '256M');
			ini_set('post_max_size', '512M');
			ini_set('memory_limit', '512M');
			ini_set('max_execution_time', '360');
						
			// Ящик Пандоры - не открывать!
			//$this->check_unique_pasp();
			//$this->check_unique_email();
			//$this->check_unique_contract_id();
			
			//$msg_sms = $this->send_sms_simple('Testy test', '79268840608');
			//$this->send_email('maestro.magnifico@mail.ru', 'testytest', 'Тестовое письмо');
			
			
			//$this->create_shit_load_of_fakes();
			//$this->adjust_fakes_amount(2016, 2, 25642730, true);
			//$this->adjust_fakes_amount(2016, 3, 28889850, true);
			//$this->adjust_fakes_amount(2015, 12, 16970800);
			//$this->adjust_fakes_amount(2016, 1, 19250900);
			
			//echo count($this->get_loans_by_month(2015, 10, 'pay_date')).'<br>';
			//echo count($this->get_loans_by_month(2015, 11, 'pay_date')).'<br>';
			//echo count($this->get_loans_by_month(2015, 12, 'pay_date')).'<br>';
			//echo count($this->get_loans_by_month(2016, 1, 'pay_date')).'<br>';die;
			//$this->check_loans_amount(2016, array(1));
			//mysql_query("UPDATE `wp_loans` SET `nbki_hide`=1 WHERE `fake` !=0");
			
			//$this->check_tutdf_content();
			//$this->check_unique_fakes();
			//$this->fix_db_names();
			//$this->seo_fix_url();
			//$this->fix_june_contracts();
			//$this->send_email('maestro.magnifico@mail.ru', 'Тестовое сообщение', 'Кредитная история', 'nbki@moneyfunny.ru', $this->site_dir.'wp-content/uploads/nbki_logs/4L01GG000001_20160303_160948.zip.p7m');
			
			//echo '<pre>'; var_export($_SERVER); echo '</pre>'; // отладка
			
			//$graph = $this->get_pay_graph(39931);
			//echo '<pre>'; var_export($graph['last_graph']); echo '</pre>'; // отладка
			//$this->recount_back_date(39931);
			
			//$row = $this->get_main_row($_GET['id']);
			//$stats = $this->get_buh_stats($row);
			//echo '<pre>'; var_export($stats); echo '</pre>'; // отладка
			//die;
			
			//$this->fix_db_names();
			//$this->set_fakes_contracts();
			//$this->move_fakes_from_hollydays();
		}
	} else {
		//die('Сайт на тех.обслуживании.'); // если крутим на боевом
	}
	
}

/* Костыли
------------------------------------------------------------------------------*/

/*
 * Функция работает странно, перенесла не все дни
 **/
function move_fakes_from_hollydays() {
	$holly_year = 2016;
	$holly_month = 3;
	$hollydays = array(7,8);
	
	$n = 0;
	$ids = $this->get_loans_by_month($holly_year, $holly_month, 'pay_date', "AND `fake` != 0", "ASC");
	foreach ($ids as $id) {
		$query = mysql_query("SELECT * FROM `wp_loans` WHERE `id`=$id");
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			
			$row['pay_date'] = $this->utc_to_local($row['pay_date']);
			list($date, $time) = explode(' ', $row['pay_date']);
			list($year, $month, $day) = explode('-', $date);
			
			if (in_array((int)$day, $hollydays)) {
				
				$recheck_shit = true;
				while ($recheck_shit) {
					$new_date = $this->pick_random_month_day($holly_year, $holly_month);
					if ($this->is_first_date_bigger($new_date, $row['date'])) {
						$recheck_shit = false;
					}
				}
				
				$sql = "UPDATE `wp_loans` SET `pay_date`='$new_date' WHERE `id`=$id";
				//echo $sql.'<br>';
				mysql_query($sql);
				$n++;
			}
		}
	}
	echo "Исправлено заявок: $n";
	die;
}

/* Меняем айдишники контрактов
------------------------------------------------------------------------------*/
function set_fakes_contracts() {
	$arr1 = $this->get_loans_by_month(2016, 2, 'pay_date', "AND `fake` != 0", "ASC");
	$arr2 = $this->get_loans_by_month(2016, 3, 'pay_date', "AND `fake` != 0", "ASC");
	$arr = array_merge($arr1, $arr2);
	$contract = 2071;
	foreach ($arr as $id) {
		$sql = "UPDATE `wp_loans` SET `contract_id`='16-000$contract' WHERE `id` =$id AND `fake` != 0";
		//echo $sql.'<br>';
		mysql_query($sql);
		$contract++;
	}
	echo 'ok';
	die;
}

function fix_db_names() {
	$query = mysql_query("SELECT * FROM `wp_loans_users` ");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$row['sname'] = $this->prop_name_translate($row['sname']);
		$row['fname'] = $this->prop_name_translate($row['fname']);
		$row['tname'] = $this->prop_name_translate($row['tname']);
		$row['relsname'] = $this->prop_name_translate($row['relsname']);
		$row['relfname'] = $this->prop_name_translate($row['relfname']);
		$row['reltname'] = $this->prop_name_translate($row['reltname']);
		
		mysql_query("UPDATE `wp_loans_users` SET `sname`='{$row['sname']}', `fname`='{$row['fname']}', `tname`='{$row['tname']}', `relsname`='{$row['relsname']}', `relfname`='{$row['relfname']}', `reltname`='{$row['reltname']}' WHERE id = {$row['id']}");
	}
	echo 'ok';
	die;

}

function check_unique_fakes() {
	$query = mysql_query("SELECT `id` FROM `wp_loans` WHERE `fake`=0");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$query2 = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans` WHERE `fake`={$row['id']}");
		@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
		if ($row2['numb'] > 1) {
			echo $row['id'].'<br>';
		}
	}
	die;
}

function split_number_to_random_parts($amount, $parts, $min, $max) {
	$parts_tmp = $parts;
	$result = array();
	$sum = 0;
	
	while($parts_tmp) {
		$m = rand($min, $max);
		$m = $m / 1000;
		$m = round($m);
		$m = $m * 1000;
		
		$result[] = $m;
		$sum = $sum + $m;
		$parts_tmp--;
	}
	
	if ($amount > $sum) {
		$razn = $amount - $sum;
		$s = 0;
		while($razn>0) {
			$key = rand(0, ($parts-1));
			
			$m = rand($min, $max);
			$m = $m / 1000;
			$m = round($m);
			$m = $m * 1000;
			if ($result[$key] + $m > $max) {
				$m = $max - $result[$key];
			}
			if ($m > $razn) {
				$m = $razn;
			}
			
			$result[$key] = $result[$key] + $m;
			$razn = $razn - $m;
			
			$s++;
			if ($s > 100000) {
				echo 'Looped';
				break;
			}
		}
	}
	
	
	echo '<pre>'; var_export($result); echo '</pre>'; // отладка
	$sum = 0;
	foreach ($result as $a) {
		$sum = $sum + $a;
	}
	echo $amount - $sum;
	
	die;
}

function get_loans_by_month($P_year, $P_month, $date_field='date', $and='', $desc="DESC") {
	// Получаем первый день следующего месяца и последний день предыдущего
	$year_prev = $year_next = $P_year;
	$ids = array();
	
	$month_prev = $P_month - 1;
	$month_next = $P_month + 1;
	if ($month_prev == 0) {
		$month_prev = 12;
		$year_prev = $P_year - 1;
	}
	if ($month_next == 13) {
		$month_next = 1;
		$year_next = $P_year + 1;
	}
	
	if (strlen($month_prev) < 2) {
		$month_prev = '0'.$month_prev;
	}
	if (strlen($month_next) < 2) {
		$month_next = '0'.$month_next;
	}
	
	$date_prev = $year_prev.'-'.$month_prev.'-'.(cal_days_in_month(CAL_GREGORIAN, $month_prev, $year_prev));
	$date_next = $year_next.'-'.$month_next.'-01';
	
	if (strlen($P_month) < 2) {
		$P_month = '0'.$P_month;
	}
	$date_now = $P_year.'-'.$P_month;
	
	// Вытащить все заявки за текущий месяц, за 1 число следующего месяца, за последнее число предыдущего месяца
	$query = mysql_query("SELECT `id`, `$date_field` FROM `wp_loans`
						 WHERE (`$date_field` LIKE '$date_now%' OR `$date_field` LIKE '$date_next%' OR `$date_field` LIKE '$date_prev%')
						 $and
						 ORDER BY `$date_field` $desc");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		// Конвертируем всё в локальное время
		$date = $row[$date_field];
		$date = $this->utc_to_local($date, "Y-m-d");
		list($year, $month, $day) = explode('-', $date);
		
		// Фильтруем заявки не за этот месяц
		if ((int)$month != (int)$P_month) {
			continue;
		}
		
		$ids[] = $row['id'];
	}
	return $ids;
}

/* Проверка какая сумма займов за выбранные месяцы
 * $months - массив
------------------------------------------------------------------------------*/
private function check_loans_amount($year, $months, $return=false, $and='') {
	$amount = 0;
	foreach ($months as $month) {
		$arr = $this->get_loans_by_month($year, $month, 'pay_date', $and);
		$arr = implode(',', $arr);
		$query = mysql_query("SELECT * FROM `wp_loans` WHERE id IN($arr)");
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$amount = $amount + $row['amount'];
		}
	}
	if ($return) {
		return $amount;
	} else {
		echo $amount;
		die;
	}
	
}

/* Пакетная подкрутка сумм в фейках
 * $only_fakes - подгоняет сумму только по фейкам, игнорируя настоящие займы
------------------------------------------------------------------------------*/
private function adjust_fakes_amount($year, $month, $right_amount, $only_fakes=true) {
	$max_amount = 30000;
	$min_amount = 5000;
	
	if ($only_fakes) {
		$only_fakes = "AND `fake` != 0";
	} else {
		$only_fakes = "";
	}
	
	$loans = $this->get_loans_by_month($year, $month, 'pay_date', $only_fakes); // только фейки
	$amount_bac = $amount = $this->check_loans_amount($year, array($month), true, $only_fakes);
	if ($right_amount == $amount) {
		echo 'Уже нужная сумма!';
		die;
	}
	
	shuffle($loans);
	
	// how to algebra?
	if ($right_amount > $amount) {
		foreach ($loans as $random_loan) {
			$query = mysql_query("SELECT `amount` FROM `wp_loans` WHERE id=$random_loan");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			
			$amount = $amount + $max_amount - $row['amount'];
			mysql_query("UPDATE `wp_loans` SET `amount`=$max_amount WHERE id=$random_loan");
			
			if ($amount >= $right_amount) {
				echo 'Достаточно, завершаю...<br>';
				
				break;
			}
		}
		
		if ($amount > $right_amount) {
			$raznica = $amount - $right_amount;
			$row['amount'] = $max_amount - $raznica;
			$amount = $amount - $raznica;
			
			mysql_query("UPDATE `wp_loans` SET `amount`={$row['amount']} WHERE id=$random_loan");
		}
		
		if ($amount < $right_amount) {
			die('Недостаточно фейков для достижения нужной суммы, увеличьте $max_amount.');
		}
		
		
	} else {
		// если за месяц слишком много денег - прогони функцию два раза
		foreach ($loans as $random_loan) {
			$query = mysql_query("SELECT `amount` FROM `wp_loans` WHERE id=$random_loan");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			
			$amount = $amount - $row['amount'] - $min_amount;
			mysql_query("UPDATE `wp_loans` SET `amount`=$min_amount WHERE id=$random_loan");
			
			if ($amount <= $right_amount) {
				echo 'Достаточно, завершаю...<br>';
				
				break;
			}
		}
	}
	
	$amount_new = $this->check_loans_amount($year, array($month), true);
	
	echo 'Было - '.$amount_bac.'<br>
		  Стало- '.$amount_new.'<br>
		  Нужно- '.$right_amount;
	die;
}

function pick_random_month_day($year, $month) {
	$min_day = 1;
	$max_day = cal_days_in_month(CAL_GREGORIAN, $month, $year);
	
	$weekend = true;
	while($weekend) {
		$day = rand($min_day, $max_day);
		$day = str_pad($day, 2, '0', STR_PAD_LEFT);
		$month = str_pad($month, 2, '0', STR_PAD_LEFT);
		
		$pay_date = $year.'-'.$month.'-'.$day.' 12:00:00';
		
		$dw = explode("-", $pay_date);
		$dw = date("l", mktime(0, 0, 0, $dw[1], $dw[2], $dw[0]));
		if (($dw != 'Saturday') and ($dw != 'Sunday')) {
			$weekend = false;
		}
	}
	return $pay_date;
}

/* Пакетное создание фейков
 * ЭТУ ФУНКЦИЮ НУЖНО ПРОХОДИТЬ ТОЛЬКО НА БОЕВОМ, потому что на тестовом у меня проблемы с GMT (спасибо, Медведев)
 * 
 * $need_count - сколько договоров надо добавить (не учитывая существующие)
 * Ключи в $need_count означают номер месяца. Счёт идёт с 1, без ведущего 0.
 * 
 * В $min и $max - минимальное и максимальное число для каждого месяца, в которые можно создавать займы.
 * Все месяцы проходят в цикле, одним запуском функции
------------------------------------------------------------------------------*/
private function create_shit_load_of_fakes() {
	//mysql_query("DELETE FROM `wp_loans` WHERE fake != 0"); // для теста только на локалке
	//mysql_query("DELETE FROM `wp_loans_users` WHERE fake != 0"); // для теста только на локалке
	
	//$need_count[1] = 683; // январь
	//$need_count[2] = 753; // февраль
	//$need_count[3] = 747; // март
	//$need_count[4] = 765; // апрель
	//$need_count[5] = 694; // май
	//$need_count[6] = 547; // июнь
	
	//$need_count[7] = 748; // июль
	//$need_count[8] = 776; // август
	//$need_count[9] = 760; // сентябрь
	
	//$need_count[10] = 1120; // октябрь
	//$need_count[11] = 930; // ноябрь
	//$need_count[12] = 575; // декабрь
	
	$need_count[2] = 872; // февраль
	$need_count[3] = 995; // март
	
	foreach ($need_count as $mon => $trash) {
		$_POST['month'] = $mon;// с какого месяца начинать (понятия не имею почему я засунул это в POST)
		break;
	}
	$_POST['year'] = 2016;
	
	foreach ($need_count as $mon => $trash) {
		$min[$mon] = 1;
		$max[$mon] = cal_days_in_month(CAL_GREGORIAN, $mon, $_POST['year']);
		$need_count_keys[] = $mon;
	}
	
	// Здесь можно переназначить $min и $max для определённых месяцев, чтобы ограничить периуд создания фейков
	//$min[1] = 12;
	
	$bad_symbols = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
	$bad_symbols = str_split($bad_symbols);
	
	if ($_GET['mode'] == 'single') {
		return;
	}
	
	while(in_array($_POST['month'], $need_count_keys)) {
		echo '<h2 style="color:red;">Месяц #'.$_POST['month'].'</h2>';
		unset($m);
		
		// Получаем первый день следующего месяца и последний день предыдущего
		$year_prev = $year_next = $_POST['year'];
		$months = array($_POST['month']);
		$ids = array();
		
		$month_prev = $_POST['month'] - 1;
		$month_next = $_POST['month'] + 1;
		if ($month_prev == 0) {
			$month_prev = 12;
			$year_prev = $_POST['year'] - 1;
		}
		if ($month_next == 13) {
			$month_next = 1;
			$year_next = $_POST['year'] + 1;
		}
		
		if (strlen($month_prev) < 2) {
			$month_prev = '0'.$month_prev;
		}
		if (strlen($month_next) < 2) {
			$month_next = '0'.$month_next;
		}
		
		$date_prev = $year_prev.'-'.$month_prev.'-'.(cal_days_in_month(CAL_GREGORIAN, $month_prev, $year_prev));
		$date_next = $year_next.'-'.$month_next.'-01';
		
		if (strlen($_POST['month']) < 2) {
			$_POST['month'] = '0'.$_POST['month'];
		}
		$date_now = $_POST['year'].'-'.$_POST['month'];
		
		// Вытащить все заявки за текущий месяц, за 1 число следующего месяца, за последнее число предыдущего месяца
		$query = mysql_query("SELECT `id`, `date`, `user_id` FROM `wp_loans`
							 WHERE fake=0 AND `validate` IN(8,9)
							 AND (`date` LIKE '$date_now%' OR `date` LIKE '$date_next%' OR `date` LIKE '$date_prev%')
							 ORDER BY `date` DESC");
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			// Конвертируем всё в локальное время
			$date = $row['date'];
			$date = $this->utc_to_local($date, "Y-m-d");
			list($year, $month, $day) = explode('-', $date);
			
			// Фильтруем заявки не за этот месяц
			if ((int)$month != (int)$_POST['month']) {
				continue;
			}
			
			// проверка id заёмщика на уникальность среди фейков
			$query2 = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans_users` WHERE `fake` ={$row['user_id']}");
			@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
			if ($row2['numb']) {
				continue;
			}
			
			// проверка на латинницу и цифры в ФИО
			$query2 = mysql_query("SELECT `tname`, `fname`, `sname` FROM `wp_loans_users` WHERE id={$row['user_id']}");
			@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
			
			$test_loan = false;
			foreach ($bad_symbols as $symbol) {
				if (substr_count($row2['tname'], $symbol)) {
					$test_loan = true;
				}
				if (substr_count($row2['fname'], $symbol)) {
					$test_loan = true;
				}
				if (substr_count($row2['sname'], $symbol)) {
					$test_loan = true;
				}
			}
			if ($test_loan) {
				continue;
			}
			
			// проверка на существование фейка к этому займу
			$query2 = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans` WHERE `fake`={$row['id']}");
			$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
			if ($row2['numb']) {
				continue;
			}
			
			$ids[] = $row['id'];
		}
		
		// если заявок мало, прибавляем ещё
		$more_ids = array();
		if (count($ids) < $need_count[(int)$_POST['month']]) {
			$need_more = $need_count[(int)$_POST['month']] - count($ids);
			$n = 0;
			while ($need_more > 0) {
				$query = mysql_query("SELECT `id`, `date`, `user_id` FROM `wp_loans`
								 WHERE fake=0 AND `validate` IN(8,9)
								 AND DATE(`date`) < '2014-12-31 00:00:00'
								 ORDER BY `date` DESC LIMIT $n,1");
				@$row = mysql_fetch_array($query, MYSQL_ASSOC);
				$n++;
				
				// проверка id заёмщика на уникальность среди фейков
				$query2 = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans_users` WHERE `fake` ={$row['user_id']}");
				@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
				if ($row2['numb']) {
					continue;
				}
				
				// проверка на латинницу и цифры в ФИО
				$query2 = mysql_query("SELECT `tname`, `fname`, `sname` FROM `wp_loans_users` WHERE id={$row['user_id']}");
				@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
				
				$test_loan = false;
				foreach ($bad_symbols as $symbol) {
					if (substr_count($row2['tname'], $symbol)) {
						$test_loan = true;
					}
					if (substr_count($row2['fname'], $symbol)) {
						$test_loan = true;
					}
					if (substr_count($row2['sname'], $symbol)) {
						$test_loan = true;
					}
				}
				if ($test_loan) {
					continue;
				}
				
				// проверка на существование фейка к этому займу
				$query2 = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans` WHERE `fake`={$row['id']}");
				$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
				if ($row2['numb']) {
					continue;
				}
				
				$more_ids[] = $row['id'];
				$need_more--;
			}
		}
		
		$ids = array_merge($ids, $more_ids);
		$ids = array_unique($ids);
		
		// если заявок много
		if (count($ids) > $need_count[(int)$_POST['month']]) {
			$ids_of_ids = array_rand($ids, $need_count[(int)$_POST['month']]);
			$ids_bac = $ids;
			$ids = array();
			foreach ($ids_of_ids as $i) {
				$ids[] = $ids_bac[$i];
			}
		}
		
		// финальная проверка количества заявок
		if (count($ids) != $need_count[(int)$_POST['month']]) {
			echo 'Не хватает заявок';
			die;
		}
		
		// проверки закончились, составляем список заявок для вставки
		// выбираем даты выдачи рандомно
		$fakes = array();
		$days = array();
		$n = 0;
		foreach ($ids as $loan_id) {
			$query = mysql_query("SELECT `date` FROM `wp_loans` WHERE id={$loan_id}");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			
			$weekend = true;
			while($weekend) {
				$pay_date = $this->utc_to_local($row['date']);
				list($date, $time) = explode(' ', $pay_date);
				$day = rand($min[(int)$_POST['month']], $max[(int)$_POST['month']]);
				$day = str_pad($day, 2, '0', STR_PAD_LEFT);
				$pay_date = $_POST['year'].'-'.$_POST['month'].'-'.$day.' '.$time;
				
				$dw = explode("-", $pay_date);
				$dw = date("l", mktime(0, 0, 0, $dw[1], $dw[2], $dw[0]));
				if (($dw != 'Saturday') and ($dw != 'Sunday')) {
					$weekend = false;
				}
			}
			
			$fakes[$n]['days'] = $pay_date;
			$fakes[$n]['loan_id'] = $loan_id;
			$days[$n] = $pay_date;
			$n++;
		}
		
		// сортируем чтобы даты были по порядку, как и номера договоров
		array_multisort($days, SORT_ASC, $fakes);
		
		foreach ($fakes as $arr) {
			$loan_id = $arr['loan_id'];
			$pay_date = $arr['days'];
			
			echo 'Копирую займ #'.$loan_id.'...<br>';
			$fake = $this->create_fake($loan_id);
			
			$fakes[] = $fake;
			
			// Выдача займа
			$row = $this->get_main_row($fake);
			
			// Проверяем номер карты, тариф и способ выплаты
			if (!$row['card']) {
				$card = $this->rand_string(16, true);
				mysql_query("UPDATE `wp_loans` SET `card`='$card' WHERE id=$fake");
			}
			if (!$row['tariff']) {
				mysql_query("UPDATE `wp_loans` SET `tariff`=11 WHERE id=$fake");
			}
			if (!$row['loan_method']) {
				mysql_query("UPDATE `wp_loans` SET `loan_method`=1 WHERE id=$fake");
			}
			
			// Получаем текущий год
			$year = substr($_POST['year'], 2, 2);
			$year2 = $_POST['year'];
			
			// Вытаскиваем ID последнего договора в этом году
			$query_cs = mysql_query("SELECT `contract_id` FROM `wp_loans` WHERE YEAR(`pay_date`)=$year2 ORDER BY `contract_id` DESC LIMIT 1");
			if ($query_cs) {
				$row_cs = mysql_fetch_array($query_cs, MYSQL_ASSOC);
			}
			
			// В 2014 номера начинаем с 0001341, далее с 0001340
			if (!$row_cs['contract_id']) {
				if ($year2 == 2014) {
					$contract = '0001341';
				} else {
					$contract = '0001340';
				}
				
				$row_cs['contract_id'] = $year.'-'.$contract;
			} else {
				list($year_trash, $contract) = explode('-', $row_cs['contract_id']);
				$contract = preg_replace("/[^0-9]/", '', $contract); // предохранитель от неведомой фигни
				if (!$contract) {
					$this->wrap_json_error('Произошла немыслимая ошибка, дальнейшее заключение договоров невозможно. Срочно свяжитесь с программистом!');
				}
				$contract++;
				$contract = str_pad($contract, 7, '0', STR_PAD_LEFT);
				$row_cs['contract_id'] = $year.'-'.$contract;
			}
			
			$pay_date = $this->local_to_utc($pay_date);
			
			$date = date_create($pay_date);
			date_add($date, date_interval_create_from_date_string((0-(rand(1,4))).' days'));
			$date = date_format($date, 'Y-m-d H:i:s');
			
			$back_date = date_create($pay_date);
			date_add($back_date, date_interval_create_from_date_string(($row['term']-1).' days'));
			$back_date = date_format($back_date, 'Y-m-d H:i:s');
			
			mysql_query("UPDATE `wp_loans` SET `validate`=5, `contract_id`='{$row_cs['contract_id']}', `date`='$date', `pay_date`='$pay_date', `back_date`='$back_date' WHERE id=$fake");
			mysql_query("UPDATE `wp_loans_users` SET `blocked` = '0000-00-00 00:00:00' WHERE id={$row['user_id']}");
			
			$m++;
			echo $m.' - Фейк <a target="_blank" style="color:black;" href="https://moneyfunny.ru/sb_cabinet/?mode=single&id='.$fake.'">#'.$fake.'</a> - '.$pay_date.' - '.$dw.'<br>';
		}
		
		
		echo 'Месяц пройден успешно!<br><br><br><br>';
		$_POST['month'] = (int)$_POST['month'];
		$_POST['month']++;
	}
	die;
}




private function fix_contracts() {
	$query = mysql_query("SELECT `contract_id`, `id` FROM `wp_loans` WHERE `contract_id` != ''");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$row['contract_id'] = str_replace('-БН', '', $row['contract_id']);
		list($contact, $year) = explode('/', $row['contract_id']);
		$contact = $year.'-'.$contact;
		mysql_query("UPDATE `wp_loans` SET `contract_id`='$contact' WHERE id={$row['id']}");
	}
	
	mysql_query("DELETE FROM `wp_loans` WHERE `id`=8332");
}

/* Использовалось для переезда с http на https, но можно использовать для чего угодно
------------------------------------------------------------------------------*/
private function find_and_replace_in_db() {
	$find = 'http://moneyfun';
	$replace = 'https://moneyfun';
	
	$query = mysql_query("SHOW TABLES FROM mragi_0");
	while ($row = mysql_fetch_array($query)) {
		unset($pri);
		$query3 = mysql_query("DESCRIBE `{$row[0]}`");
		while ($row3 = mysql_fetch_array($query3, MYSQL_ASSOC)) {
			if (($row3['Key']=='PRI') and ($row3['Extra']=='auto_increment')) {
				$pri = $row3['Field'];
				continue;
			}
		}
		
		if (!$pri) {
			continue;
		}
		
		$query2 = mysql_query("SELECT * FROM `{$row[0]}`");
		while ($row2 = mysql_fetch_array($query2, MYSQL_ASSOC)) {
			foreach ($row2 as $key => $value) {
				if (substr_count($value, $find)) {
					
					$arr = unserialize($value);
					
					if ($arr != false) {
						foreach ($arr as $k => $v) {
							$arr[$k] = $v = str_replace($find, $replace, $v);
						}
						$value = serialize($arr);
					} else {
						$value = str_replace($find, $replace, $value);
					}
					$value = mysql_real_escape_string($value);
					mysql_query("UPDATE `{$row[0]}` SET `$key`='$value' WHERE `$pri`='{$row2[$pri]}'");
				}
			}
		}
	}
	echo 'ok';
	die;
}

/* Если нужно перенарезать тумбы
------------------------------------------------------------------------------*/
private function recreate_thumbs($field, $thumb_name, $page=1) {
	$per_page = 50;
	
	$query = mysql_query("SELECT COUNT(`id`) AS `numb` FROM `wp_loans_users` WHERE `$field` != '' AND `$field` != 'a:0:{}'");
	$row = mysql_fetch_array($query, MYSQL_ASSOC);
	$num_pages=ceil($row['numb']/$per_page);
	$limit = $per_page*($page-1).', '.$per_page;
	echo 'Страница: '.$page.' из '.$num_pages.'<br>';
	
	switch ($thumb_name) {
		case 'thumb':
			$width = 100;
			$height = 100;
		break;
		case 'thumb2':
			$width = 185;
			$height = 237;
		break;
	}
	
	$_POST['ajaxing'] = 'recreate';
	require_once($this->site_dir.'wp-content/plugins/mfunny-cabs/upload.php');
	$_T = new mfunny_uplod_funcs();
	$dir = $this->site_dir.'wp-content/uploads/photos/';
	
	//echo "<br>Запрос: SELECT `$field` FROM `wp_loans_users` ORDER BY `id` LIMIT $limit";
	$query = mysql_query("SELECT `$field` FROM `wp_loans_users` WHERE `$field` != '' AND `$field` != 'a:0:{}' ORDER BY `id` LIMIT $limit");
	$n = $m = 0;
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$n++;
		if (!$row[$field]) {
			continue;
		}
		$row[$field] = unserialize($row[$field]);
		if (!count($row[$field])) {
			continue;
		}

		setlocale(LC_ALL, 'ru_RU.utf8');
		foreach ($row[$field] as $key => $photo) {
			$tmp = pathinfo($photo);
			
			if (!in_array(strtolower($tmp['extension']), array('jpg', 'jpeg', 'png', 'gif', 'bmp'))) {
				continue;
			}

			@unlink($dir.$tmp['filename'].'-'.$thumb_name.'.'.$tmp['extension']);
			@unlink($dir.$thumb_name.'/'.$tmp['filename'].'-'.$thumb_name.'.'.$tmp['extension']);
			
			if (!file_exists($dir.$photo)) {
				continue;
			}
			
			$_T->create_thumbnail($dir.$photo, $width, $height, $dir.$thumb_name.'/'.$tmp['filename'].'-'.$thumb_name.'.'.$tmp['extension'], true);
			$m++;
		}
	}
	echo '<br> Пройдено займов : '.$n.' - Из нх нарезано изображений: '.$m;
}

/* Скрипт для проверки уникальности паспортных данных
------------------------------------------------------------------------------*/
private function check_unique_pasp() {
	$arr = array();
	$query = mysql_query("SELECT `id`, `paspser`, `paspnom` FROM `wp_loans_users` WHERE `fake`=0");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$key = $row['paspser'].'-'.$row['paspnom'];
		if (($key != '-') and ($key != '0-0')) {
			$arr[$key][] = $row['id'];
		}
	}
	
	foreach ($arr as $contract => $ids) {
		if (count($ids) == 1) {
			unset($arr[$contract]);
		}
	}
	echo '<pre>'; var_export($arr); echo '</pre>';  //отладка
	die;

}

/* Скрипт для проверки уникальности мыла
------------------------------------------------------------------------------*/
private function check_unique_email() {
	$arr = array();
	$query = mysql_query("SELECT `id`, `email` FROM `wp_loans_users` WHERE `fake`=0");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$arr[strtolower($row['email'])][] = $row['id'];
	}
	
	foreach ($arr as $contract => $ids) {
		if (count($ids) == 1) {
			unset($arr[$contract]);
		}
	}
	
	echo '<pre>'; var_export($arr); echo '</pre>';  //отладка
	die;

}

/* Скрипт для проверки уникальности номеров договоров - раскоментить, когда всё плохо
------------------------------------------------------------------------------*/
private function check_unique_contract_id() {
	$arr = array();
	$query = mysql_query("SELECT `id`, `contract_id` FROM `wp_loans` WHERE fake=0 AND `contract_id` != '' ORDER BY `contract_id` DESC");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$arr[$row['contract_id']][] = $row['id'];
	}
	
	foreach ($arr as $contract => $ids) {
		if (count($ids) == 1) {
			unset($arr[$contract]);
		}
	}
	echo '<pre>'; var_export($arr); echo '</pre>';  //отладка
	die;

}

function cURL($url, $post, $ref=''){
	if (!$ref) {
		$ref = $this->site_url;
	}
	$ch =  curl_init();
	curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 0);    
	curl_setopt($ch, CURLOPT_USERAGENT, $_SERVER['HTTP_USER_AGENT']);
	curl_setopt($ch, CURLOPT_REFERER, $ref);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);    
	curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, 0);    
	if ($post) {
		curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
		curl_setopt($ch, CURLOPT_POST, 1);
		curl_setopt($ch, CURLOPT_POSTFIELDS, $post);
	}
	$result = curl_exec($ch);
	curl_close($ch);
	return $result;
}

function send_post_arr_to_server($url, $postdata) {
	$postdata = http_build_query($postdata);
	$opts = array('http' =>
		array(
			'method'  => 'POST',
			'header'  => 'Content-type: application/x-www-form-urlencoded',
			'content' => $postdata
		)
	);
	
	$context  = stream_context_create($opts);
	$result = file_get_contents($url, false, $context);
	return $result;
}

function cabs_tpl($nav, $content, $mode='') {
	switch ($mode) {
		case 'new':
			if (!$_COOKIE['disable-hint1']) {
				$info = '<div class="info-box">На эту страницу попадают заявки, сразу же после оформления на сайте.<span id="hint1" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'counter':
			if (!$_COOKIE['disable-hint2']) {
				$info = '<div class="info-box">На этой странице отображатся заявки после обработки менеджером.<span id="hint2" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'single':
			if (!$_COOKIE['disable-hint3']) {
				$info = '<div class="info-box info-box-single">Каждая заявка проверяется в два этапа: 1. Информация от заемщика;<br>2. Дополненная информация от менеджера.<span id="hint3" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'manage':
			if (!$_COOKIE['disable-hint4']) {
				$info = '<div class="info-box">Сотрудник СБ проверил доступную информацию в этих заявках и прислал их вам для дополнения.<span id="hint4" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'contract':
			if (!$_COOKIE['disable-hint5']) {
				$info = '<div class="info-box">Эти займы прошли контрольную проверку в СБ и ожидают оформления.<span id="hint5" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'pay':
			if (!$_COOKIE['disable-hint6']) {
				$info = '<div class="info-box">На этой странице отображается список одобренных займов, ожидающих выплаты.<span id="hint6" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'success':
			if (!$_COOKIE['disable-hint7']) {
				$info = '<div class="info-box">Это архив выданных и успешно возвращенных займов.<span id="hint7" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'exceed':
			if (!$_COOKIE['disable-hint8']) {
				$info = '<div class="info-box">Это список неплательщиков, просрочивших крайний срок выплат.<span id="hint8" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'debts':
			if (!$_COOKIE['disable-hint9']) {
				$info = '<div class="info-box">Здесь отображаются выданные займы, ещё не просрочившие крайний срок выплат.<span id="hint9" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'egosearch':
			if (!$_COOKIE['disable-hint10']) {
				$info = '<div class="info-box">Здесь отображаются замы, привязанные к вам по любой из доступных ролей.<span id="hint10" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'nbki':
			if (!$_COOKIE['disable-hint11']) {
				$info = '<div class="info-box">Список заявок, отложенных на проверку в НБКИ.<span id="hint11" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'expired':
			if (!$_COOKIE['disable-hint12']) {
				$info = '<div class="info-box">Эти займы требуют обзвона.<span id="hint12" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'collect':
			if (!$_COOKIE['disable-hint13']) {
				$info = '<div class="info-box">Здесь отображается список займов, отправленных бухгалтером на взыскание.<span id="hint13" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'collected':
			if (!$_COOKIE['disable-hint14']) {
				$info = '<div class="info-box">Эти займы были обработаны коллектором. Вы можете перенести их в выданные, либо возвращенные, либо повторно отправить коллектору.<span id="hint14" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'need_call':
		case 'need_call_all':
			if (!$_COOKIE['disable-hint15']) {
				$info = '<div class="info-box">Этим заемщикам надо позвонить.<span id="hint15" class="jsCloseBtn"></span></div>';
			}
		break;
		case 'payments':
			if (!$_COOKIE['disable-hint16']) {
				$info = '<div class="info-box">На этой странице отображаются автоматические платежи от заёмщиков.<span id="hint16" class="jsCloseBtn"></span></div>';
			}
		break;
	}
	

	if (current_user_can('loan_view_all')) {
		$showAllState = '';
		if ($_COOKIE['show_all']) {
			$showAllState = ' checked';
		}
		
		$admin_tools.='
		<div class="show-all-wrap">
			<input type="checkbox" class="checkbox jsShowAll" id="show-all" '.$showAllState.'>
			<label for="show-all">Отображать заявки всех сотрудников</label>
		</div>
		';
	}
	if ($this->is_admin) {
		$content.='<div class="jsAdminHolder"></div>';
	}
	
	if (($this->is_admin) and ($_COOKIE['show_admin_hidden'])) {
		$main_wrap_class = 'kahfjkaF-on';
	} else {
		$main_wrap_class = '';
	}
	
	echo '
	<div id="wrapper" class="'.$main_wrap_class.'">
		<header id="header">
			<div class="header-top">
				<div class="employer-wrap">
				<h3 class="employer">Сотрудник: '.$this->get_staff_name($this->user_id).'</h3>
				</div>
				'.$info.'
				<a href="'.$this->site_url.'" class="site-url"><img src="'.$this->site_url.'wp-content/uploads/2012/09/MFlogotr.png" alt="На сайт"></a>
				<a href="'.$this->site_url.'sb_cabinet" class="slogan">Система управления займами</a>
				'.$admin_tools.'
			</div>
			'.$nav.'
		</header>
			
		<section id="middle">
		
			<div id="container">
				<div id="content">
					'.$content.'
					<div id="jsModeHolder" style="display:none;">'.$_GET['mode'].'</div>
				</div>
			</div>
		</section>
		
		<footer>
			<span class="sf-label typ-btn">Я нашел ошибку</span>
			<div class="send-feedback">
				<p class="info-box-static">Эта форма предназначена для оперативной связи с программистом. Просьба сообщать только об ошибках <u>технического</u> характера.</p>
				<p>Опишите проблему в деталях:</p>
				<textarea name="f_message" class="f_message"></textarea>
				<input readonly type="text" name="f_sender" value="'.$this->user_id.'">
				<textarea readonly name="f_post">'.$this->form_text(var_export($_POST, true), 'textarea').'</textarea>
				<textarea readonly name="f_get">'.$this->form_text(var_export($_GET, true), 'textarea').'</textarea>
				
				<div class="centered"><a href="#" class="typ-btn jsSendFeedback">Отправить</a></div>
			</div>
		</footer>
	</div>';
}

function error_tpl($capture='Ошибка', $content='У вас нет доступа к этой странице.', $class='error') {
	switch ($_SERVER['REDIRECT_URL']) {
		default:
			$second_link = '';
		break;
		case '/sb_cabinet/':
			$second_link = '<a href="'.$this->site_url.'sb_cabinet">← К системе управления займами</a><br>';
		break;
		case '/stat_admin/':
			$second_link = '<a href="'.$this->site_url.'stat_admin">← К статистике микрозаймов</a><br>';
		break;
		case '/creditor_cabs/':
			$second_link = '<a href="'.$this->site_url.'creditor_cabs">← В кабинет заемщика</a><br>';
		break;
	}
	
	if ($class == 'error') {
		die('
			<div class="error-main-wrapper '.$class.'">
				<h1><a href="'.$this->site_url.'"></a></h1>
				<div class="error-wrap">
					<div class="error-icon"></div>
					<div class="error-title">'.$capture.'</div>
					<div class="error-content">
						'.$content.'
						<p id="backtoblog">
							<a href="#" onclick="history.go(-1);return false;">← Назад</a><br>
							'.$second_link.'
							<a href="'.$this->site_url.'" title="Потерялись?">← К сайту «Мани Фанни»</a>
						</p>
					</div>
					<div class="clear"></div>
				</div>
			</div>
			<div class="center-pseudo"></div>
			</body>
		</html>');
	} else {
		die('<div id="wrapper">
				<header id="header" class="error">
					<h1>'.$capture.'</h1>
				</header>
					
				<section id="middle">
			
					<div id="container">
						<div id="content" class="'.$class.'">
							'.$content.'
						</div>
					</div>
					
				</section>
			</div>
			<div class="center-pseudo"></div>
			</body>
		</html>');
	}
}

function form_text($text, $mode='output') {
if (is_array($text)) {
	return $text;
}
	
// формирует текст под разные ситуации
switch ($mode){
	case 'output':
        // текст из бд -> вывод на страницу
		$text = stripslashes($text);
		$text = htmlspecialchars($text);
		$text = nl2br($text, false);
	break;

	case 'textarea':
        // текст из бд -> поле ввода
        $text = stripslashes($text);
		$text = str_replace('textarea', '[LKAHJFoifh1oihr1okrjlklaFJLAKJIOJ14jo1oirj14jKLJFk2jflk]', $text); // костыль от поломок, обратно преобразуется через JS
	break;
    
    case 'db':
        // текст из POST -> запрос в db
        $text = trim($text);
        $text = addslashes($text);
        $text = mysql_real_escape_string($text);
    break;
	case 'mobile':
		// вывод для нативных приложений
		$text = strip_tags($text);
		$text = str_replace('&nbsp;', '', $text);
		$text = preg_replace("/(\r?\n){2,}/", "\r\n\r\n", $text);
		
		$text = trim($text);
	break;
}
return $text;
}

function rand_string($length, $numb_only=false) {
	if ($numb_only) {
		$characters = '0123456789';
	} else {
		$characters = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
	}
    $randomString = '';
    for ($i = 0; $i < $length; $i++) {
        $randomString .= $characters[rand(0, strlen($characters) - 1)];
    }
    return $randomString;
}

// Ключи из базы - на язык человеков
function loans_translate($text) {
	$translate = array (
		'remind' => 'Напомнить об истечении',
		'purpose' => 'Цель займа',
		'sms_off' => 'SMS-оповещения',
		'nbki_hide' => 'Передача в НБКИ',
		'corp1' => 'Корпус <span class="text-gray text-small">(не обязательно)</span>',
		'corp2' => 'Корпус <span class="text-gray text-small">(не обязательно)</span>',
		'structure1' => 'Строение <span class="text-gray text-small">(не обязательно)</span>',
		'structure2' => 'Строение <span class="text-gray text-small">(не обязательно)</span>',
		'back_date_real' => 'Дата закрытия',
		'force_percent' => 'Форсированный процент <p class="text-red text-small">При выставлении этого процента, процент в тарифе будет игнорироваться.</p>',
		'idea' => 'Идея',
		'loan_method' => 'Способ выдачи',
		'payment_date' => 'Дата платежа',
		'pay_method' => 'Способ оплаты',
		'need_call_date' => 'Надо позвонить',
		'score' => 'Рейтинг заемщика',
		'stat_now_amount_full' => 'Сумма с процентами',
		'stat_now_amount' => 'Тело займа',
		'stat_now_percent_sum' => 'Набежавшие проценты (в %)',
		'stat_now_percent_rub_sum' => 'Набежавшие проценты (в рублях)',
		'percent' => 'Ставка',
		'blocked' => 'В чёрном списке',
		'sb_comment1' => 'Комментарий службы безопасности #1',
		'sb_comment2' => 'Комментарий службы безопасности #2',
		'buh_comment' => 'Комментарий бухгалтера',
		'branch' => 'Филиал заемщика',
		'back_date' => 'Крайний день возврата',
		'manager_calls2_comment' => '<span class="text-orange manager-comment3-capture">Как прошел звонок:</span>',
		'manager_calls2' => 'Заемщик приглашен в офис',
		'manager_calls' =>'Заемщик оповещен об истечении',
		'photo' => 'Фото',
		'images' => 'Другие изображения',
		'last_doc' => '<img src="'.$this->site_url.'wp-content/themes/pixelpress/images/599.png" alt="Вложение">',
		'relsname' => 'Фамилия',
		'relfname' => 'Имя',
		'reltname' => 'Отчество',
		'relphone' => 'Телефон',
		'relative' => 'Степень родства',
		'pay_date_long' => 'Дата заключения договора <p class="text-red text-small">Внимательно проверяйте это поле перед распечаткой договора, от него считается крайняя дата погашения займа.</p>',
		'pay_date' => 'Дата заключения договора',
		'card' => 'Номер банковской карты',
		'tariff' => 'Тариф',
		'contract_id' => 'Договор №',
		'sb_id' => 'Обрабатывается сотрудником СБ',
		'manager_id' => 'Обрабатывается менеджером',
		'buh_id' => 'Обрабатывается бухгалтером',
		'collector_id' => 'Обрабатывается коллектором',
		'validate' => 'Статус займа',
		'id' => 'ID займа',
		'amount_full' => 'Сумма',
		'amount' => 'Сумма',
		'term_long' => 'На срок',
		'term' => 'На срок',
		'earnings' => 'Доход в месяц',
		'fname' => 'Имя',
		'sname' => 'Фамилия',
		'birthaddr' => 'Место рождения',
		'birth' => 'Дата рождения',
		'tname' => 'Отчество',
		'sex' => 'Пол',
		'paspser' => 'Серия',
		'paspnom' => 'Номер',
		'fulladdr_old' => 'Адрес прописки (старый формат)',
		'fulladdr' => 'Адрес прописки',
		'address_same' => 'По месту прописки',
		'address_old' => 'Адрес проживания (старый формат)',
		'address' => 'Адрес проживания',
		'paspkem' => 'Кем выдан',
		'paspdate' => 'Дата выдачи',
		'seconddoc' => 'Второй документ',
		'phone1' => 'Телефон 1',
		'phone2' => 'Телефон 2',
		'phones' => 'Телефоны',
		'phone' => 'Телефон',
		'email' => 'E-Mail',
		'date' => 'Дата подачи заявки',
		'actions' => 'Действия',
		'full_name' => 'Ф.И.О.',
		'loans_count' => 'Подано заявок',
		'street1' => 'Название улицы',
		'street2' => 'Название улицы',
		'city1' => 'Город',
		'city2' => 'Город',
		'pasp_location' => 'Место выдачи паспорта <span class="text-gray text-small">(достаточно города)</span>',
		'house_number1' => 'Дом',
		'apartment1' => 'Квартира',
		'house_number2' => 'Дом',
		'apartment2' => 'Квартира',
		'street_type1' => 'Тип улицы',
		'street_type2' => 'Тип улицы',
		'propiska' => 'Прописка'
	);
	return str_ireplace(array_keys($translate), array_values($translate), $text);
}

function loans_translate_4list($text) {
	$translate = array (
		'back_date_real' => 'Дата закрытия',
		'expired_days' => 'Просрочка',
		'relative' => 'Родственник',
		'blocked' => 'Дата блокировки',
		'last_comment4_full' => 'Последний общий коммент',
		'last_comment4' => 'Последний общий коммент',
		'back_date_real_tiny' => 'Дата погашения',
		'back_date' => 'Крайний день возврата',
		'pay_date' => 'Дата выдачи',
		'sb_id' => 'СБ',
		'manager_id' => 'Менеджер',
		'buh_id' => 'Бухгалтер',
		'collector_id' => 'Коллектор',
		'term' => 'Срок'
	);
	$text = str_ireplace(array_keys($translate), array_values($translate), $text);
	return $this->loans_translate($text);
}


// Значения из базы - на язык человеков
function loans_translate_vals($arr) {
	
	if (!is_array($arr)) {
		return false;
	}
	
	foreach ($arr as $key => $value) {
		if ($key=='real') {
			continue;
		}
	
	    switch ($key) {
			default:
				$arr[$key] = $this->form_text($arr[$key]);
			break;
			case 'contract_id':
				if ($arr[$key]) {
					list($year, $contact) = explode('-', $arr[$key]);
					$arr[$key] = $contact.'/'.$year.'-БН';
				}
			break;
			case 'loan_method':
				switch ($arr[$key]) {
					default:
						$arr[$key] = '';
					break;
					case 1:
						$arr[$key] = 'Наличные';
					break;
					case 2:
						$arr[$key] = 'Безналичные';
					break;
					case 3:
						$arr[$key] = 'На карту';
					break;
				}
			break;
			case 'pay_validate':
				switch ($arr[$key]) {
					case 0:
						$arr[$key] = '<span class="text-orange">Пользователь не перешёл на сайт Робокассы.</span>';
					break;
					case 5:
						$arr[$key] = '<span class="text-orange">Операция только инициализирована, деньги от покупателя ещё не получены.</span>';
					break;
					case 10:
						$arr[$key] = '<span class="text-red">Операция отменена, деньги от покупателя не были получены.</span>';
					break;
					case 50:
						$arr[$key] = '<span class="text-green">Деньги от покупателя получены, производится зачисление денег на счет магазина.</span>';
					break;
					case 60:
						$arr[$key] = '<span class="text-red">Деньги после получения были возвращены покупателю.</span>';
					break;
					case 80:
						$arr[$key] = '<span class="text-orange">Исполнение операции приостановлено Робокассой. Платёж будет обработан в ручном режиме.</span>';
					break;
					case 100:
						$arr[$key] = '<span class="text-green">Операция выполнена, завершена успешно.</span>';
					break;
				}
			break;
			case 'sb_comment1':
			case 'sb_comment1':
				// do nothing
			break;
			case 'term':
				$arr[$key] = $this->term_to_years($arr[$key]);
			break;
			case 'term_long':
				$arr[$key] = $this->term_to_years($arr[$key]);
				// продления
				$n=1;
				$query = mysql_query("SELECT * FROM `wp_loans_term_adds` WHERE loan_id={$arr['id']}");
				while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
					$arr[$key] .= '<span class="text-orange"> + <a href="'.$this->site_url.'print-contract?id='.$arr['id'].'&doc=5&v='.$row['doc_numb'].'" target="_blank">'.$row['term'].' дн.</a></span>';
					$n++;
				}
				if ($n>1) {
					$arr[$key] .=' <span class="jsDeleteTerm fancy-tip no-selection" title="Удалить"></span>';
				}
			break;
			case 'amount':
				if ($arr[$key]) {
					$arr['amount_i'] = $this->num2str($arr[$key]);
					$arr[$key] .= ' р.';
				}
			break;
			case 'percent':
				if ($arr[$key]) {
					$arr[$key] = (double)$arr[$key];
					$arr[$key] .= '% в день';
				} else {
					$arr[$key] = 'NaN';
				}
			break;
			case 'earnings':
				if ($arr[$key]) {
					$arr[$key] .= ' р.';
				}
			break;
			case 'sex':
				if ($value == 0) {
					$arr[$key] = '';
				}
				if ($value == 1) {
					$arr[$key] = 'Женский';
				}
				if ($value == 2) {
					$arr[$key] = 'Мужской';
				}
			break;
			case 'fname':
					$arr['full_name'] = $arr['sname'].' '.$value.' '.$arr['tname'];
			break;
			case 'pay_date':
					if ($arr[$key]) {
						$arr[$key] = $this->get_time($arr[$key], 'compact', true).$back_date;
					}
			break;
			case 'payment_date':
				if ($arr[$key]) {
					if ($arr['user'] == 'user') {
						$arr[$key] = $this->get_time($arr[$key], 'full', true);
						$arr['ass_numb'] = '-';
					} else {
						$arr[$key] = $this->get_time($arr[$key], 'compact', true);
					}
				}
			break;
			case 'pay_date_long':
				if ($arr[$key] != '0000-00-00 00:00:00') {
					$arr[$key] = $this->get_time($arr[$key], 'compact', true).$back_date;
					if ($arr['real']['back_date'] != '0000-00-00 00:00:00') {
						$arr[$key] .= ' (крайний день возврата: '. $this->get_time($arr['real']['back_date'], 'compact', true).')';
					}
				} else {
					$arr[$key] = '';
				}
			break;
			case 'date':
			case 'edit_date':
		
					if ($arr[$key] != '0000-00-00 00:00:00') {
						$arr[$key] = $this->get_time($arr[$key], 'full', true);
					} else {
						$arr[$key] = '';
					}
				
			break;
			case 'need_call_date':
			if ($arr[$key] != '0000-00-00 00:00:00') {
					$arr[$key] = $this->get_time($arr[$key], 'full');
				} else {
					$arr[$key] = '';
				}
			break;
			case 'date_stats':
			case 'back_date':
			case 'back_date_real':
				if ($arr[$key] != '0000-00-00 00:00:00') {
					$arr[$key] = $this->get_time($arr[$key], 'compact', true);
				} else {
					$arr[$key] = '';
				}
			break;
			case 'back_date_real_tiny':
				if ($arr[$key] != '0000-00-00 00:00:00') {
					$arr[$key] = $this->get_time($arr[$key], 'tiny_dig', true);
				} else {
					$arr[$key] = '';
				}
			break;
			case 'date_stats':
			case 'back_date':
			case 'birth':
			case 'paspdate':
				if ($arr[$key] > 0) {
					$arr[$key] = $this->get_time($arr[$key], 'compact');
				} else {
					$arr[$key] = '';
				}
			break;
			case 'comment_date':
				if ($arr[$key] > 0) {
					$arr[$key] = $this->get_time($arr[$key], 'full_dig', true);
				} else {
					$arr[$key] = '';
				}
			break;
			break;
			case 'propiska':
				switch ($value) {
					case 1:
						$arr[$key] = 'Москва';
					break;
					case 2:
						$arr[$key] = 'Московская область';
					break;
					case 3:
						$arr[$key] = 'Вне Москвы и Московской области';
					break;
				}
				if ($arr['city1']) {
					$arr[$key] = $arr['city1'];
				}
			break;
			case 'address_same':
				$checked = '';
				$label = '<span class="text-red">Нет</span>';
				if ($arr[$key] == 1) {
					$checked = 'checked';
					$label = '<span class="text-green">Да</span>';
					unset($arr['city2']);
					unset($arr['street_type2']);
					unset($arr['street2']);
					unset($arr['house_number2']);
					unset($arr['structure2']);
					unset($arr['apartment2']);
					unset($arr['corp2']);
				}
				$arr[$key] = '<input type="checkbox" id="jsSameAddress" class="checkbox" '.$checked.'> <label for="jsSameAddress">'.$label.'</label>';
			break;
			case 'address_old':
				if ($value == 1) {
					$arr[$key] = 'По месту прописки';
				} else {
					$arr[$key] = $arr['vat'];
				}
				
				if (($arr['city2']) or ($arr['street_type2']) or ($arr['street2']) or ($arr['house_number2']) or ($arr['structure2']) or ($arr['apartment2']) or ($arr['corp2'])) {
					$arr[$key] = 0;
				}
				
				if (($value == 1) and (($arr['city1']) or ($arr['street_type1']) or ($arr['street1']) or ($arr['house_number1']) or ($arr['structure1']) or ($arr['apartment1']) or ($arr['corp1']))) {
					$arr[$key] = 0;
				}
				
			break;
			case 'address':
				if ($value == 1) {
					$arr[$key] = 'По месту прописки';
				} else {
					$addr = array();
					if ($arr['city2']) {
						$addr[]= $arr['city2'];
					}
					if ($arr['street_type2']) {
						if (is_numeric(trim($arr['street_type2']))) {
							$row = array('street_type2' => $arr['street_type2']);
							$row = $this->loans_translate_vals($row);
							$arr['street_type2'] = $row['street_type2'];
						}
						$street = $arr['street_type2'];
						if ($arr['street2']) {
							$street .= ' '.$arr['street2'];
						}
						$addr[]= $street;
					}
					if ($arr['house_number2']) {
						$addr[]= 'дом '.$arr['house_number2'];
					}
					if ($arr['corp2']) {
						$addr[]= 'корпус '.$arr['corp2'];
					}
					if ($arr['structure2']) {
						$addr[]= 'строение '.$arr['structure2'];
					}
					if ($arr['apartment2']) {
						$addr[]= 'квартира '.$arr['apartment2'];
					}
					$arr[$key] = trim(implode(', ', $addr));
					if (!$arr[$key]) {
						$arr[$key] = $arr['vat'];
					}
				}
			break;
			case 'seconddoc':
				switch ($value) {
					case 1:
						$arr[$key] = 'Вод. удостоверение';
					break;
					case 2:
						$arr[$key] = 'Заграничный паспорт';
					break;
					case 3:
						$arr[$key] = 'Свидетельство ИНН';
					break;
					case 4:
						$arr[$key] = 'Пенсионное удостоверение';
					break;
					case 5:
						$arr[$key] = 'Удостоверение генерала';
					break;
					case 6:
						$arr[$key] = 'Cвидетельство о регистрации ТС';
					break;
					case 7:
						$arr[$key] = 'Карточка пенсионного страхования';
					break;
				}
			break;
			case 'validate':
				if (!in_array($arr[$key], array(4,5,6,12,13,14))) {
					unset($arr['pay_date']);
					unset($arr['contract_id']);
				}
				$arr[$key] = $this->update_likes_status($value);
				$arr[$key] = $arr[$key][0];
				
				
				/* Перезвонить
				------------------------------------------------------------------------------*/
				if (($this->is_sb) or ($this->is_manager)) {
					if (!$this->is_sb) {
						$dude = 'manager';
					} else {
						$dude = 'sb';
					}
					
					if ($arr['recall_'.$dude]) {
						$arr[$key].= ', <span class="text-orange">Отложен</span>';
						if ($arr['recall_reason_'.$dude]) {
							$arr[$key].= ' <span class="text-orange">(Причина: '.$this->form_text($arr['recall_reason_'.$dude]).')</span>';
						}
					}
				}
	
				
				/* Проверка в НБКИ
				------------------------------------------------------------------------------*/
				if ($arr['recall_nbki']) {
					$arr[$key].= ', <span class="text-orange">Отложен на проверку в НБКИ</span>';
				}
				
				/* Корзина
				------------------------------------------------------------------------------*/
				if (($this->is_sb) or ($this->is_manager)) {
					if (!$this->is_sb) {
						$dude = 'manager';
					} else {
						$dude = 'sb';
					}
					
					if ($arr['trash_'.$dude]) {
						$arr[$key].= ', <span class="text-red">В корзине</span>';
					}
				}
			break;
			case 'sb_id':
			case 'buh_id':
			case 'collector_id':
				if ($arr[$key]) {
					$arr[$key] = $this->get_staff_name($arr[$key], false);
					$arr[$key.'_r'] = $this->get_staff_name($arr[$key], true);
				} else {
					$arr[$key] = '';
				}
			break;
			case 'manager_id':
				if ($arr[$key]) {
					$id = $arr[$key];
					$arr['manager_name'] = $this->get_staff_name($id, false);
					$arr['manager_name_r'] = $this->get_staff_name($id, true);
					$arr['manager_dov'] = $this->get_manager_status($id);
					
					$arr[$key] = trim($arr['manager_name']).', действующего на основании Доверенности №'.$arr['manager_dov'];
					$arr[$key.'_r'] = trim($arr['manager_name_r']).', действующего на основании Доверенности №'.$arr['manager_dov'];
				}

			break;
		
			case 'tariff':
				$arr[$key] = $this->form_text($arr[$key], 'db');
				$query = mysql_query("SELECT `name`, `percent` FROM `wp_loans_tariffs` WHERE id={$arr[$key]}");
				$row = mysql_fetch_array($query, MYSQL_ASSOC);
				$arr[$key] = $row['name'];
				if ($arr[$key]) {
					$arr[$key].= ' ('.(float)$row['percent'].'%)';
				}
			break;
			case 'relative':
				switch ($value) {
					case 0:
						$arr[$key] = 'Нет ближайших родственников';
					break;
					case 1:
						$arr[$key] = 'Отец';
					break;
					case 2:
						$arr[$key] = 'Мать';
					break;
					case 3:
						$arr[$key] = 'Муж';
					break;
					case 4:
						$arr[$key] = 'Жена';
					break;
					case 5:
						$arr[$key] = 'Брат';
					break;
					case 6:
						$arr[$key] = 'Сестра';
					break;
					case 7:
						$arr[$key] = 'Сын';
					break;
					case 8:
						$arr[$key] = 'Дочь';
					break;
				}
			break;
			case 'images':
			case 'photo':
				$arr_tmp = unserialize($arr[$key]);
				
				if ((!$arr[$key]) or (count($arr_tmp) < 1)) {
					$arr[$key]='<div class="jsPhotoWrap-'.$key.'"><form class="dropzone-'.$key.' dropzone"></form></div>';
				} else {
					
					$arr[$key] = '';
					setlocale(LC_ALL, 'ru_RU.utf8');
					foreach ($arr_tmp as $img) {
						$tmp = pathinfo($img);
						if (in_array(strtolower($tmp['extension']), array('jpg', 'jpeg', 'png', 'gif', 'bmp'))) {
							$arr[$key].='
							<div class="dz-preview dz-image-preview dz-success">
							<a class="fancybox fancybox-thumb" rel="'.$key.'-thumb" href="/wp-content/uploads/photos/'.$img.'">
								<div class="dz-details">
									<div class="dz-size" data-dz-size="">
										'.$img.'
									</div>
									<img src="/wp-content/uploads/photos/thumb/'.$tmp['filename'].'-thumb.'.$tmp['extension'].'" alt="'.$img.'" data-dz-thumbnail="">
								</div>
								<a id="'.$img.'" class="dz-remove jsDeletePhoto" href="javascript:delete;" data-dz-remove="">Удалить</a>
							</a>
							</div>
							';
						} else {
							$arr[$key].='
							<div class="dz-preview dz-image-preview dz-success">
							<a target="_blank" href="/wp-content/uploads/docs/'.$img.'">
								<div class="dz-details">
									<div class="dz-size" data-dz-size="">
										'.$img.'
									</div>
									<img src="/wp-content/plugins/mfunny-cabs/css/images/unknownassociation.png" alt="'.$img.'" data-dz-thumbnail="">
								</div>
								<a id="'.$img.'" class="dz-remove jsDeletePhoto" href="javascript:delete;" data-dz-remove="">Удалить</a>
							</a>
							</div>
							';
						}
					}
					
					$arr[$key]='<div class="jsPhotoWrap-'.$key.'"><form class="dropzone-'.$key.' dropzone dz-ajaxed">'.$arr[$key].'</form></div>';
				}
			break;
		
			case 'manager_calls':
				// только для выданных
				if ($arr['real']['validate'] != 5) {
					unset($arr[$key]);
					break;
				}
				
				if ($this->stats['days_till_end'] > 3) {
					unset($arr[$key]);
					break;
				}
				
				switch ($arr['real'][$key]) {
					default:
					case 0: // дефолт - нет обзвонов
						$checked1 = '';
						$checked2 = '';
						$disable1 = '';
						$disable2 = 'disabled';
					break;
					case 1: // только первый обзвон
						$checked1 = 'checked';
						$checked2 = '';
						$disable1 = '';
						$disable2 = '';
					break;
					case 2: // два обзвона
						$checked1 = 'checked';
						$checked2 = 'checked';
						$disable1 = 'disabled';
						$disable2 = '';
					break;
				}
				// тот же это менеджер?
				$query6 = mysql_query("SELECT `user_id` FROM `wp_loans` WHERE id={$arr['real']['id']}");
				$row6 = mysql_fetch_array($query6, MYSQL_ASSOC);
				$query6 = mysql_query("SELECT `manager_id` FROM `wp_loans_users` WHERE id={$row6['user_id']}");
				$row6 = mysql_fetch_array($query6, MYSQL_ASSOC);
				
				if ($row6['manager_id'] != $this->user_id) {
					if ($this->is_admin) {
						$disable1 = $disable2 = 'disabled';
					} else {
						unset($arr[$key]);
						break;
					}
				}
				
				$arr[$key] = '
				<input type="checkbox" id="jsManagerRecall1" class="checkbox" '.$checked1.' '.$disable1.'> <label for="jsManagerRecall1">За 3 дня</label>
				<span class="vert-separator"></span>
				<input type="checkbox" id="jsManagerRecall2" class="checkbox" '.$checked2.' '.$disable2.'> <label for="jsManagerRecall2">За 1 день</label>';
			break;
		
			case 'manager_calls2':
				if (!current_user_can('loan_recall2')) {
					unset($row['manager_calls2']);
					unset($row['manager_calls2_comment']);
					break;
				}
					
				$val = $arr[$key];
				$checked = '';
				if ($val) {
					$checked = 'checked';
				}
				$arr[$key] = '<input type="checkbox" id="manager_calls2-'.$val.'" class="checkbox jsManagerRecall3" '.$checked.' '.$disable.'>
				<label for="manager_calls2-'.$val.'">Заемщик приглашен в офис</label>';
				if (($checked) and ($arr['manager_calls2_date'] != '0000-00-00 00:00:00')) {
					$arr[$key] .= ' '.$this->get_time($arr['manager_calls2_date'], 'full', true);
				}
				
				if (($val) and ($arr['manager_calls2_comment'])) {
					$arr[$key] .= '
					<p>'.$this->loans_translate('manager_calls2_comment').'</p>
					<p>'.$this->form_text($arr['manager_calls2_comment']).'</p>
					';
				}
				
			break;
		
			case 'pay_method':
				switch ($arr[$key]) {
					case '1':
						$arr[$key] = 'Спецсетьстройбанк';
					break;
					case '2':
						$arr[$key] = 'Контакт';
					break;
					case '3':
						$arr[$key] = 'Сбербанк';
					break;
					case '4':
						$arr[$key] = 'RoboKassa';
					break;
					case '5':
						if ($_POST['id']) {
							$_GET['id'] = $_POST['id'];
						}
						$arr[$key] = 'Наличные <a href="'.$this->site_url.'print-contract2?id='.$_GET['id'].'&doc=7&v='.$arr['id'].'" target="_blank" class="word-doc fancy-tip" title="Приходный кассовый ордер"></a>';
					break;
					case '6':
						$arr[$key] = 'Робокасса';
						if (trim($arr['pay_method2'])) {
							$arr[$key] .= ' ('.trim($arr['pay_method2']).')';
						}
					break;
				}
			break;
			case 'branch':
				$arr[$key] = $this->form_text($arr[$key], 'db');
				$query = mysql_query("SELECT `name` FROM `wp_loans_branches` WHERE id='{$arr[$key]}'");
				@$row = mysql_fetch_array($query, MYSQL_ASSOC);
				$arr[$key] = $row['name'];
			break;
			
			case 'author':
				if ($arr[$key] == 0) {
					$arr[$key] = 'Неизвестный сотрудник';
				} else {
					$arr[$key] = $this->get_staff_name($arr[$key], false, false, 'initials');
				}
			break;
			case 'blocked':
				if ($arr[$key] == '0000-00-00 00:00:00') {
					$arr[$key] = '';
				} else {
					$arr[$key] = 'Дата блокировки: '.$this->get_time($arr[$key], 'full', true);
				}
			break;
			case 'score':
				$arr[$key] = $this->score_to_stars($arr[$key], $arr['real']['user_id']);
			break;
			case 'comment':
				$arr['text'] = $this->form_text($arr[$key]);
				
				switch ($arr['real']['comment_type']) {
					case 5:
						$comment_label = '';
						if ($arr['text']) {
							$comment_label = ' Комментарий:';
						}
						$arr['text'] = '<p class="text-orange text-small">Отметил, что заемщику надо позвонить '.str_replace(', ', ' в ', $this->get_time($arr['special'], 'full')).'.'.$comment_label.'</p>'.$arr['text'];
					break;
					case 6:
						$comment_label = '';
						if ($arr['text']) {
							$comment_label = ' Как прошёл звонок:';
						}
						
						$arr['text'] = '<p class="text-green text-small">Отметил, что звонок состоялся.'.$comment_label.'</p>'.$arr['text'];
					break;
				}
				
			break;
			case 'street_type1':
			case 'street_type2':
				switch ($arr[$key]) {
					case '00':
					case '0':
					case '':
						$arr[$key] = '';
					break;
					case '01':
						$arr[$key] = 'Аллея';
					break;
					case '02':
						$arr[$key] = 'Бульвар';
					break;
					case '03':
						$arr[$key] = 'Въезд';
					break;
					case '04':
						$arr[$key] = 'Дорога';
					break;
					case '05':
						$arr[$key] = 'Заезд';
					break;
					case '06':
						$arr[$key] = 'Казарма';
					break;
					case '07':
						$arr[$key] = 'Квартал';
					break;
					case '08':
						$arr[$key] = 'Километр';
					break;
					case '09':
						$arr[$key] = 'Кольцо';
					break;
					case '10':
						$arr[$key] = 'Линия';
					break;
					case '11':
						$arr[$key] = 'Местечко';
					break;
					case '12':
						$arr[$key] = 'Микрорайон';
					break;
					case '13':
						$arr[$key] = 'Набережная';
					break;
					case '14':
						$arr[$key] = 'Парк';
					break;
					case '15':
						$arr[$key] = 'Переулок';
					break;
					case '16':
						$arr[$key] = 'Переезд';
					break;
					case '17':
						$arr[$key] = 'Площадь';
					break;
					case '18':
						$arr[$key] = 'Площадка';
					break;
					case '19':
						$arr[$key] = 'Проспект';
					break;
					case '20':
						$arr[$key] = 'Проезд';
					break;
					case '21':
						$arr[$key] = 'Просек';
					break;
					case '22':
						$arr[$key] = 'Проселок';
					break;
					case '23':
						$arr[$key] = 'Проулок';
					break;
					case '24':
						$arr[$key] = 'Строение';
					break;
					case '25':
						$arr[$key] = 'Территория';
					break;
					case '26':
						$arr[$key] = 'Тракт';
					break;
					case '27':
						$arr[$key] = 'Тупик';
					break;
					case '28':
						$arr[$key] = 'Улица';
					break;
					case '29':
						$arr[$key] = 'Участок';
					break;
					case '30':
						$arr[$key] = 'Шоссе';
					break;
				}
			break;
			
			case 'force_percent':
				$arr[$key] = (double)$arr[$key];
				if ($arr[$key]) {
					$arr[$key] .= '%';
				}
			break;
			
			case 'nbki_hide':
				if ($arr[$key]) {
					$arr[$key] = '';
					if (current_user_can('loan_pull_ki')) {
						$arr[$key] .= '<input name="nbki_hide" type="checkbox" class="checkbox" id="jsHideFromNBKI">&nbsp;';
					}
					$arr[$key] .= '<label for="jsHideFromNBKI" class="text-red">Приостановлена</label>';
				} else {
					$arr[$key] = '';
					if (current_user_can('loan_pull_ki')) {
						$arr[$key] .= '<input name="nbki_hide" type="checkbox" class="checkbox" id="jsHideFromNBKI" checked>&nbsp;';
					}
					$arr[$key] .= '<label for="jsHideFromNBKI" class="text-green">Активна</label>';
				}
			break;
			
			case 'sms_off':
				if ($arr[$key]) {
					$arr[$key] = '';
					if (current_user_can('loan_sms')) {
						$arr[$key] .= '<input name="sms_off" type="checkbox" class="checkbox" id="jsSmsOff">&nbsp;';
					}
					$arr[$key] .= '<label for="jsSmsOff" class="text-red">Выключены</label>';
				} else {
					$arr[$key] = '';
					if (current_user_can('loan_sms')) {
						$arr[$key] .= '<input name="sms_off" type="checkbox" class="checkbox" id="jsSmsOff" checked>&nbsp;';
					}
					$arr[$key] .= '<label for="jsSmsOff" class="text-green">Влючены</label>';
				}
			break;	
			
			case 'fulladdr_old':
				if (($arr['city1']) or ($arr['street_type1']) or ($arr['street1']) or ($arr['house_number1']) or ($arr['structure1']) or ($arr['apartment1']) or ($arr['corp1'])) {
					$arr[$key] = 0;
				}
			break;
			case 'fulladdr':
				$addr = array();
				$fallback = $arr[$key];
				if ($arr['city1']) {
					$addr[]= $arr['city1'];
				}
				if ($arr['street_type1']) {
					if (is_numeric(trim($arr['street_type1']))) {
						$row = array('street_type1' => $arr['street_type1']);
						$row = $this->loans_translate_vals($row);
					} else {
						$row['street_type1'] = $arr['street_type1'];
					}
					$street = $row['street_type1'];
					if ($arr['street1']) {
						$street .= ' '.$arr['street1'];
					}
					$addr[]= $street;
				}
				if ($arr['house_number1']) {
					$addr[]= 'дом '.$arr['house_number1'];
				}
				if ($arr['corp1']) {
					$addr[]= 'корпус '.$arr['corp1'];
				}
				if ($arr['structure1']) {
					$addr[]= 'строение '.$arr['structure1'];
				}
				if ($arr['apartment1']) {
					$addr[]= 'квартира '.$arr['apartment1'];
				}
				$arr[$key] = trim(implode(', ', $addr));
				if (!$arr[$key]) {
					$arr[$key] = $fallback;
				}
				
			break;
			case 'remind':
				if ($arr[$key]) {
					$arr[$key] = '<span class="text-green">За '.$arr[$key].' дн.</span>';
				} else {
					$arr[$key] = '<span class="text-red">Не напоминать</span>';
				}
				
				$arr[$key] = '<span id="remind_slider_holder">'.$arr[$key].'</span>';
				if (($arr['remind_date']) and ($arr['remind_date'] != '0000-00-00')) {
					if ($this->date_local_short == $arr['remind_date']) {
						$arr[$key].= ' <span class="text-red remind_slider_clear">(сегодня)</span>';
					} else {
						$arr[$key].= '<span class="remind_slider_clear"> ('.$this->get_time($arr['remind_date'], 'compact', false).')</span>';
					}
				}
				
				
				
				if (current_user_can('loan_remind_slider')) {
					$_POST['id'] = $arr['id'];
					if ($this->check_ajax_rights('loan_remind_slider', false, false, false)) {
						$arr[$key] .= '<div id="remind_slider"></div>';
					}
				}
			break;
			
		}
	}
	unset($arr['vat']);
	return $arr;
}

function term_to_years($term) {
	$years = $month = $days = 0;
	
	while($term >= 360) {
		$term = $term - 360;
		$years++;
	}
	while($term >= 30) {
		$term = $term - 30;
		$month++;
	}
	if ($term > 0) {
		$days = $term;
	}
	
	if ($years) {
		switch (substr($years, -1)) {
			case 1:
				$years.= ' год ';
			break;
			case 2:
			case 3:
			case 4:
				$years.= ' года ';
			break;
			default:
				$years.= ' лет ';
			break;
		}
	} else {
		$years = '';
	}
	if ($month) {
		$month.= ' мес. ';
	} else {
		$month = '';
	}
	if ($days) {
		$days.= ' дн.';
	} else {
		$days = '';
	}
	
	return $years.$month.$days;
}

function loans_translate_vals_4list($arr) {
	if (!is_array($arr)) {
		return false;
	}
	//echo '<pre>'; var_export($arr); echo '</pre>';die;  //отладка
	
	foreach ($arr as $key => $value) {
	    switch ($key) {
			default:
				$tmp_var = array(
					'id' => $arr['id'],
					$key => $value
				);
				$tmp_var = $this->loans_translate_vals($tmp_var);
				$arr[$key] = $tmp_var[$key];
			break;
			case 'user_validate':
				$arr['validate'] = $this->translate_validate_4user($arr[$key]);
				unset($arr['user_validate']);
			break;
			case 'manager_id':
			case 'sb_id':
			case 'buh_id':
			case 'collector_id':
				if ($arr[$key]) {
					$arr[$key] = $this->get_staff_name($arr[$key], false, true);
				} else {
					$arr[$key] = '';
				}
			break;
			case 'full_name':
				// работает - не трогай
			break;
			case 'manager_calls2':
				if (!current_user_can('loan_recall2')) {
					unset($row['manager_calls2']);
					unset($row['manager_calls2_comment']);
				}
					
				$val = $arr[$key];
				$checked = '';
				if ($val) {
					$checked = 'checked';
				}
				$arr[$key] = '<input type="checkbox" class="checkbox jsManagerRecall3" '.$checked.' '.$disable.'>';
				
				if (($checked) and ($arr['manager_calls2_date'] != '0000-00-00 00:00:00')) {
					$time = '<p><span class="text-small"> Приглашен '.$this->get_time($arr['manager_calls2_date'], 'full_dig', true).'</span></p>';
				} else {
					$time = '';
				}
				
				if (($val) and ($arr['manager_calls2_comment'])) {
					$comment = '
					<p>
						'.$this->loans_translate('manager_calls2_comment').'
					</p>
					<p>
						'.$this->form_text($arr['manager_calls2_comment']).'
					</p>';
					$class = 'have-comment';
				} else {
					$comment = '';
					$class = '';
				}
				
				if (($val) and ($arr['manager_calls2_comment']) or ($time)) {
				
					$arr[$key] .= '&nbsp;<a class="jsSpoilerToggler btn-typ '.$class.'"></a><div class="jsSpoilerContentWrap"><div class="jsSpoilerContent" style="display:none;">
						'.$time.'
						'.$comment.'
					</div></div>';
				}
				
				$arr[$key].='&nbsp;<a title="Редактировать" class="btn-edit"></a>';
			break;
			case 'last_comment4':
			case 'last_comment4_full':
				$k = 'loan_comment4';
				$can_add = current_user_can($k);
				
				if (($can_add) and (!$this->is_sb)) {
					$_POST['id'] = $arr['loan_id'];
					$can_add = $this->check_ajax_rights($k, false, false, false);
				}
				
				$add_btn = '';
				if ($can_add) {
					$add_btn = '<span id="jsCommentContainer-4-'.$arr['id'].'" class="jsCommentContainer"><a class="jsAddComment jsAddComment2 btn-typ" href="#" title="Добавить"></a></span>';
				}
				
				$arr[$key] = $this->form_text($arr[$key]);
				switch ($arr['real']['comment_type']) {
					case 5:
						$comment_label = '';
						if (trim($arr[$key])) {
							$comment_label = ' Комментарий:';
						}
						$arr[$key] = '<p class="text-orange text-small">Отметил, что заемщику надо позвонить '.str_replace(', ', ' в ', $this->get_time($arr['real']['special'], 'full')).'.'.$comment_label.'</p>'.$arr[$key];
					break;
					case 6:
						$comment_label = '';
						if (trim($arr[$key])) {
							$comment_label = ' Как прошёл звонок:';
						}
						
						$arr[$key] = '<p class="text-green text-small">Отметил, что звонок состоялся.'.$comment_label.'</p>'.$arr[$key];
					break;
				}
				
				if ($arr[$key]) {
					$moderate = '';
					if (($arr['real']['author'] == $this->user_id) or ($this->is_admin)) {
						$moderate = '<a title="Редактировать" class="btn-edit"></a>';
					}
					
					if ($key == 'last_comment4') {
						$arr[$key] = $add_btn.'&nbsp;<a class="jsSpoilerToggler btn-typ"></a><div class="jsSpoilerContentWrap"><div class="jsSpoilerContent" style="display:none;">
						<p class="text-gray text-small">'.$arr['comment_date'].'</p>
						<p><b>'.$arr['author'].'</b>:</p>
						<p><span class="quote">'.$arr[$key].'</span></p>
						</div></div>&nbsp;'.$moderate;
					} else {
						$arr[$key] = '<p class="centered">'.$add_btn.'</p>
						<p class="text-gray text-small">'.$arr['comment_date'].'</p>
						<p><b>'.$arr['author'].'</b>:</p>
						<span class="quote">'.$arr[$key].'</span>&nbsp;'.$moderate;
					}
				} else {
					$arr[$key] = $add_btn;
				}
			break;
			case 'blocked':
				if ($arr[$key] == '0000-00-00 00:00:00') {
					$arr[$key] = '';
				} else {
					$arr[$key] = $this->get_time($arr[$key], 'full', true);
				}
			break;
			case 'last_doc':
				$arr[$key] = '<a class="file-icon fancy-tip" href="'.$this->site_url.'print-contract/?id='.$arr[$key].'&doc=last" target="_blank" title="Просмотреть договор займа"></a>';
			break;
			case 'pay_method':
				switch ($arr[$key]) {
					case '1':
						$arr[$key] = 'Спецсетьстройбанк';
					break;
					case '2':
						$arr[$key] = 'Контакт';
					break;
					case '3':
						$arr[$key] = 'Сбербанк';
					break;
					case '4':
						$arr[$key] = 'RoboKassa';
					break;
					case '5':
						$arr[$key] = 'Наличные';
					break;
					case '6':
						$arr[$key] = 'Робокасса';
						if (trim($arr['pay_method2'])) {
							$arr[$key] .= ' ('.trim($arr['pay_method2']).')';
						}
					break;
				}
			break;
			case 'payment_date':
				if ($arr['user'] == 'user') {
					$arr[$key] = $this->get_time($arr[$key], 'full', true);
				} else {
					$arr[$key] = $this->get_time($arr[$key], 'compact', false);
				}
				
			break;
		}
	}
	return $arr;
}

function get_staff_name($id, $rod=false, $tiny=false, $format='') {
	
	$id = $this->form_text($id, 'db');
	// Родительный подеж
	if ($rod) {
		$m_name = get_user_meta($id, 'mname_r', true);
		$f_name = get_user_meta($id, 'fname_r', true);
		$s_name = get_user_meta($id, 'sname_r', true);
		if (!$m_name) {
			$m_name = get_user_meta($id, 'mname', true);
		}
		if (!$f_name) {
			$f_name = get_user_meta($id, 'first_name', true);
		}
		if (!$s_name) {
			$s_name = get_user_meta($id, 'last_name', true);
		}
	} else {
		$m_name = get_user_meta($id, 'mname', true);
		$f_name = get_user_meta($id, 'first_name', true);
		$s_name = get_user_meta($id, 'last_name', true);
	}
	switch ($format) {
		default:
			$out = $s_name.' '.$f_name;
			if (!$tiny) {
				$out.= ' '.$m_name;
			}
		break;
		case 'initials':
			$out = $s_name;
			if (($out) and ($f_name)) {
				$out.='&nbsp;';
			}
			if ($f_name) {
				$out.= mb_substr($f_name, 0, 1).'.';
			}
			if (($out) and ($m_name)) {
				$out.='&nbsp;';
			}
			if ($m_name) {
				$out.= mb_substr($m_name, 0, 1).'.';
			}
		break;
	}

	if (!trim($out)) {
		$user_info = get_userdata($id);
		$out = $user_info->user_login;
	}
	
	return $out;
}

function get_manager_status($id) {
	$numb = get_user_meta($id, 'manager_pa', true);
	if ($numb) {
		$out .= $numb;
		$date = get_user_meta($id, 'manager_pa_date', true);
		if ($date) {
			list($day, $month, $year) = explode('/', $date);
			$date = $year.'-'.$month.'-'.$day;
			$date = $this->get_time($date, 'compact');
			$out .= ' от '.$date.'';
		}
	}
	return $out;
}

function show_pages($num_pages, $get_page='', $range=5, $classes='') {
/*
* Я писал эту функцию когда мне было 14
* $num_pages - общее количество страниц
* $get_page - активная страница
* $range - длина "хвоста" влево и вправо от текущей страницы (минимум 3)
*/

	// Ссылка нафиг не нужна, ибо аджаксим
	$link = '#';
    //выставляем страницу 1 по дефолту
    if (!$get_page) { $get_page=1; }
    //начинаем плясать от ссылки на текущую страницу
    $pages = '<a class="page-unactive page-current" href="#page='.$get_page.'">'.$get_page.'</a>';
    $left_counter=$right_counter=0;
    for ($i=1; $i<$range+1; $i++) {
        //считаем левый хвост
        if ($get_page-$i>0) {
            $left_counter++;
        }
        //считаем правый хвост
        if ($get_page+$i<$num_pages+1) {
            $right_counter++;
        }
    }
    //если левый хвост короткий, делаем правый хвост длиньше
    if ($left_counter < $range) {
        $right_counter=$right_counter+($range-$left_counter);
    }
    //если правый хвост короткий, делаем левый хвост длиньше
    if ($right_counter < $range) {
        $left_counter=$left_counter+($range-$right_counter);
    }
    //пересчитываем на случай, если оба хвоста короткие
    $n=$left_counter;
    $m=0;
    for ($i=1; $i<$n+1; $i++) {
        //считаем левый хвост
        if ($get_page-$i>0) {
            $m++;
        }
    }
    $left_counter=$m;
    $m=0;
    
    $n=$right_counter;
    for ($i=1; $i<$n+1; $i++) {
        //считаем правый хвост
        if ($get_page+$i<$num_pages+1) {
            $m++;
        }
    }
    $right_counter=$m;
    //проверяем, нужно ли добавлять ... и крайний номер
    if ($get_page-$left_counter > 1) {
        $left_counter=$left_counter-2;
        $left_span=true;
    }
    if ($get_page+$right_counter < $num_pages) {
        $right_counter=$right_counter-2;
        $right_span=true;
    }
    //собираем левый хвост
    for ($i=1; $i<$left_counter+1; $i++) {
        if ($get_page-$i>0) {
            $left='<a href="'.$link.'page='.($get_page-$i).'">'.($get_page-$i).'</a>'.$left;
        }
    }
    //собираем правый хвост
    for ($i=1; $i<$right_counter+1; $i++) {
        if ($get_page+$i<$num_pages+1) {
            $right=$right.'<a href="'.$link.'page='.($get_page+$i).'">'.($get_page+$i).'</a>';
        }
    }
    if ($left_span) {
        $left='<a href="'.$link.'page=1">1</a><span>...</span>'.$left;
    }
    if ($right_span) {
        $right=$right.'<span>...</span><a href="'.$link.'page='.$num_pages.'">'.$num_pages.'</a>';
    }
    //формируем кнопки вперёд-назад
    if ($get_page>1) {
        $href = 'href="'.$link.'page='.($get_page-1).'"';
    } else {
        $href = 'href="'.$link.'page='.($get_page).'" class="page-unactive"';
    }
    $back = '<a '.$href.'>&larr;</a>';
    unset($href);
    if ($get_page<$num_pages) {
        $href = 'href="'.$link.'page='.($get_page+1).'"';
    } else {
        $href = 'href="'.$link.'page='.($get_page).'" class="page-unactive"';
    }
    $forward = '<a '.$href.'>&rarr;</a>';
    //собираем всё воедино
	//$hide_mode = '';
	//if ($num_pages < 2) {
	//	$hide_mode = 'style="display:none;"';
	//}
	
    $pages = '
    <div class="page-nav-wrap1" '.$hide_mode.'>
		<div class="page-nav-wrap2">
			<div class="page-nav '.$classes.'">
                '.$back.$left.$pages.$right.$forward.'
            </div>
		</div>
	</div>';
    return $pages;
}

function local_to_utc($date, $format = 'Y-m-d H:i:s') {
	// предохранитель от кривых рук
	if (!substr_count($date, ' ')) {
		$date .= ' 12:00:00';
	}

	$date = new DateTime($date, new DateTimeZone('Europe/Moscow'));
	$time_zone = new DateTimeZone('UTC');
	
	$date->setTimezone($time_zone);
	return $date->format($format);
}

function utc_to_local($date, $format = 'Y-m-d H:i:s') {
	// предохранитель от кривых рук
	if (!substr_count($date, ' ')) {
		$date .= ' 12:00:00';
	}
	
	$date = new DateTime($date, new DateTimeZone('UTC'));
	$time_zone = new DateTimeZone('Europe/Moscow');
	
	$date->setTimezone($time_zone);
	
	return $date->format($format);
}


function get_time($timestamp, $format='full', $gmt_offset=false) {
	if (substr($timestamp, 0, 10) == '0000-00-00') {
		return '';
	}
	
	// у нас дата рождения и выдачи паспорта вводится в свободном формате, по локальному времени, поэтому им не нужно делать оффсет
	if ($gmt_offset) {
		$timestamp = $this->utc_to_local($timestamp);
	}

	$_LANG['01']='января';
	$_LANG['02']='февраля';
	$_LANG['03']='марта';
	$_LANG['04']='апреля';
	$_LANG['05']='мая';
	$_LANG['06']='июня';
	$_LANG['07']='июля';
	$_LANG['08']='августа';
	$_LANG['09']='сентября';
	$_LANG['10']='октября';
	$_LANG['11']='ноября';
	$_LANG['12']='декабря';
    
	list($date, $time) = explode(' ', $timestamp);
	list($year, $month, $day) = explode('-', $date);
	if ($time) {
		list($hour, $minute, $sec) = explode(':', $time);
	} else {
		$hour = $minute = $sec = '00';
	}

    switch($format) {
        case 'full':
			$month = $_LANG[$month];
			$timestamp = $day.' '.$month.' '.$year.', '.$hour.':'.$minute;
        break;
        case 'full_dig':
			$timestamp = $day.'.'.$month.'.'.$year.', '.$hour.':'.$minute;
        break;
		case 'tiny_dig':
			$timestamp = $day.'.'.$month.'.'.$year;
        break;
        case 'compact':
			$month = $_LANG[$month];
			$timestamp = $day.' '.$month.' '.$year.' г.';
        break;
        case 'compact2':
			$month = $_LANG[$month];
			$timestamp = $day.' '.mb_substr($month, 0, 3).' '.$year;
        break;
        case 'fancy':
			$month = $_LANG[$month];
			$timestamp = '«'.$day.'» '.$month.' '.$year.' г.';
        break;
        case 'tiny':
			$timestamp = $day.'/'.$month.'/'.$year;
        break;
		case 'tiny_w_time':
			$timestamp = $day.'/'.$month.'/'.$year.' '.$hour.':'.$minute;
        break;
        case 'db': // только из tiny
			list($date, $time) = explode(' ', $timestamp);
			list($day, $month, $year) = explode('/', $date);
			$timestamp = $year.'-'.$month.'-'.$day.' 12:00:00';
        break;
		case 'db_w_time':
			list($date, $time) = explode(' ', $timestamp);
			list($day, $month, $year) = explode('/', $date);
			$timestamp = $year.'-'.$month.'-'.$day.' '.$time.':00';
        break;
		case 'db_date':
			list($date, $time) = explode(' ', $timestamp);
			list($day, $month, $year) = explode('/', $date);
			$timestamp = $year.'-'.$month.'-'.$day;
        break;
		case 'ki':
			list($date, $time) = explode(' ', $timestamp);
			$timestamp = str_replace('-', '', $date);
		break;
        case 'ki_time':
            list($date, $time) = explode(' ', $timestamp);
			$timestamp = str_replace(':', '', $time);
        break;
		case 'ki_out':
			list($datetime, $zone) = explode('+', $timestamp);
			list($date, $time) = explode(' ', $datetime);
			list($year, $month, $day) = explode('-', $date);
			$timestamp = $day.'.'.$month.'.'.$year;
			if ($time) {
				$timestamp .= ' '.$time;
			}
		break;
    }
	
return $timestamp;
}

function show_loans($mode, $view, $orderby='', $direct='', $page='', $search='', $date='', $datefield='') {
	
	// затычка от js-косяков
	if ($view == 'undefined') {
		$view = 'new';
	}
	
	// дефолты для необязательных параметров (ибо они могут быть заменены на пустые)
	if (!$direct) {
		$direct = 'Asc';
	}

	$page = preg_replace("/[^0-9]/", '', $page);
	if (!$page) {
		$page = 1;
	}
	
	$where = $this->get_where($mode, $view);

/* Поиск
------------------------------------------------------------------------------*/
	$search = trim($search);
	$query_str = '';
	if ($search !== '') {
		$int_types = array(
			'int',
			'tinyint',
			'bigint',
		);
		$text_types = array(
			'varchar',
			'text',
		);
		$no_search = array (
			'id',
			'password',
			'auth',
			'temp',
			'user_id'
		);
		
		// поиск каждого слова
		$search_arr = explode(' ', $search);
		$where_arr = array();
		
		foreach ($search_arr as $s) {
			
			$text_fields = $all_fields = array();
			$s = $this->form_text($s, 'db');
			
			$query = mysql_query("SHOW COLUMNS FROM `wp_loans`");
			while( $row = mysql_fetch_array($query, MYSQL_ASSOC)) {
				$row['Type'] = preg_replace("'(.*?)(\(.*?)\)'is", "\\1", $row['Type']);
				if (($row['Type'] == 'date') or ($row['Type'] == 'datetime')) {
					continue;
				}
				if (in_array($row['Field'], $no_search)) {
					continue;
				}
				
				if (in_array($row['Type'] ,$int_types)) {
					$all_fields[] = "l.`{$row['Field']}` LIKE '%$s%'";
				}
				
				if (in_array($row['Type'] ,$text_types)) {
					$text_fields[] = "l.`{$row['Field']}` LIKE '%$s%'";
					$all_fields[] = "l.`{$row['Field']}` LIKE '%$s%'";
				}
			}
			
			$query = mysql_query("SHOW COLUMNS FROM `wp_loans_users`");
			while( $row = mysql_fetch_array($query, MYSQL_ASSOC)) {
				$row['Type'] = preg_replace("'(.*?)(\(.*?)\)'is", "\\1", $row['Type']);
				if (($row['Type'] == 'date') or ($row['Type'] == 'datetime')) {
					continue;
				}
				if (in_array($row['Field'], $no_search)) {
					continue;
				}
				
				if (in_array($row['Type'] ,$int_types)) {
					$all_fields[] = "u.`{$row['Field']}` LIKE '%$s%'";
				}
				
				if (in_array($row['Type'] ,$text_types)) {
					$text_fields[] = "u.`{$row['Field']}` LIKE '%$s%'";
					$all_fields[] = "u.`{$row['Field']}` LIKE '%$s%'";
				}
			}
			
			if (preg_match("'^([0-9]+)$'is",$s)) {
				$query_str = implode(' OR ', $all_fields);
			} else {
				$query_str = implode(' OR ', $text_fields);
			}
			
			$where .= " AND ($query_str)";
		}
	}
	
	if ($date) {
		$date_sql = $this->get_where_date($datefield);
		
		if (is_array($date)) {
			// фильтр по календарю от - до
			$date[0] = $this->get_time($date[0], 'db');
			$date[1] = $this->get_time($date[1], 'db');
			$date[0] = explode(' ', $date[0]);
			$date[0] = $date[0][0];
			$date[1] = explode(' ', $date[1]);
			$date[1] = $date[1][0];

			$dates = $this->see_today_into_tomorrow($date[0]);
			$date_prev = $dates['prev'];
			$dates = $this->see_today_into_tomorrow($date[1]);
			$date_next = $dates['next'];

			$where .= " AND (($date_sql >= '{$date[0]} 00:00:00' AND $date_sql <= '{$date[1]} 23:59:59') OR $date_sql LIKE '$date_prev%' OR $date_sql LIKE '$date_next%')";
			$banned_days = array($date_prev, $date_next);
			$dates = $this->see_today_into_tomorrow($date_prev);
			$banned_days[] = $dates['prev'];
			$dates = $this->see_today_into_tomorrow($date_next);
			$banned_days[] = $dates['next'];
		} else {
			// вывод одного дня
			$date_now = $this->get_time($date, 'db');
			$date_now = explode(' ', $date_now);
			$date_now = $date_now[0];

			$dates = $this->see_today_into_tomorrow($date_now);
			$date_prev = $dates['prev'];
			$date_next = $dates['next'];
			// смотрим сегодня в завтрашний и вчерашний день, во избежании косяков с UTC
			$where .= " AND ($date_sql LIKE '$date_now%' OR $date_sql LIKE '$date_prev%' OR $date_sql LIKE '$date_prev%')";
			$banned_days = array($dates['prev'], $dates['next']);
			$dates = $this->see_today_into_tomorrow($date_prev);
			$banned_days[] = $dates['prev'];
			$dates = $this->see_today_into_tomorrow($date_next);
			$banned_days[] = $dates['next'];
		}
	}
		
	$per_page = 40;
	$clickable = false;
	// массив некликабельных TR - используется для инструментов модерирования
	$not_clickable = array();
	// массив полей, по которым нельзя сортировать
	$sorting_locked = array('last_doc');
	// поля, которые сортируются сложным способом
	$not_sortable = array('last_comment4', 'last_comment4_full', 'expired_days', 'stat_now_amount', 'stat_now_percent_sum', 'stat_now_percent_rub_sum');
	// Вывод разных полей для разных видов
	$check_field = 'l.`id`';
	// подсвечивать или нет чужие заявки серым
	$HL_bros_loans = true;
	
	
	
	// при глобальном поиске показываем те же поля
	if ($mode == 'search') {
		$mode_bac = $mode;
		$mode = $_POST['rel'];
	}
	
	// Поля, которые достаются отдельным запросом
	$add_fields = array();
	
	switch ($mode) {
		default:
			$fields = 'l.`date`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`manager_id`, u.`sb_id`, u.`buh_id`, u.`branch`';
		break;
		case 'manage':
			$fields = 'l.`date`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`manager_id`, u.`sb_id`, u.`buh_id`, u.`branch`, `phones`, `manager_calls2`';
			//$moder_btns = 'manager_calls2';
			$not_clickable = array('manager_calls2', 'last_comment4');
			$add_fields[] = 'last_comment4';
		break;
		case 'expired':
			switch ($view) {
				default:
					$fields = 'l.`back_date`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`manager_id`, u.`sb_id`, u.`buh_id`, u.`branch`, `phones`, `manager_calls2`';
					//$moder_btns = 'manager_calls2';
					$not_clickable = array('manager_calls2', 'last_comment4');
					$add_fields[] = 'last_comment4';
				break;
				case 'raw':
					$fields = '`full_name`';
					$add_fields[] = 'stat_now_amount';
					$add_fields[] = 'stat_now_percent_sum';
					$add_fields[] = 'stat_now_percent_rub_sum';
				break;
			}
		break;
		break;
		case 'single':
			$fields = 'l.`date`, l.`amount`, l.`term`, l.`tariff`, l.`validate`';
			$per_page = 5;
			$capture = '<h3>Другие займы этого пользователя</h3>';
		break;
		case 'user':
			$fields = 'l.`date`, l.`amount`, l.`term`, l.`tariff`, l.`validate`';
			$per_page = 5;
			$capture = '<h3>Все ваши заявки:</h3>';
			$clickable = false;
		break;
		case 'user_loans':
			$fields = '`back_date_real_tiny`, l.`contract_id`, l.`amount`, `expired_days`, `percent`, `last_doc`';
			$per_page = 5;
			$capture = '';
			$clickable = false;
		break;
		case 'users':
			$fields = '`full_name`';
			$check_field = 'u.`id`';
		break;
		
		// Списки Буха
		case 'pay':
			$fields = 'l.`date`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`buh_id`, u.`branch`, `phones`';
		break;
		case 'debts':
			$fields = '`pay_date`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`buh_id`, u.`branch`, `phones`';
		break;
		case 'exceed':
			switch ($view) {
				default:
					$fields = 'l.`back_date`,`pay_date`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`buh_id`, u.`branch`, `phones`, `manager_calls2`';
					//$moder_btns = 'manager_calls2';
					$not_clickable = array('manager_calls2', 'last_comment4');
					$add_fields[] = 'last_comment4';
				break;
				case 'raw':
					$fields = '`full_name`';
					$add_fields[] = 'stat_now_amount';
					$add_fields[] = 'stat_now_percent_sum';
					$add_fields[] = 'stat_now_percent_rub_sum';
				break;
			}
		break;
		case 'success':
			$fields = 'l.`date`,`pay_date`,l.`back_date_real`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`buh_id`, u.`branch`, l.`validate`, `phones`';
		break;
		case 'nbki':
		case 'recall':
		case 'egosearch':
			$fields = 'l.`date`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`manager_id`, u.`sb_id`, u.`buh_id`, u.`branch`, `validate`';
		break;
		case 'collect':
			switch ($view) {
				default:
					$fields = '`pay_date`, l.`back_date`, l.`amount`, l.`term`, `full_name`';
					$clickable = true;
					$HL_bros_loans = false;
					$not_clickable = array('last_comment4');
					$add_fields[] = 'last_comment4';
				break;
				case 'raw':
					$fields = '`full_name`';
					$add_fields[] = 'stat_now_amount';
					$add_fields[] = 'stat_now_percent_sum';
					$add_fields[] = 'stat_now_percent_rub_sum';
				break;
			}
		break;
		case 'bans':
			$fields = '`full_name`, `loans_count`, u.`relative`, u.`branch`, u.`sb_id`, `phones`, u.`blocked`';
		break;
		case 'need_call':
		case 'need_call_all':
			// сумма с просрочкой
			$fields = 'l.`need_call_date`, l.`amount`, l.`term`, `full_name`, u.`branch`, `phones`';
			$add_fields[] = 'stat_now_amount_full';
			$add_fields[] = 'last_comment4_full';
			$not_clickable = array('last_comment4_full');
		break;
	
		case 'payments':
			$fields = '`payment_date`, `full_name`, p.`amount`, p.`pay_method`';
		break;
		case 'graph':
			$fields = 'l.`pay_date`, l.`amount`, l.`term`, l.`tariff`, `full_name`, u.`manager_id`, u.`sb_id`, u.`buh_id`, u.`branch`';
		break;
	}
	
	if ($view == 'raw') {
		$per_page = 0;
	}
	
	// при глобальном поиске показываем те же поля + статус заявки
	if ($mode_bac) {
		if (!substr_count($fields, 'validate')) {
			$fields.=',l.`validate`';
		}
		$mode = $mode_bac;
	}
	
	
	if (!$orderby) {
		$orderby = explode(',', $fields);
		$orderby = $orderby[0];
	}
	
	// если у сотрудника указан филиал, выводим заявки или без филиала или с филиалом сотрудника
	$user_branch = get_user_meta($this->user_id, 'branch', true);
	if (($user_branch) and (!$this->is_admin) and ($mode != 'single') and ($mode != 'user') and ($mode != 'user_loans') and (!get_user_meta($this->user_id, 'branch_global', true))) {
		$where.=" AND (u.`branch`=0 OR u.`branch`=$user_branch)";
	}
	
	// после формировки where, фильтруем лишние даты, если у нас задан фильтр по дате
	if ($date) {
		$date_sql_simple = explode('`', $date_sql);
		$date_sql_simple = $date_sql_simple[1];
		$query = mysql_query("SELECT $date_sql $where");
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$dateee = $this->utc_to_local($row[$date_sql_simple], 'Y-m-d');
			if (in_array($dateee, $banned_days)) {
				$where.=" AND $date_sql != '{$row[$date_sql_simple]}'";
			}
		}
	}
	//echo "SELECT COUNT($check_field) AS numb $where";
	//die;
	// Узнаём общее количество страниц по запросу
	$query = mysql_query("SELECT COUNT($check_field) AS numb $where");
	$row = mysql_fetch_array($query);
	$error = mysql_error();
	if ($error) {
		return $error;
	}
	
	if ($per_page) {
		$num_pages=ceil($row['numb']/$per_page);
		$limit = $per_page*($page-1).', '.$per_page;
		$limit_str = "LIMIT $limit";
	} else {
		$limit_str = "";
	}

	// Для краткости записи выше
	$fields_sql = str_replace("`full_name`", "(CONCAT(u.`sname`, ' ', u.`fname`, ' ', u.`tname`)) AS `full_name`", $fields);
	$fields_sql = str_replace('`phones`', "(CONCAT(u.`phone1`, '
', u.`phone2`)) AS `phones`", $fields_sql);
	$fields_sql = str_replace("`manager_calls2`", "l.`manager_calls2`, l.`manager_calls2_comment`, l.`manager_calls2_date`", $fields_sql);
	$fields_sql = str_replace("`loans_count`", "COUNT(l.`id`) AS `loans_count`", $fields_sql);
	$fields_sql = str_replace("`percent`", "t.`percent` AS `percent`", $fields_sql);
	$fields_sql = str_replace("`last_doc`", "l.`id` AS `last_doc`", $fields_sql); // фейк
	$fields_sql = str_replace("`expired_days`", "l.`id` AS `expired_days`", $fields_sql); // фейк
	$fields_sql = str_replace("`back_date_real_tiny`", "`back_date_real` AS `back_date_real_tiny`", $fields_sql);
	$fields_sql = str_replace("`payment_date`", "p.`date` AS `payment_date`", $fields_sql);
	$fields_sql = str_replace("`fake_date`", "l.`date` AS `fake_date`", $fields_sql);

	// обратный порядок для дат
	if (substr_count($orderby, 'date')) {
		if ($direct == 'Desc') {
			$direct = 'Asc';
		} else {
			$direct = 'Desc';
		}
	}
	
	if ($moder_btns) {
		$moder_btns_sql = str_replace("manager_calls2", "l.`manager_calls2`, l.`manager_calls2_comment`", $moder_btns);
		
		$sql_moder = "SELECT $check_field AS `id` $fields_sql, $moder_btns_sql $where ORDER BY $orderby ".strtoupper($direct)." $limit_str";
		$query_moder = mysql_query($sql_moder);
		//echo $sql_moder.'<br>'.mysql_error();
		while ($row_tmp = mysql_fetch_array($query_moder, MYSQL_ASSOC)) {
			$row_moder[$row_tmp['id']] = $row_tmp;
		}
	}
	

	// главный массив, который мы должны получить в этой секции и прогнать дальше
	$rows = array();
	
	if (!in_array($orderby, $not_sortable)) {
		// обычная сортировка в SLQ-запросе
		$main_sql = "SELECT $check_field, $fields_sql, l.`have_graph`, l.`back_date`,
		l.`need_call_date`, u.`blocked`, u.`manager_id` AS `manager_check`, u.`sb_id` AS `sb_check`, u.`collector_id` AS `collector_check`, u.`buh_id` AS `buh_check`
		$moder_btns_sql $where";
		
		$sql = "$main_sql ORDER BY $orderby ".strtoupper($direct)." $limit_str";
		
		
		//echo $sql.'<br>'.mysql_error();
		$query = mysql_query($sql);
		if (!$query) {
			switch ($mode) {
				default:
					$content = '<div class="ajaxContainer"><div class="jsModeHolder" style="display:none;">'.$mode.'</div><div class="green-message jsFirstFieldHolder" id="'.$fields[0].'">Записи отсутствуют (Код: #1). '.mysql_error().'</div></div>';
				break;
				case 'single':
					$content = '
					<div class="jsModeHolder" style="display:none;">'.$mode.'</div>
					<h3>Другие займы этого пользователя</h3>
					<div class="white-wrap">
						Нет других займов (Код: #1). '.mysql_error().'
					</div>';
				break;
				case 'user':
					$content = '<div class="jsModeHolder" style="display:none;">'.$mode.'</div>
					<p>У вас пока нет займов (Код: #1)</p>';
				break;
				case 'user_loans':
					$content = '<div class="jsModeHolder" style="display:none;">'.$mode.'</div>
					<div class="no-other-loans-wrapper">Нет истории займов (Код: #1).</div>';
				break;
			}
			
			return $content;
		}
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$rows[] = $row;
		}
	} else {
		// сортировка по несортируемому полю
		$ids = $sort_field = $hard_fields = array();
		
		// вытаскиваем айдишники всех записей и несортируемое поле для каждого
		$query = mysql_query("SELECT $check_field $moder_btns_sql $where");
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			// ключи в обоих массивов получаются одинаковые
			$ids[] = $row['id'];
			$hard_field = $this->get_hard_field2($orderby, $row['id']);
			if (count($hard_field)) {
				$sort_field[] = $hard_field['real'];
				// также сохраняем значение для каждого id, чтобы не вытаскивать второй раз
				$hard_fields[$row['id']] = $hard_field['pretty'];
			}
		}
		
		// переставляем айдишники местами, сортируя по выбранной графе
		if (strtoupper($direct) == 'ASC') {
			array_multisort($sort_field, SORT_ASC, $ids);
		} else {
			array_multisort($sort_field, SORT_DESC, $ids);
		}
		
		// вырезаем нужную страницу из массива IDшников
		list($start, $stop) = explode(', ', $limit);
		$n = $m = 0;
		
		foreach ($ids as $id) {
			if ($n < (int)$start) {
				$n++;
				continue;
			}
			
			// самое мясо
			// теперь просто вытаскиваем все записи поочередно
			$query = mysql_query("$main_sql AND $check_field=$id");
			
			$rows[$m] = mysql_fetch_array($query, MYSQL_ASSOC);
			// это поле мы уже вытаскивали выше
			$rows[$m][$orderby] = $hard_fields[$id];
			$m++;
			
			if ($m >= (int)$stop) {
				break;
			}
		}
	}

	
	// обратный порядок для дат (этот кусок нужен для правильного вывода стрелки)
	if (substr_count($orderby, 'date')) {
		if ($direct == 'Desc') {
			$direct = 'Asc';
		} else {
			$direct = 'Desc';
		}
	}

	// После формирования SQL-запроса, с этим массивом можно творить что угодно
	$fields = str_replace('l.', '', $fields);
	$fields = str_replace('u.', '', $fields);
	$fields = str_replace('p.', '', $fields);
	$fields = str_replace('t.', '', $fields);
	$fields = str_replace('`', '', $fields);
	$fields = explode(',', $fields);
	$fields = array_merge($fields, $add_fields);
	
	if ($moder_btns) {
		$moder_btns = str_replace('l.', '', $moder_btns);
		$moder_btns = str_replace('u.', '', $moder_btns);
		$moder_btns = str_replace('p.', '', $moder_btns);
		$moder_btns = str_replace('t.', '', $moder_btns);
		$moder_btns = str_replace('`', '', $moder_btns);
		$moder_btns = explode(',', $moder_btns);
	}

	$clickable_const = $clickable;
	$ITOGO = array(); // для https://moneyfunny.ru/raw
	// для tr
	
	foreach ($rows as $row) {
		$back_date = $row['back_date'];
		if (($mode == 'user') or ($mode == 'user_loans')) {
			$row['user_validate'] = $row['validate'];
			unset($row['validate']);
		}
		$branch = $row['branch'];
		$blocked = $row['blocked'];
		$need_call_back = $row['need_call_date'];
		$tariff = $row['tariff'];
		$validate = $row['validate'];
		if ($view != 'raw') {
			$row = $this->loans_translate_vals_4list($row);
		}
		$section = '';
		$addclass = '';
		$clickable = $clickable_const;
		$n = 0;
		
		if (($mode != 'user') and ($mode != 'user_loans')) {
			if (($row['sb_check'] == $this->user_id) or ($row['manager_check'] == $this->user_id) or ($row['buh_check'] == $this->user_id) or ($row['collector_check'] == $this->user_id) or
			(($this->is_sb) and (!$row['sb_check'])) or (($this->is_manager) and (!$row['manager_check'])) or (($this->is_buh) and (!$row['buh_check']))  or (($this->is_collector) and (!$row['collector_check']))) {
				
				$clickable = true;
				
			} else {
				if ($HL_bros_loans) {
					$addclass .= ' brosLoan';
				}
				if ($this->is_sb) {
					$clickable = true;
				}
			}
			
			if ($this->is_admin) {
				$clickable = true;
			}
			
			if (($need_call_back != '0000-00-00 00:00:00') and ($this->is_first_date_bigger($this->date, $back_date))) {
				$addclass .= ' need_call_back';
			}
			
			$query22 = mysql_query("SELECT `id` FROM `wp_loans` WHERE `fake`={$row['id']}");
			@$row22 = mysql_fetch_array($query22, MYSQL_ASSOC);
			if (($this->is_admin) and ($row22['id'])) {
				$addclass .= ' KJASjkhaJAfasd';
			}
			
			if ($validate == 14) {
				$addclass .= ' uncollected';
			}
			
			if ($blocked != '0000-00-00 00:00:00') {
				if (!$addclass) {
					$addclass .= ' blocked-list2';
				} else {
					$addclass .= ' blocked-list';
				}
			}
		
			if (!$branch) {
				$addclass .= ' nobranch';
			}
			
			if ($row['have_graph']) {
				$addclass .= ' got-graph';
			}
			
			if ($tariff == 13) {
				$addclass .= ' startup';
			}
		}
				
		if ($addclass) {
			$addclass = 'class="'.$addclass.'"';
		}
		
		if ($moder_btns) {
			$buttons = '';
			foreach ($moder_btns as $fieldd) {
				$fieldd = trim($fieldd);
				$row_moder[$row['id']] = $this->get_tool($fieldd, $row_moder[$row['id']]);
				$buttons.= $row_moder[$row['id']][$fieldd].' ';
			}
		}
		
		$stats = array();
		
		// для td
		foreach ($fields as $field) {
			$field = trim($field);
			$n++;
			
			// поля, которые нельзя просто так взять и достать SQL-запросом
			$hard_field = $this->get_hard_field2($field, $row['id'], $stats);
			if (count($hard_field)) {
				$row[$field] = $hard_field['pretty'];
			}
			if ($view == 'raw') {
				// ИТОГО для raw
				switch ($field) {
					case 'stat_now_amount':
					case 'stat_now_percent_rub_sum':
						$ITOGO[$field]['real'] = $ITOGO[$field]['real'] + $hard_field['real'];
					break;
					case 'full_name':
						if (!$ITOGO[$field]['real']) {
							$ITOGO[$field]['real'] = 0;
						}
						$ITOGO[$field]['real']++;
					break;
				}
			}
			
			if (($clickable) and (!in_array($field, $not_clickable))) {
				$row[$field] = '<a href="'.$this->site_url.'sb_cabinet/?mode=single&rel='.$mode.'&id='.$row['id'].'" class="loan">'.$row[$field].'</a>';
			}
			$section.='
			<td class="'.$field.'">'.$row[$field].'</td>';
		}

		unset($stats);
		
		if ($section) {
			if ($buttons) {
				$section.='
				<td>'.$buttons.'</td>';
			}
			
			if ($s%2 ==0) {
				$zebra = 'odd';
			} else {
				$zebra = 'even';
			}
			$s++;
			
			$content.= '
			<tr id="loan-'.$row['id'].'"'.$addclass.' class="'.$zebra.'">
				'.$section.'
			</tr>';
		}
	}
	
	
	if (($n) and ($view == 'raw')) {
		$itogo_str = '';
		
		foreach ($fields as $field) {
			// перевод ИТОГО
			switch ($field) {
				case 'stat_now_amount':
				case 'stat_now_percent_rub_sum':
					$ITOGO[$field]['pretty'] = number_format($ITOGO[$field]['real'], 0, '.', ' ').' р.';
				break;
				case 'stat_now_percent_sum':
					$ITOGO[$field]['real'] = round($ITOGO['stat_now_percent_rub_sum']['real'] * 100 / $ITOGO['stat_now_amount']['real'], 2);
					$ITOGO[$field]['pretty'] = $ITOGO[$field]['real'].'%';
				break;
				case 'full_name':
					$ITOGO[$field]['pretty'] = $ITOGO[$field]['real'].' чел.';
				break;
			}
		
			if ($ITOGO[$field]['real']) {
				$itogo_str.='<td><b>'.$ITOGO[$field]['pretty'].'</b></td>';
			} else {
				$itogo_str.='<td>&nbsp;</td>';
			}
		}
		$content.='
		<tr>'.$itogo_str.'</tr>';
	
	}
	
	if ($n == 0) {
		switch ($mode) {
			default:
				$content = '<div class="ajaxContainer"><div class="jsModeHolder" style="display:none;">'.$mode.'</div><div class="green-message jsFirstFieldHolder" id="'.$fields[0].'">Записи отсутствуют (Код: #2).</div></div>';
				//if ($this->user_id == 21) {
					//$content .= '<br class="clear"><br><br>';
					//$content .= "mode=$mode, view=$view, orderby=$orderby, direct=$direct, page=$page, search=$search, date=$date, datefield=$datefield <br>";
					//$content .= $sql;
				//}
			break;
			case 'single':
				$content = '
				<div class="jsModeHolder" style="display:none;">'.$mode.'</div>
				<h3>Другие займы этого пользователя</h3>
				<div class="white-wrap">
					Нет других займов. '.mysql_error().'
				</div>';
			break;
			case 'user':
				$content = '<div class="jsModeHolder" style="display:none;">'.$mode.'</div>
				<p>У вас пока нет займов. '.mysql_error().'</p>';
			break;
			case 'user_loans':
				$content = '<div class="jsModeHolder" style="display:none;">'.$mode.'</div>
				<div class="no-other-loans-wrapper">Нет истории займов. '.mysql_error().'</div>';
			break;
		}
		return $content;
	}
	
	$orderby =  str_replace('`', '', $orderby);
	$orderby =  str_replace('l.', '', $orderby);
	$orderby =  str_replace('u.', '', $orderby);
	$class = array();
	$class[$orderby] = ' class="js'.$direct.'"';
	
	$section = '';
	foreach ($fields as $field) {
		$field = trim($field);
		$addclass = '';
		
		if (in_array($field, $sorting_locked)) {
			$addclass.= 'jsNotSortable';
		}
	    $section .= '
		<td'.$class[$field].' id="'.$field.'" class="table-capt '.$addclass.'">'.$this->loans_translate_4list($field).'</td>';
	}
	if ($buttons) {
		$section.='
		<td>Управление</td>';
	}

	if (($mode != 'single') and ($mode != 'user') and ($mode != 'user_loans'))  {
		$tips = '
		<ul class="tip-wrap">
			<li><span class="centered brosLoan">а</span> - заявки других сотрудников.</li>
			<li><span class="centered need_call_back">а</span> - в списке "Работа с просрочкой".</li>
			<li><span class="centered blocked">а</span> - заявки от заёмщиков в чёрном списке.</li>
			<li><span class="centered uncollected">а</span> - заявки от заёмщиков с безнадёжным долгом.</li>
			<li><span class="centered nobranch">а</span> - заемщики без филиала.</li>
			<li><span class="centered got-graph">а</span> - есть график выплат.</li>
			<li><span class="centered startup">а</span> - заявки с тарифом "Стартап".</li>
		</ul>';
	} else {
		$tips = '';
	}
	
	$content = '
		'.$capture.'
		<div class="jsModeHolder" style="display:none;">'.$mode.'</div>
		<table class="jsSelectable jsSortable jsFePayments">
			<thead>
			<tr>
				'.$section.'
			</tr>
			</thead>
			<tbody>
				'.$content.'
			</tbody>
		</table>
		'.$tips;
	
	$content .= $this->show_pages($num_pages, $page, 5, 'jsLoansListPages');
	
	//if ($this->user_id == 21) {
		//$content .= '<br class="clear"><br><br>';
		//$content .= "mode=$mode, view=$view, orderby=$orderby, direct=$direct, page=$page, search=$search, date=$date, datefield=$datefield <br>";
		//$content .= $sql;
	//}
	
	$content = '<div class="ajaxContainer">'.$content.'<div class="clear"></div></div>';
		
	return $content;
}

function get_where($mode, $view='', $add_and='') {
	$main_sql = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE u.`id`=l.`user_id` AND l.`fake`=0 AND u.`fake`=0";
	
	// затычка для "Отображать заявки всех сотрудников" (здесь, а не в ajax, как не странно)
	$_POST['show_all'] = preg_replace("/[^0-9]/", '', $_POST['show_all']);
	if (($_POST['show_all']) or ($_COOKIE['show_all'])) {
		if (!current_user_can('loan_view_all')) {
			unset($_POST['show_all']);
			unset($_COOKIE['show_all']);
		}
	}
	
	if (($view == 'raw') and (current_user_can('loan_view_all'))) {
		$_POST['show_all'] = 1;
	}
	
	switch ($mode) {
		/* Список новых
		------------------------------------------------------------------------------*/
		case 'new':
			$and = 'AND u.sb_id IN(0,'.$this->form_text($this->user_id, 'db').')';
			$and_stirct =  'AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0';
			
			// Выпадающий список
			switch ($view) {
				default:
				case 'new': // Ожидает первой проверки
					$validate = 0;
				break;
				case 'allowed':
					$validate = 1; // Ожидает оформления менеджером
				break;
			}
		break;
		
		/* Список от бухгалтера
		------------------------------------------------------------------------------*/
		case 'counter':
			$and = 'AND u.sb_id IN(0,'.$this->form_text($this->user_id, 'db').')';
			$and_stirct = ' AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0';
			
			// Выпадающий список
			switch ($view) {
				default:
				case 'new': // Отправлен в СБ на контрольную проверку
					$validate = 2;
				break;
				case 'allowed': // Ожидает оформления
					$validate = 3;
				break;
			}
		break;
		/* Список от СБ
		------------------------------------------------------------------------------*/
		case 'manage':
			$and = 'AND u.manager_id IN(0,'.$this->form_text($this->user_id, 'db').')';
			$and_stirct = 'AND l.recall_manager=0 AND l.trash_manager=0';
			
			// Выпадающий список
			switch ($view) {
				default:
				case 'new': // Ожидает дополнения менеджером
					$validate = 1;
				break;
				case 'allowed': // Отправлен в СБ на контрольную проверку
					$validate = 2;
				break;
			}
		break;
		/* Список от СБ2
		------------------------------------------------------------------------------*/
		case 'contract':
			$and = 'AND u.manager_id IN(0,'.$this->form_text($this->user_id, 'db').')';
			$and_stirct = 'AND l.recall_manager=0 AND l.trash_manager=0';
			
			// Выпадающий список
			switch ($view) {
				default:
				case 'new': // Ожидает оформления
					$validate = 3;
				break;
				case 'allowed': // Ожидает выплаты
					$validate = 4;
				break;
			}
		break;
		/* Заявки на выплату
		------------------------------------------------------------------------------*/
		case 'pay':
			$validate = 4;
			$and = 'AND u.buh_id IN(0,'.$this->form_text($this->user_id, 'db').')';
			$and_stirct = ' AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0';
		break;
		/* Выплаченные
		------------------------------------------------------------------------------*/
		case 'debts':
			$validate = 5;
			$and = array();
			if ($this->is_sb) {
				$and[0] = '(u.`sb_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($this->is_manager) {
				$and[1] = '(u.`manager_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($this->is_buh) {
				$and[2] = '(u.`buh_id`='.$this->form_text($this->user_id, 'db').')';
			}
			$and = 'AND ('.(implode(' OR ', $and)).')';

			$and_stirct = " AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0
			AND (DATEDIFF(l.`back_date`, UTC_TIMESTAMP())) >= 0";
		break;
		/* Просроченные (бух)
		------------------------------------------------------------------------------*/
		case 'exceed':
			$and = 'AND u.buh_id IN('.$this->form_text($this->user_id, 'db').')';
			
			// Выпадающий список
			switch ($view) {
				default:
				case 'new': // просроченные
					$validate = 5;
					$and_stirct = " AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0
					AND (DATEDIFF(l.`back_date`, UTC_TIMESTAMP())) < 0";
				break;
				case 'collect': // отправлены коллекторам
					$validate = 12;
					$and_stirct = ' AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0';
				break;
			}
		break;
		/* Просроченные (менеджер)
		------------------------------------------------------------------------------*/
		case 'expired':
			$validate = 5;
			$and = 'AND u.manager_id IN('.$this->form_text($this->user_id, 'db').')';
			$and_stirct = " AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0 
			AND (DATEDIFF(l.`back_date`, UTC_TIMESTAMP())) ";
			switch ($view) {
				default:
				case '0': // 3- (все перечисленные ниже)
					$and_stirct.='<= 3';
				break;
				case '1': // 3
					$and_stirct.='< 3 AND (DATEDIFF(l.`back_date`, UTC_TIMESTAMP())) > 1 AND l.`manager_calls` < 1';
				break;
				case '2': // 2
					$and_stirct.='< 2 AND (DATEDIFF(l.`back_date`, UTC_TIMESTAMP())) > 0 AND l.`manager_calls` < 1';
				break;
				case '3': // 1
					$and_stirct.='< 1 AND (DATEDIFF(l.`back_date`, UTC_TIMESTAMP())) = 0 AND l.`manager_calls` < 2';
				break;
				case '4': // 0-
					$and_stirct.='< 0';
				break;
				case 'raw':
					$and_stirct.='< 3 AND (DATEDIFF(l.`back_date`, UTC_TIMESTAMP())) >= 0';
				break;
			}
		break;
		/* Просроченные (СБ)
		------------------------------------------------------------------------------*/
		case 'need_call':
			$validate = 5;
			$and = 'AND u.sb_id IN(0, '.$this->form_text($this->user_id, 'db').')';
			$and_stirct = " AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0 
			AND (DATEDIFF(l.`back_date`, UTC_TIMESTAMP())) < 0
			AND `need_call_date` != '0000-00-00 00:00:00'";
		break;
		/* Перезвонить (менеджер)
		------------------------------------------------------------------------------*/
		case 'need_call_all':
			$validate = '5,12';
			$and = 'AND u.manager_id IN(0, '.$this->form_text($this->user_id, 'db').')';
			$and_stirct = " AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0
			AND ((`need_call_date` != '0000-00-00 00:00:00') OR (`remind_date` = '$this->date_local_short'))";
		break;
		/* Напоминание об окончании займа по выбранной дате (для SMS-рассылке по крону)
		------------------------------------------------------------------------------*/
		case 'remind_date':
			$validate = '5,12';
			$and = '';
			$and_stirct = " AND `remind_date` = '$this->date_local_short'";
		break;
		/* Возвращенные (архив)
		------------------------------------------------------------------------------*/
		case 'success':
			if ($this->is_sb) {
				$and_stirct = 'l.`validate` > 2 AND l.`validate` < 8';
				$and[0] = '(u.`sb_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($this->is_manager) {
				$and_stirct = 'l.`validate` > 3 AND l.`validate` < 8';
				$and[1] = '(u.`manager_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($this->is_buh) {
				$and_stirct = 'l.`validate` > 5 AND l.`validate` < 8';
				$and[2] = '(u.`buh_id`='.$this->form_text($this->user_id, 'db').')';
			}
			$and = 'AND ('.(implode(' OR ', $and)).')';
			if (($_POST['show_all']) or ($_COOKIE['show_all'])) {
				$and = '';
			}
			
			return "$main_sql AND $and_stirct $and";

		break;
		/* Перезвонить
		------------------------------------------------------------------------------*/
		case 'recall':
			// работает - не трогай!
			$and = array();
			// только СБ
			if (($this->is_sb) and (!$this->is_manager)) {
				$and =' AND u.`sb_id` IN(0,'.$this->form_text($this->user_id, 'db').')';
				$and_stirct = 'l.`recall_sb`=1';
			}
			// только Менеджер
			if ((!$this->is_sb) and ($this->is_manager)) {
				$and =' AND u.`manager_id` IN(0,'.$this->form_text($this->user_id, 'db').')';
				$and_stirct = 'l.`recall_manager`=1';
			}
			// и СБ и Менеджер
			if (($this->is_sb) and ($this->is_manager)) {
				$and =' AND ((u.`manager_id` IN(0,'.$this->form_text($this->user_id, 'db').')) OR (u.`sb_id` IN(0,'.$this->form_text($this->user_id, 'db').')))';
				$and_stirct = 'l.`recall_sb`=1';
			}
			
			if (($this->is_admin) and (($_POST['show_all']) or ($_COOKIE['show_all']))) {
				$and = '';
			}
			// свой return с блэк джеком и шлюхами
			//echo "$main_sql AND $and_stirct $and";
			return "$main_sql AND $and_stirct $and";
		break;
		/* Проверка в НБКИ
		------------------------------------------------------------------------------*/
		case 'nbki':
			$and = 'l.`recall_nbki`=1';
			if (!$this->is_admin) {
				 $and .=' AND ((u.`sb_id` IN(0,'.$this->form_text($this->user_id, 'db').')) OR (u.`manager_id` IN(0,'.$this->form_text($this->user_id, 'db').')))';
			}
			if (($_POST['show_all']) or ($_COOKIE['show_all'])) {
				$and = 'l.`recall_nbki`=1';
			}
			return "$main_sql AND $and";
		break;
		/* Поиск своих записей
		------------------------------------------------------------------------------*/
		case 'egosearch':
			$manager_check = $this->is_manager;
			$sb_check = $this->is_sb;
			$buh_check = $this->is_buh;
			if ($this->is_sb) {
				$max_vaidate = '< 3';
			}
			if ($this->is_manager) {
				$max_vaidate = '< 4';
			}
			if ($this->is_buh) {
				$max_vaidate = '< 6';
			}

			if ($view == 'sb') {
				$manager_check = false;
				$buh_check = false;
				$sb_check = true;
			}
			if ($view == 'manager') {
				$sb_check = false;
				$buh_check = false;
				$manager_check = true;
			}
			if ($view == 'buh') {
				$sb_check = false;
				$manager_check = false;
				$buh_check = true;
			}
			
			$and = array();
			if ($sb_check) {
				$and[0] = '(u.`sb_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($manager_check) {
				$and[1] = '(u.`manager_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($buh_check) {
				$and[2] = '(u.`buh_id`='.$this->form_text($this->user_id, 'db').')';
			}
			$and = '('.(implode(' OR ', $and)).')';
			
			if (($_POST['show_all']) or ($_COOKIE['show_all'])) {
				$and = '((u.`sb_id` !=0) OR (u.`manager_id` !=0))';

				if ($view == 'sb') {
					$and = '(u.`sb_id` !=0)';
				}
				if ($view == 'manager') {
					$and = '(u.`manager_id` !=0)';
				}
				if ($view == 'buh') {
					$and = '(u.`buh_id` !=0)';
				}
			}
			$and.=' AND l.`recall_sb`=0 AND l.`recall_nbki`=0 AND l.`trash_sb`=0 AND l.`validate` '.$max_vaidate;
			return "$main_sql AND $and";
		break;
		/* Корзина
		------------------------------------------------------------------------------*/
		case 'trash':
		case 'trash2':
			if ($this->is_sb) {
				$and_stirct = 'u.`sb_id`='.$this->form_text($this->user_id, 'db');
			}
			if ($this->is_manager) {
				$and_stirct = 'u.`manager_id`='.$this->form_text($this->user_id, 'db');
			}
			if (($this->is_manager) and ($this->is_sb)){
				$and_stirct = 'u.`sb_id`='.$this->form_text($this->user_id, 'db').' OR u.`manager_id`='.$this->form_text($this->user_id, 'db').'';
			}
			
			if (($this->is_sb) or ($this->is_manager)) {
				$and = 'l.`trash_sb`=1';
			}
			if ((!$this->is_sb) and ($this->is_manager)) {
				$and = 'l.`trash_manager`=1';
			}
			if (($_POST['show_all']) or ($_COOKIE['show_all'])) {
				$and_stirct = '';
			}
			
			if ($and_stirct) {
				$and_stirct = " AND ($and_stirct)";
			}
			
			// Выпадающий список
			switch ($view) {
				default:
				case 'new': // Любой
					return "$main_sql AND (l.`validate` IN(8,9) OR $and) $and_stirct";
				break;
				case 'denied': // В корзине
					return "$main_sql AND $and $and_stirct";
				break;
				case 'denied1': // Отклонен при первой проверке
					$validate = 8;
					$and = '(u.`sb_id`='.$this->form_text($this->user_id, 'db').')'; // отклонен мной
					
					$and = "AND $and";
					$and_stirct = '';
				break;
				case 'denied2': // Отклонен при повторной проверке
					$validate = 9;
					$and = '(u.`sb_id`='.$this->form_text($this->user_id, 'db').')'; // отклонен мной
					
					$and = "AND $and";
					$and_stirct = '';
				break;
			}
		break;
	
		/* Глобальный поиск
		------------------------------------------------------------------------------*/
		case 'search':
			$and = array();
			if ($this->is_sb) {
				$and[0] = '(u.`sb_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($this->is_manager) {
				$and[1] = '(u.`manager_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($this->is_buh) {
				$and[2] = '(u.`buh_id`='.$this->form_text($this->user_id, 'db').')';
			}
			$and = 'AND ('.(implode(' OR ', $and)).')';
			if (($_POST['show_all']) or ($_COOKIE['show_all'])) {
				$and = '';
			}
			return "$main_sql $and";
		break;
	
		/* Другие займы пользователя
		------------------------------------------------------------------------------*/
		case 'single':
			// $view - user_id
			$user_id = $this->form_text($view, 'db');
			if (!$_GET['id']) {
				$_GET['id'] = $_POST['id']; // займ, который выводить не надо
			}
			
			$where = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE l.`user_id`='$user_id' AND l.`id` != '".$this->form_text($_GET['id'], 'db')."' AND u.id = l.user_id";
			return $where;
		break;
	
		/* Все займы пользователя - для кабинета creditor_cabs
		------------------------------------------------------------------------------*/
		case 'user':
			// $view - user_id
			$user_id = $this->form_text($view, 'db');
			
			$where = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE l.`user_id`='$user_id' AND u.id = l.user_id AND l.`fake`=0";
			return $where;
		break;
		
		/* Удачно выданные и возвращенные займы
		------------------------------------------------------------------------------*/
		case 'user_loans':
			// $view - user_id
			$user_id = $this->form_text($view, 'db');
			
			// вытаскиваем последний открытый займ
			$query2 = mysql_query("SELECT `id` FROM `wp_loans` WHERE `user_id`=$user_id AND `validate` IN(5,12,13) ORDER BY `pay_date` DESC LIMIT 1");
			$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
			if ($row2['id']) {
				// если есть открытый займ, не показываем его (он показывается более подробно, с таймером и прочими наворотами)
				$where = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u`, `wp_loans_tariffs` AS `t` WHERE l.`user_id`='$user_id' AND l.`fake`=0 AND u.id = l.user_id AND t.`id`=l.`tariff` AND l.`id` != {$row2['id']} AND `validate`=6";
			} else {
				$where = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u`, `wp_loans_tariffs` AS `t` WHERE l.`user_id`='$user_id' AND l.`fake`=0 AND u.id = l.user_id AND t.`id`=l.`tariff` AND `validate`=6";
			}
			return $where;
		break;
	
		/* Тссссссс
		------------------------------------------------------------------------------*/
		case 'fake':
			$user_id = $this->form_text($view, 'db');
			
			$and = array();
			if ($this->is_sb) {
				$and[0] = '(u.`sb_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($this->is_manager) {
				$and[1] = '(u.`manager_id`='.$this->form_text($this->user_id, 'db').')';
			}
			if ($this->is_buh) {
				$and[2] = '(u.`buh_id`='.$this->form_text($this->user_id, 'db').')';
			}
			$and = 'AND ('.(implode(' OR ', $and)).')';
			if (($_POST['show_all']) or ($_COOKIE['show_all'])) {
				$and = '';
			}
			$where = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE u.id = l.user_id AND l.`fake` != 0 AND u.`fake` != 0 $and";
			return $where;
		break;
	
		/* Затычка (больше не используется)
		------------------------------------------------------------------------------*/
		case '404':
			$user_id = $this->form_text($view, 'db');
			
			$where = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE u.id = l.user_id AND l.`id` = 'ftw'";
			return $where;
		break;
	
		/* Список заемщиков
		------------------------------------------------------------------------------*/
		case 'users':
			$user_id = $this->form_text($view, 'db');
			
			$where = "FROM `wp_loans_users` AS `u`";
			return $where;
		break;
	
		/* Контрольная проверка админа
		------------------------------------------------------------------------------*/
		case 'control':
			$validate = '10, 11';
		break;
		
		/* Список коллектора - Требуют взыскания
		------------------------------------------------------------------------------*/
		case 'collect':
			$and_stirct = 'AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0';
			switch ($view) {
				default:
				case 'new':
					$validate = 12;
				break;
				case 'collect':
					$validate = 13;
				break;
			}
			
		break;
		/* Список коллектора - Долг взыскан
		------------------------------------------------------------------------------*/
		case 'collected':
			$and_stirct = 'AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0';
			$validate = 13;
		break;
		/* Список коллектора - Невозможно вернуть долг
		------------------------------------------------------------------------------*/
		case 'uncollected':
			$and_stirct = 'AND l.recall_sb=0 AND l.recall_nbki=0 AND l.trash_sb=0 AND l.recall_manager=0 AND l.trash_manager=0';
			$validate = 14;
		break;
		/* Чёрный список
		------------------------------------------------------------------------------*/
		case 'bans':
			$where = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE u.id = l.user_id AND u.`blocked` != '0000-00-00 00:00:00' GROUP BY u.`id`";
			return $where;
		break;
		/* Автоматические платежи от заемщиков
		------------------------------------------------------------------------------*/
		case 'payments':
			$where = ", p.`id` AS `payment_id`, p.`pay_method2`, p.`user` FROM `wp_loans` AS `l`, `wp_loans_users` AS `u`, `wp_loans_payments` AS `p` WHERE l.`id` = p.`loan_id` AND u.`id` = l.`user_id` AND p.`validate`=100 AND p.`user`='user'";
			return $where;
		break;
		/* График выплат
		------------------------------------------------------------------------------*/
		case 'graph':
			$where = "FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE validate IN(5,12,13) AND u.`id` = l.`user_id` AND l.`have_graph`=1";
			return $where;
		break;
	}

	
	if (($_POST['show_all']) or ($_COOKIE['show_all'])) {
		// заявки всех сотрудников не отображаем в выданных и истекающих - причуда заказчика
		//if ((($mode != 'debts') and ($mode != 'expired')) or ($this->is_admin)) {
			$and = '';
		//}
	}
	//if (($mode == 'expired') and (!$_POST['ajaxing'])) {
		//echo "$mode - $view".mysql_error();
	//}
	if ($add_and) {
		$and = $add_and;
	}
	
	return "$main_sql AND l.validate IN($validate) $and $and_stirct";
}

function loan_section_tpl($capture, $key_list, $row, $cook=array()) {
	$key_list = explode(',', $key_list);
	
	// Исключения
	$not_editable = array(
		'validate' => 'validate',
		'contract_id',
		'date',
		'back_date',
		'moderate',
		'moderate2',
		'moderate3',
		'photo',
		'images',
		'manager_calls',
		'blocked',
		'propiska',
		'fulladdr',
		'address_old',
		'fulladdr_old',
		'address_same'
	);
	if (!$this->is_admin) {
		$not_editable[] = 'sb_id';
		$not_editable[] = 'manager_id';
		$not_editable[] = 'buh_id';
		$not_editable[] = 'collector_id';
	}
	
	
	$not_copyble = array(
		'photo',
		'images',
		'manager_calls',
		'manager_calls2'
	);
	
	// костыль когда поле должно редактировать другое поле, но не всеми
	$not_always_visible = array (
		'manager_calls2',
		'pay_date_long',
		'blocked',
		'idea'
	);
	
	if ($row['real']['address'] == 1) {
		$not_always_visible[] = 'city2';
		$not_always_visible[] = 'street_type2';
		$not_always_visible[] = 'street2';
		$not_always_visible[] = 'corp2';
		$not_always_visible[] = 'structure2';
		$not_always_visible[] = 'house_number2';
		$not_always_visible[] = 'apartment2';
	}
	
	if (($this->is_sb) and ($row['recall_sb'])) {
		unset($not_editable['validate']);
	}
	
	if ((!$this->is_sb) and ($this->is_manager) and ($row['recall_manager'])) {
		unset($not_editable['validate']);
	}

	foreach ($key_list as $key) {
		$key = trim($key);
		
		$class = '';
		if ($cook[$key]) {
			$class = 'tr-hl';
		}
		
		// Костыль для типов займов - earnings выводятся не всегда (см loans_translate_vals())
		// pay_date выводится только при полной валидации (4)
		if (($key=='earnings') or ($key=='pay_date')) {
			if (!$row[$key]) {
				continue;
			}
		}

		$key_id = $key;
		switch ($key) {
			case 'moderate':
				$moder_html = $this->moderate;
			break;
			case 'moderate2':
				$moder_html = $this->moderate2;
			break;
			case 'moderate3':
				$moder_html = $this->moderate3;
			break;
			case 'term_long':
				$key_id = 'term';
			break;
			case 'amount_full':
				$key_id = 'amount';
			break;
			case 'pay_date_long':
				$key_id = 'pay_date';
			break;
			case 'branch':
				if (!$row[$key]) {
					$class.= ' nobranch';
				}
			break;
			case 'blocked':
				$class.= ' blocked';
			break;
		}
		
		if (!$row[$key]) {
			if (!in_array($key, $not_editable)) {
				$row[$key] = '&nbsp;';
			}
			if (in_array($key, $not_always_visible)) {
				$row[$key] = '';
			}
			
			if (($this->is_sb) and ($key == 'sb_id')) {
				$row[$key] = 'Не присвоено <a href="#" class="jsTakeSb take-btn fancy-tip" title="Взять в обработку"></a>';
			}
			
			if (($this->is_manager) and ($key == 'manager_id')) {
				$row[$key] = 'Не присвоено <a href="#" class="jsTakeManager take-btn fancy-tip" title="Взять в обработку"></a>';
			}
			
			if (($this->is_buh) and ($key == 'buh_id')) {
				$row[$key] = 'Не присвоено <a href="#" class="jsTakeBuh take-btn fancy-tip" title="Взять в обработку"></a>';
			}
			if (($this->is_collector) and ($key == 'collector_id')) {
				$row[$key] = 'Не присвоено <a href="#" class="jsTakeCollector take-btn fancy-tip" title="Взять в обработку"></a>';
			}
		}
		
		if ($row[$key]) {
			
			$edit_btn = $zclip_btn = '';
			if ((!in_array($key, $not_editable)) and (in_array($key_id, $this->privileges))) {
				 $edit_btn = '
				 <a title="Редактировать" class="btn-edit" id="'.$key_id.'"></a>';
			}
			//if (!in_array($key, $not_copyble)) {
			//	 $zclip_btn = '
			//	 <a title="Скопировать в буфер" class="btn-copy"></a>';
			//}
			if (($edit_btn) or ($zclip_btn)) {
				$row[$key] = '<div class="jsEditWrap">'.$row[$key].'</div>';
			}

			$content.='
			<tr id="'.$key_id.'" class="'.$class.'">
				<td width="30%">'.$this->loans_translate($key).'</td>
				<td>
					<div class="typ-rel">
						'.$row[$key].'
						'.$edit_btn.'
						'.$zclip_btn.'
						<a class="copy-active"></a>
					</div>
				</td>
			</tr>';
		}
	}
	
	if ($capture) {
		$capture = '<h3>'.$capture.'</h3>';
	}
	
	$content = '
	'.$capture.'
	<div class="wrep-wrap">
		<table class="jsSingleTable">
			<tbody>
				'.$content.'
			</tbody>
		</table>
	</div>
	'.$moder_html;
	return $content;
}



function update_likes_status($validate) {
	switch ($validate) {
		case 0:
			// Заявка только поступила, доступна первая секция
			$arr[1][1] = '';
			$arr[1][2] = '';
			$arr[1][3] = '';
			
			$arr[2][1] = 'locked';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';
			
			$arr[0] = '<span class="text-orange">Ожидает первой проверки</span>';
		break;
		case 1:
			// Заявка получила первый лайк, можно поставить только первый дислайк
			$arr[1][1] = 'locked selected';
			$arr[1][2] = '';
			$arr[1][3] = '';
			
			$arr[2][1] = 'locked';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = '';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';
			
			$arr[0] = '<span class="text-yellow">Ожидает дополнения менеджером</span>';
		break;
		case 2:
			// Заявка одобрена и оформлена менеджером, первая секция залочена, можно управлять только второй
			$arr[1][1] = 'locked selected';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = '';
			$arr[2][2] = '';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'selected';
			$arr[3][2] = 'Отозвать контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';
			
			$arr[0] = '<span class="text-orange">Отправлен в СБ на контрольную проверку</span>';
		break;
		case 3:
			// Заявка получила второй лайк, можно поставить только второй дислайк
			$arr[1][1] = 'locked selected';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = 'locked selected';
			$arr[2][2] = '';
			$arr[2][3] = '';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = '';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';
			
			$arr[0] = '<span class="text-yellow">Ожидает оформления</span>';
		break;
		case 4:
			// Заявка оформлена - полный лок
			$arr[1][1] = 'locked selected';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = 'locked selected';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'selected';
			$arr[4][2] = 'Расторгнуть договор';
			$arr[5][1] = '';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';

			$arr[0] = '<span class="text-green">Ожидает выплаты</span>';
		break;
		case 5:
			// Заявка выплачена - можно только отменить выплату
			$arr[1][1] = 'locked selected';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = 'locked selected';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Расторгнуть договор';
			$arr[5][1] = 'selected';
			$arr[5][2] = 'Из выданных';
			$arr[6][1] = '';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = '';
			$arr[8][2] = 'Отправить коллектору';
			
			$arr[0] = '<span class="text-green">Займ выдан</span>';
		break;
		case 6:
			// Заявка возвращен - можно только отменить выплату
			$arr[1][1] = 'locked selected';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = 'locked selected';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Расторгнуть договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'Из выданных';
			$arr[6][1] = 'selected';
			$arr[6][2] = 'Отменить закрытие займа';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';
			
			$arr[0] = '<span class="text-green">Займ закрыт</span>';
		break;
		case 8:
			// Заявка дислайкнута один раз, можно поставить только первый лайк
			$arr[1][1] = '';
			$arr[1][2] = 'locked selected';
			$arr[1][3] = '';
			
			$arr[2][1] = 'locked';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';
			
			$arr[0] = '<span class="text-red">Отклонен при первой проверке</span>';
		break;
		case 9:
			// Заявка дислайкнута дважды, можно поставить только второй лайк
			$arr[1][1] = 'locked selected';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = '';
			$arr[2][2] = 'locked selected';
			$arr[2][3] = '';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';

			$arr[0] = '<span class="text-red">Отклонен при контрольной проверке</span>';
		break;
		case 10:
			// Первый "не уверен"
			$arr[1][1] = '';
			$arr[1][2] = '';
			$arr[1][3] = 'locked selected';
			
			$arr[2][1] = 'locked';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';

			$arr[0] = '<span class="text-orange">Ожидает контрольной проверки администратором.</span>';
		break;
		case 11:
			// Второй "не уверен"
			$arr[1][1] = 'locked';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = '';
			$arr[2][2] = '';
			$arr[2][3] = 'locked selected';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'locked';
			$arr[8][2] = 'Отправить коллектору';

			$arr[0] = '<span class="text-orange">Ожидает контрольной проверки администратором.</span>';
		break;
		case 12:
			// Отправлено коллектору
			$arr[1][1] = 'locked';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = 'locked';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = 'locked';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = 'locked';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = 'selected';
			$arr[8][2] = 'Отозвать у коллектора';

			$arr[0] = '<span class="text-orange">Отправлено коллектору.</span>';
		break;
		case 13:
			// Коллектор отправил менеджеру
			$arr[1][1] = 'locked';
			$arr[1][2] = 'locked';
			$arr[1][3] = 'locked';
			
			$arr[2][1] = 'locked';
			$arr[2][2] = 'locked';
			$arr[2][3] = 'locked';
			
			$arr[3][1] = 'locked';
			$arr[3][2] = 'Отправить в СБ на контрольную проверку';
			$arr[4][1] = 'locked';
			$arr[4][2] = 'Заключить договор';
			$arr[5][1] = '';
			$arr[5][2] = 'В выданные';
			$arr[6][1] = '';
			$arr[6][2] = 'Закрыть займ';
			$arr[8][1] = '';
			$arr[8][2] = 'Отправить коллектору';

			$arr[0] = '<span class="text-orange">Коллектор вернул задолжность, требуется действие бухгалтера.</span>';
		break;
	}

	return $arr;
}

// Возвращает верстку полей для редактирования
function get_edit_field($field, $id, $edit_btns=true, $red_border=false, $optional=false) {
	$vat = '';
	if ($field == 'address') {
		$vat = ', vat';
	}
	if ($field == 'manager_calls2') {
		$vat = ', manager_calls2_comment';
	}
	
	if ($red_border) {
		$addclass = ' red-border';
	} else {
		$addclass = '';
	}
	if ($optional) {
		$addclass .= ' optional';
	}

	$field = $this->form_text($field ,'db');
	$id = $this->form_text($id ,'db');
	
	switch ($field) {
		default:
			// обычное редактирование полей
			$query = mysql_query("SELECT `$field`$vat FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE u.id = l.user_id AND l.`id`=$id");
		break;
		case 'comment':
			// комментарий к галке "заемщик приглашен в офис"
			$query = mysql_query("SELECT * FROM `wp_loans_comments` WHERE `id`=$id");
		break;
		case 'last_comment4':
		case 'last_comment4_full':
			$query = mysql_query("SELECT * FROM `wp_loans_comments` WHERE loan_id=$id AND `comment_type` IN(4,5,6) ORDER BY `date` DESC LIMIT 1");
		break;
	}
	
	$row = mysql_fetch_array($query, MYSQL_ASSOC);

	if ($row[$field] === 0) {
		$row[$field] = '';
	}

	$addClass = $hidden = '';
	switch ($field) {
		default:
			$html = '<textarea name="'.$field.'" type="text" class="'.$addclass.'">'.$this->form_text($row[$field], 'textarea').'</textarea>';
		break;

		case 'fname':
		case 'sname':
		case 'tname':
		case 'email':
		case 'relsname':
		case 'relfname':
		case 'reltname':
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" maxlength="255" class="'.$addclass.'">';
		break;
		case 'comment':
			$html = '<textarea name="type-'.$row['comment_type'].'" id="jsCommentContainer-'.$id.'" class="'.$addclass.'">'.$this->form_text($row['text'], 'textarea').'</textarea>';
		break;
		case 'paspser':
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="jsForceNumb'.$addclass.'" maxlength="4">';
		break;
		case 'paspnom':
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="jsForceNumb'.$addclass.'" maxlength="7">';
		break;
		case 'phone1':
		case 'phone2':
		case 'relphone':
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" maxlength="30" class="'.$addclass.'">';
		break;
		case 'amount':
		case 'earnings':
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="jsForceNumb'.$addclass.'" maxlength="7">';
		break;
		case 'term':
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="jsForceNumb'.$addclass.'" maxlength="3">';
		break;
		case 'card':
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="jsForceNumb'.$addclass.'" maxlength="16">';
		break;
	
		case 'birth':
			$row[$field] = $this->get_time($row[$field], 'tiny');
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="birth'.$addclass.'" readonly>';
		break;
		case 'pay_date':
			$row[$field] = $this->get_time($row[$field], 'tiny', true);
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="payDate'.$addclass.'" readonly>';
		break;
		case 'paspdate':
			$row[$field] = $this->get_time($row[$field], 'tiny');
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="pasp'.$addclass.'" readonly>';
		break;
		
		case 'sex':
			$html = $this->get_select_tpl($field, 3, $row[$field], true, false, $addclass);
		break;
	
		case 'propiska':
			$html = $this->get_select_tpl($field, 3, $row[$field], false, false, $addclass);
		break;
	

		case 'seconddoc':
			$html = $this->get_select_tpl($field, 7, $row[$field], false, false, $addclass);
		break;
		case 'relative':
			$html = $this->get_select_tpl($field, 7, $row[$field], true, false, $addclass);
		break;
	
		case 'tariff':
			if ($field == 'tariff') {
				$html = '<option value="0"></option>';
			}

			$query2 = mysql_query("SELECT * FROM `wp_loans_tariffs` WHERE `archive`=0 ORDER BY `order`");
			while ($row2 = mysql_fetch_array($query2, MYSQL_ASSOC)) {
				$label = $row2['name'];
				
				if ($row2['descr']) {
					$label = $row2['descr'];
				}
				$selected = '';
				if ($row[$field] == $row2['id']) {
					$selected = ' selected';
				}
				$html.= '<option'.$selected.$locked.' value="'.$row2['id'].'">'.$label.' ('.(float)$row2['percent'].'%)</option>';
			}
			
			$query3 = mysql_query("SELECT `tariff` FROM `wp_loans` WHERE id={$_POST['id']}");
			@$row3 = mysql_fetch_array($query3, MYSQL_ASSOC);
			
			$query2 = mysql_query("SELECT * FROM `wp_loans_tariffs` WHERE `archive`=1 ORDER BY `order`");
			while ($row2 = mysql_fetch_array($query2, MYSQL_ASSOC)) {
				$label = $row2['name'];
				
				if ($row2['descr']) {
					$label = $row2['descr'];
				}
				$selected = '';
				if ($row[$field] == $row2['id']) {
					$selected = ' selected';
				}
				
				$disabled = '';
				if ($row3['tariff'] != $row2['id']) {
					$disabled = 'disabled';
				}
				
				$html.= '<option'.$selected.' '.$disabled.' value="'.$row2['id'].'">'.$label.' ('.(float)$row2['percent'].'%)</option>';
			}
			$html = '
			<select name="'.$field.'" class="'.$addclass.'">
				'.$html.'
			</select>';
		break;
	
		case 'address':
			$selected[$row[$field]] = ' checked';
			if ($row[$field] == 1) {
				$hidden = 'typ-hidden';
			}
			$html = '
				<p><input'.$selected[1].' type="radio" name="address" value="1" id="address1" class="checkbox'.$addclass.'">&nbsp;<label for="address1">По месту прописки</label></p>
				<p><input'.$selected[2].' type="radio" name="address" value="2" id="address2" class="checkbox'.$addclass.'">&nbsp;<label for="address1">Другой адресс</label></p>
				<p><textarea name="vat" class="'.$hidden.$addclass.' jsAddressArea">'.$row['vat'].'</textarea></p>
			';
			$addClass = 'jsSeparate';
		break;
		case 'branch':
			$query2 = mysql_query("SELECT * FROM `wp_loans_branches` ORDER BY `order`");
			while (@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC)) {
				$selected = '';
				if ($row[$field] == $row2['id']) {
					$selected = ' selected';
				}
				$html.= '<option value="'.$row2['id'].'" '.$selected.'>'.$row2['name'].'</option>';
			}
			
			$html =
			'<select name="branch" class="select req'.$addclass.'">
				<option value=""></option>
				'.$html.'
			</select>';
		break;
		case 'sb_id':
		case 'manager_id':
		case 'buh_id':
		case 'collector_id':
			$query2 = mysql_query("SELECT * FROM `wp_users` ORDER BY `display_name`");
			while (@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC)) {
				$selected = '';
				if ($row[$field] == $row2['ID']) {
					$selected = ' selected';
				}
				
				$nickname = $this->get_staff_name($row2['ID']);
				if (trim($nickname)) {
					$nickname .= ' ('.$row2['user_nicename'].')';
				} else {
					$nickname = $row2['user_nicename'];
				}
				
				$html.= '<option value="'.$row2['ID'].'" '.$selected.'>'.$nickname.'</option>';
			}
			
			$html =
			'<select name="'.$field.'" class="select'.$addclass.'">
				<option value=""></option>
				'.$html.'
			</select>';
		break;
	
		case 'manager_calls2':
			$checked = '';
			if ($row[$field]) {
				$checked = 'checked';
			}
			
			$html = '
			<p>'.$this->loans_translate('manager_calls2_comment').'</p>
			<p><textarea name="manager_calls2_comment" type="text">'.$this->form_text($row['manager_calls2_comment'], 'textarea').'</textarea></p>';
		break;
		
		case 'last_comment4':
		case 'last_comment4_full':
			if (($row['author'] != $this->user_id) and (!$this->is_admin)) {
				$_P->wrap_json_error('Вы не можете редактировать чужие комментарии.');
			}
			$html = '<p><textarea name="manager_calls2_comment" type="text">'.$this->form_text($row['text'], 'textarea').'</textarea></p>';
		break;
		case 'loan_method':
			$html = $this->get_select_tpl($field, 4, $row[$field], true, false, $addclass);
		break;
		case 'street1':
		case 'street2':
		case 'city1':
		case 'city2':
		case 'pasp_location':
			$html = '<input name="'.$field.'" type="text" maxlength="100" value="'.$this->form_text($row[$field], 'textarea').'" class="'.$addclass.'">';
		break;
		case 'house_number1':
		case 'apartment1':
		case 'house_number2':
		case 'apartment2':
		case 'corp1':
		case 'corp2':
			$html = '<input name="'.$field.'" type="text" maxlength="10" value="'.$this->form_text($row[$field], 'textarea').'" class="'.$addclass.'">';
		break;
		case 'street_type1':
		case 'street_type2':
			$html = $this->get_select_tpl($field, 31, $row[$field], true, true, $addclass);
		break;
		case 'force_percent':
			$html = '<input name="'.$field.'" type="text" maxlength="100" value="'.$this->form_text($row[$field], 'textarea').'" class="jsForceDouble'.$addclass.'" maxlength="5">';
		break;
		case 'structure1':
		case 'structure2':
			$html = '<input name="'.$field.'" type="text" maxlength="20" value="'.$this->form_text($row[$field], 'textarea').'" class="'.$addclass.'">';
		break;
		case 'purpose':
			$html = '<input name="'.$field.'" type="text" value="'.$this->form_text($row[$field], 'textarea').'" class="'.$addclass.'">';
		break;
	}
	
	if ($edit_btns) {
		$html.='<div class="edit-btns">
			<a href="#" class="jsEditSaveBtn typ-btn '.$addClass.'">Сохранить</a>
			<a href="#" title="В нижний регистр" class="jsLowerBtn typ-btn">&darr;</a>
			<a href="#" class="jsCancelSaveBtn selected typ-btn">Отменить<span style="display:none;"></span></a>
		</div>';
	}
	
	return $html;
}

// Возвращает верстку списка с переводом по заданному числу опций
function get_select_tpl($field, $options_numb, $selected_option = 1, $start_from_zero=false, $force_two_didgets=false, $addclass) {
	$i = 1;
	$step = 1;
	if ($start_from_zero) {
		$i = 0;
		$step = 0;
	}
	
	for ($i; $i<$options_numb+$step; $i++) {
		if (($force_two_didgets) and ($i > 0) and ($i < 10)) {
			$pre_i = 0;
		} else {
			$pre_i = '';
		}
		
		$selected[(int)$selected_option] = ' selected';
		$arr = $this->loans_translate_vals(array($field => $pre_i.$i));
		$label[$i] = $arr[$field];
		
		$options.= '<option'.$selected[$i].' value="'.$pre_i.$i.'">'.$label[$i].'</option>';
	}
	return '
	<select name="'.$field.'" class="'.$addclass.'">
		'.$options.'
	</select>';
}

function num2str_right($numb) {
	if (substr_count($numb, '.')) {
		list($rub,$kop) = explode('.', $numb);
		$out.= $this->num2str($rub, 'rub');
		if ((int)$kop) {
			$out.= ', '.$this->num2str($kop, 'kop');
		}
	} else {
		$out.= $this->num2str($numb, 'rub');
	}
	return($out);
}

/**
 * Возвращает сумму прописью
 * @author runcore
 * @uses morph(...)
 */
function num2str($num, $mode='rub') {
    $nul='ноль';
    $ten=array(
        array('','один','два','три','четыре','пять','шесть','семь', 'восемь','девять'),
        array('','одна','две','три','четыре','пять','шесть','семь', 'восемь','девять'),
    );
    $a20=array('десять','одиннадцать','двенадцать','тринадцать','четырнадцать' ,'пятнадцать','шестнадцать','семнадцать','восемнадцать','девятнадцать');
    $tens=array(2=>'двадцать','тридцать','сорок','пятьдесят','шестьдесят','семьдесят' ,'восемьдесят','девяносто');
    $hundred=array('','сто','двести','триста','четыреста','пятьсот','шестьсот', 'семьсот','восемьсот','девятьсот');
    $unit=array( // Units
        array('копейка' ,'копейки' ,'копеек',	 1),
        array('рубль'   ,'рубля'   ,'рублей'    ,0),
        array('тысяча'  ,'тысячи'  ,'тысяч'     ,1),
        array('миллион' ,'миллиона','миллионов' ,0),
        array('миллиард','милиарда','миллиардов',0),
    );
    //
    list($rub,$kop) = explode('.',sprintf("%015.2f", floatval($num)));
    $out = array();
	
    if (intval($rub)>0) {
        foreach(str_split($rub,3) as $uk=>$v) { // by 3 symbols
            if (!intval($v)) continue;
            $uk = sizeof($unit)-$uk-1; // unit key
            $gender = $unit[$uk][3];
            list($i1,$i2,$i3) = array_map('intval',str_split($v,1));
            // mega-logic
            $out[] = $hundred[$i1]; # 1xx-9xx
            if ($i2>1) $out[]= $tens[$i2].' '.$ten[$gender][$i3]; # 20-99
            else $out[]= $i2>0 ? $a20[$i3] : $ten[$gender][$i3]; # 10-19 | 1-9
            // units without rub & kop
            if ($uk>1) $out[]= $this->morph($v,$unit[$uk][0],$unit[$uk][1],$unit[$uk][2]);
        } //foreach
    }
    else $out[] = $nul;
	
	switch ($mode) {
		case 'rub':
			$name=$unit[1][2];
		break;
		case 'kop':
			$name=$unit[0][2];
		break;
	}
	
    $out[] = $this->morph(intval($rub), $unit[1][0],$unit[1][1],$name); // rub
    return trim(preg_replace('/ {2,}/', ' ', join(' ',$out)));
}


/**
 * Склоняем словоформу
 * @ author runcore
 */
function morph($n, $f1, $f2, $f5='') {
    $n = abs(intval($n)) % 100;
    if ($n>10 && $n<20) return $f5;
    $n = $n % 10;
    if ($n>1 && $n<5) return $f2;
    if ($n==1) return $f1;
    return $f5;
}

function docs_list($id, $show_additional = false) {
	
	if (!$show_additional) {
		$hidden = ' typ-hidden';
	}
	
	$id = $this->form_text($id, 'db');

	$query = mysql_query("SELECT `doc_numb` FROM `wp_loans_term_adds` WHERE loan_id=$id ORDER BY `doc_numb` DESC LIMIT 1");
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	$numb = $row['doc_numb'];
	
	$query = mysql_query("SELECT * FROM `wp_loans_term_adds` WHERE loan_id=$id");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		if ($row['doc_numb'] == $numb) {
			$del_btn = ' <span class="jsDeleteTerm fancy-tip no-selection" title="Удалить"></span>';
		}
		$term_add.='
		<tr class="additional">
			<td><a class="jsTermAddDoc" href="'.$this->site_url.'print-contract?id='.$id.'&doc=5&v='.$row['doc_numb'].'" target="_blank">Приложение №'.$row['doc_numb'].'. Дополнительное соглашение на продление ('.$row['term'].' дн.)</a>'.$del_btn.'</td>
		</tr>';
		$n++;
	}
	
	
	$query = mysql_query("SELECT `doc_numb` FROM `wp_loans_payments_graph` WHERE loan_id=$id ORDER BY `doc_numb` DESC LIMIT 1");
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	$numb = $row['doc_numb'];

	$arr = array();
	$query = mysql_query("SELECT * FROM `wp_loans_payments_graph` WHERE `loan_id`=$id ORDER BY `pay_date`");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$arr[$row['create_date']] = $row;
	}
	unset($row);
	
	$output = $table = '';
	$s = 0;
	foreach ($arr as $create_date => $row) {
		$s++;
		$content = '';
		
		$moderate = '';
		if ((current_user_can('loan_stop_percent')) and ($row['doc_numb'] == $numb)) {
			if (!$row['locked']) {
				$moderate = ' <a href="#" title="Удалить" class="jsDeleteGraph fancy-tip delete-btn" id="graph-'.$create_date.'"></a>';
			} else {
				if (!$row['closed']) {
					$moderate = ' <a href="#" title="Закрыть" class="jsCloseGraph fancy-tip close-btn" id="graph-'.$create_date.'"></a>';
				} else {
					$moderate = ' <a href="#" title="Открыть" class="jsReOpenGraph fancy-tip unclose-btn" id="graph-'.$create_date.'"></a>';
				}
			}
		}
		
		$pay_graph.= '
		<tr class="additional">
			<td><a class="" href="'.$this->site_url.'print-contract?id='.$id.'&doc=6&v='.$row['doc_numb'].'" target="_blank">Приложение №'.$row['doc_numb'].'. График выплат от '.$this->get_time($create_date, 'tiny_dig').$moderate.'</td>
		</tr>';
	}
	
	

	return '
	<h3>Документация:</h3>
	<table class="jsSingleTable">
		<tbody>
			<tr class="additional'.$hidden.'">
				<td><a href="'.$this->site_url.'print-contract?id='.$id.'&doc=1" target="_blank">Договор микрозайма</a></td>
			</tr>
			<tr>
				<td><a href="'.$this->site_url.'print-contract?id='.$id.'&doc=2" target="_blank">Согласие на КИ</a></td>
			</tr>
			<tr>
				<td><a href="'.$this->site_url.'print-contract?id='.$id.'&doc=3" target="_blank">Согласие на ПД</a></td>
			</tr>
			<tr class="additional'.$hidden.'">
				<td><a href="'.$this->site_url.'print-contract?id='.$id.'&doc=4" target="_blank">Приложение 1. Акт прил 2 список банков</a></td>
			</tr>
			<tr>
				<td><a href="'.$this->site_url.'print-contract2?id='.$id.'&doc=6" target="_blank">Расходный кассовый ордер</a></td>
			</tr>
			'.$term_add.'
			'.$pay_graph.'
		</tbody>
	</table>
	';
}

function recount_nav() {
	//$this->is_manager = false;
	//$is_buh = false;
	//$this->is_sb = false;
	if ($_POST['mode']) { // ajax
		$_GET['mode'] = $_POST['mode'];
	}
	
	if ($_POST['grel']) { // ajax
		$_GET['rel'] = $_POST['grel'];
	}

	if (($_GET['mode'] == 'single') and ($_GET['rel'])) {
		$class[$_GET['rel']] = ' active';
	} else {
		$class[$_GET['mode']] = ' active';
	}
	
	if ($this->is_sb) {
		$nav.= '
			<a href="'.$this->m_url.'?mode=new" class="'.$class['new'].' sb">
				'.$this->get_h1('new').' ('.$this->count_loans('new').')
			</a>
			<a href="'.$this->m_url.'?mode=counter" class="'.$class['counter'].' sb">
				'.$this->get_h1('counter').' ('.$this->count_loans('counter').')
			</a>
			<a href="'.$this->m_url.'?mode=success" class="a-left'.$class['success'].' sb">
				'.$this->get_h1('success').' ('.$this->count_loans('success').')
			</a>
			<a href="'.$this->m_url.'?mode=egosearch" class="'.$class['egosearch'].' sb">
				'.$this->get_h1('egosearch').' ('.$this->count_loans('egosearch').')
			</a>
			<a href="'.$this->m_url.'?mode=recall" class="'.$class['recall'].' sb">
				'.$this->get_h1('recall').' ('.$this->count_loans('recall').')
			</a>
			<a href="'.$this->m_url.'?mode=nbki" class="'.$class['nbki'].' sb">
				'.$this->get_h1('nbki').' ('.$this->count_loans('nbki').')
			</a>
			<a href="'.$this->m_url.'?mode=trash" class="'.$class['trash'].' sb">
				'.$this->get_h1('trash').' ('.$this->count_loans('trash').')
			</a>
			<a href="'.$this->m_url.'?mode=bans" class="'.$class['bans'].' sb">
				'.$this->get_h1('bans').' ('.$this->count_loans('bans').')
			</a>
			<a href="'.$this->m_url.'?mode=need_call" class="'.$class['need_call'].' sb">
				'.$this->get_h1('need_call').' ('.$this->count_loans('need_call').')
			</a>
		';
	}
	
	if ($this->is_manager) {
		$selected = '';
		$m = $this->count_loans('expired');
		if ($m > 0) {
			$selected = 'selected';
		}
		$nav.= '
			<a href="'.$this->m_url.'?mode=manage" class="'.$class['manage'].' manager">
				'.$this->get_h1('manage').' ('.$this->count_loans('manage').')
			</a>
			<a href="'.$this->m_url.'?mode=contract" class="'.$class['contract'].' manager">
				'.$this->get_h1('contract').' ('.$this->count_loans('contract').')
			</a>
			<a href="'.$this->m_url.'?mode=expired" class="'.$class['expired'].' manager  typ-btn '.$selected.'">
				'.$this->get_h1('expired').' ('.$m.')
			</a>
			<a href="'.$this->m_url.'?mode=debts" class="a-left'.$class['debts'].' manager">
				'.$this->get_h1('debts').' ('.$this->count_loans('debts').')
			</a>
			<a href="'.$this->m_url.'?mode=need_call_all" class="'.$class['need_call_all'].' manager">
				'.$this->get_h1('need_call_all').' ('.$this->count_loans('need_call_all').')
			</a>
			<a href="'.$this->m_url.'?mode=egosearch" class="'.$class['egosearch'].' manager">
				'.$this->get_h1('egosearch').' ('.$this->count_loans('egosearch').')
			</a>
			<a href="'.$this->m_url.'?mode=success" class="a-left'.$class['success'].' manager">
				'.$this->get_h1('success').' ('.$this->count_loans('success').')
			</a>
			<a href="'.$this->m_url.'?mode=recall" class="'.$class['recall'].' manager">
				'.$this->get_h1('recall').' ('.$this->count_loans('recall').')
			</a>
			<a href="'.$this->m_url.'?mode=trash2" class="'.$class['trash2'].' manager">
				'.$this->get_h1('trash2').' ('.$this->count_loans('trash', 'denied').')
			</a>
		';
	}
	
	if ($this->is_buh) {
		$selected = '';
		$m = $this->count_loans('exceed');
		if ($m > 0) {
			$selected = 'selected';
		}
		$nav.= '
			<a href="'.$this->m_url.'?mode=pay" class="a-left'.$class['pay'].' buh">
				'.$this->get_h1('pay').' ('.$this->count_loans('pay').')
			</a>
			<a href="'.$this->m_url.'?mode=debts" class="a-left'.$class['debts'].' buh">
				'.$this->get_h1('debts').' ('.$this->count_loans('debts').')
			</a>
			<a href="'.$this->m_url.'?mode=exceed" class="a-left'.$class['exceed'].' typ-btn buh '.$selected.'">
				'.$this->get_h1('exceed').' ('.$m.')
			</a>
			<a href="'.$this->m_url.'?mode=collected" class="a-left'.$class['collected'].' typ-btn buh ">
				'.$this->get_h1('collected').' ('.$this->count_loans('collected').')
			</a>
			<a href="'.$this->m_url.'?mode=success" class="a-left'.$class['success'].' buh">
				'.$this->get_h1('success').' ('.$this->count_loans('success').')
			</a>
			<a href="'.$this->m_url.'?mode=payments" class="a-left'.$class['payments'].' buh">
				'.$this->get_h1('payments').' ('.$this->count_loans('payments').')
			</a>
			<a href="'.$this->m_url.'?mode=graph" class="a-left'.$class['graph'].' buh">
				'.$this->get_h1('graph').' ('.$this->count_loans('graph').')
			</a>';
	}
	
	if ($this->is_collector) {
		$nav.= '
			<a href="'.$this->m_url.'?mode=collect" class="a-left'.$class['collect'].' collector">
				'.$this->get_h1('collect').' ('.$this->count_loans('collect').')
			</a>
			<a href="'.$this->m_url.'?mode=collected" class="a-left'.$class['collected'].' collector">
				'.$this->get_h1('collected').' ('.$this->count_loans('collected').')
			</a>
			<a href="'.$this->m_url.'?mode=uncollected" class="a-left'.$class['uncollected'].' collector">
				'.$this->get_h1('uncollected').' ('.$this->count_loans('uncollected').')
			</a>';
	}
	
	if ($this->is_admin) {
		$nav.= '
			<a href="'.$this->m_url.'?mode=control" class="a-left'.$class['control'].' admin">
				'.$this->get_h1('control').' ('.$this->count_loans('control').')
			</a>';
		if ($_COOKIE['show_admin_hidden']) {
			$nav.= '<span class="kaGhJKA"><a href="'.$this->site_url.'sb_cabinet/?mode=fake" class="typ-btn admin kaGhJKA-btn">Фейки ('.$this->count_loans('fake').')</a></span>';
		} else {
			$nav.= '<span class="kaGhJKA"></span>';
		}
	}
	
	$nav.= '<div class="clear"></div>
	
	<div class="section-separator"><p><span></span></p></div>
	<h1>'.$this->get_h1($_GET['mode']).'</h1>
	';
	
	return $nav;
}

function get_h1($mode) {
	switch ($mode) {
		default:
			$title = '';
		break;
		case 'bans':
			$title = 'Чёрный список';
		break;
		case 'new':
			$title = 'Заявки на первую проверку';
		break;
		case 'counter':
			$title = 'Заявки на контрольную проверку';
		break;
		case 'nbki':
			$title = 'Проверка в НБКИ';
		break;
		case 'egosearch':
			$title = 'В работе';
		break;
		case 'recall':
			$title = 'Отложенные';
		break;
		case 'trash':
			$title = 'Отклоненные';
		break;
		case 'trash2':
			$title = 'В корзине';
		break;
		case 'manage':
			$title = 'Предварительно одобрены';
		break;
		case 'contract':
			$title = 'Заявки на оформление';
		break;
		case 'expired':
			$title = 'Истекающие';
		break;
		case 'pay':
			$title = 'Заявки на выплату';
		break;
		case 'debts':
			$title = 'Выданные займы';
		break;
		case 'exceed':
			$title = 'Просроченные';
		break;
		case 'success':
			$title = 'Архив';
		break;
		case 'users':
			$title = 'Заемщики';
		break;
		case 'control':
			$title = 'Контрольная проверка';
		break;
		case 'collect':
			$title = 'Требуют взыскания';
		break;
		case 'collected':
			$title = 'Долг взыскан';
		break;
		case 'need_call':
			$title = 'Работа с просрочкой';
		break;
		case 'need_call_all':
			$title = 'Требуется звонок';
		break;
		case 'payments':
			$title = 'Платежи';
		break;
		case 'graph':
			$title = 'С графиком выплат';
		break;
		case 'uncollected':
			$title = 'Невозможно вернуть';
		break;
	}
	return $title;
}

function recount_list() {
	if ($_POST['mode']) { // ajax
		$_GET['mode'] = $_POST['mode'];
	}
	
	if ($_POST['view']) { // ajax
		$selected[$_POST['view']] = 'selected';
	}
	switch ($_GET['mode']) {
		default:
			return '';
		break;
		case 'new':
			$title = $this->loans_translate('validate');
			$o1 = $this->update_likes_status(0);
			$o2 = $this->update_likes_status(1);
			$options = '
			<option value="new" '.$selected['new'].'>'.$o1[0].' ('.$this->count_loans('new').')</option>
			<option value="allowed" '.$selected['allowed'].'>'.$o2[0].' ('.$this->count_loans('new', 'allowed').')</option>';
		break;
		case 'counter':
			$title = $this->loans_translate('validate');
			$o1 = $this->update_likes_status(2);
			$o2 = $this->update_likes_status(3);
			$options = '
			<option value="new" '.$selected['new'].'>'.$o1[0].' ('.$this->count_loans('counter').')</option>
			<option value="allowed" '.$selected['allowed'].'>'.$o2[0].' ('.$this->count_loans('counter', 'allowed').')</option>';
		break;
		case 'manage':
			$title = $this->loans_translate('validate');
			$o1 = $this->update_likes_status(1);
			$o2 = $this->update_likes_status(2);
			$options = '
			<option value="new" '.$selected['new'].'>'.$o1[0].' ('.$this->count_loans('manage').')</option>
			<option value="allowed" '.$selected['allowed'].'>'.$o2[0].' ('.$this->count_loans('manage', 'allowed').')</option>';
		break;
		case 'contract':
			$title = $this->loans_translate('validate');
			$o1 = $this->update_likes_status(3);
			$o2 = $this->update_likes_status(4);
			$options = '
			<option value="new" '.$selected['new'].'>'.$o1[0].' ('.$this->count_loans('contract').')</option>
			<option value="allowed" '.$selected['allowed'].'>'.$o2[0].' ('.$this->count_loans('contract', 'allowed').')</option>';
		break;
		case 'expired':
			$title = 'Должен вернуть через';
			$options = '
			<option value="0" '.$selected['0'].'>3 дня или меньше ('.$this->count_loans('expired', '0').')</option>
			<option value="1" '.$selected['1'].'>3 дня ('.$this->count_loans('expired', '1').')</option>
			<option value="2" '.$selected['2'].'>2 дня ('.$this->count_loans('expired', '2').')</option>
			<option value="3" '.$selected['3'].'>1 день ('.$this->count_loans('expired', '3').')</option>
			<option value="4" '.$selected['4'].'>Просрочил выплату ('.$this->count_loans('expired', '4').')</option>';
		break;
		case 'egosearch':
			$title = 'Привязано к роли';
			$options = '<option value="new" '.$selected['new'].'>Любая роль ('.$this->count_loans('egosearch').')</option>';
			if ($this->is_sb) {
				$options .= '<option value="sb" '.$selected['sb'].'>Служба Безопасности ('.$this->count_loans('egosearch', 'sb').')</option>';
			}
			if ($this->is_manager) {
				$options .= '<option value="manager" '.$selected['manager'].'>Менеджер ('.$this->count_loans('egosearch', 'manager').')</option>';
			}
			if ($this->is_buh) {
				$options .= '<option value="buh" '.$selected['buh'].'>Бухгалтер ('.$this->count_loans('egosearch', 'buh').')</option>';
			}
		break;
		case 'trash':
			$title = $this->loans_translate('validate');
			$o2 = $this->update_likes_status(8);
			$o3 = $this->update_likes_status(9);
			$options = '
			<option value="new" '.$selected['new'].'>Любой ('.$this->count_loans('trash').')</option>
			<option value="denied" '.$selected['denied'].'>В корзине ('.$this->count_loans('trash', 'denied').')</option>
			<option value="denied1" '.$selected['denied1'].'>'.$o2[0].' ('.$this->count_loans('trash', 'denied1').')</option>
			<option value="denied2" '.$selected['denied2'].'>'.$o3[0].' ('.$this->count_loans('trash', 'denied2').')</option>';
		break;
		case 'exceed':
			$title = 'Отобразить';
			$options = '
			<option value="new" '.$selected['new'].'>'.$this->get_h1('exceed').' ('.$this->count_loans('exceed').')</option>
			<option value="collect" '.$selected['collect'].'>Отправлено коллекторам ('.$this->count_loans('exceed', 'collect').')</option>';
		break;
		case 'collect':
			$title = 'Отобразить';
			$options = '
			<option value="new" '.$selected['new'].'>'.$this->get_h1('collect').' ('.$this->count_loans('collect').')</option>
			<option value="collect" '.$selected['collect'].'>Отправлено менеджерам ('.$this->count_loans('collect', 'collect').')</option>';
		break;
	}

	return '
	<div class="views">
		'.$title.':
		<select id="jsView">
			'.$options.'
		</select>
	</div>';
}

function check_mysql_error() {
	$error = mysql_error();
	if ($error) {
		$this->wrap_json_error($error);
	}
}

function check_ajax_rights($key, $bind=true, $lock_admin=false, $show_alert=true) {
	$check = array();
	$check_any = false;
	$staff = array('sb', 'manager', 'buh', 'collector');
	
	foreach ($staff as $role) {
		if (current_user_can('loan_view_'.$role)) {
			// У группы менеджеров есть нужный ключ?
			if ($this->check_group_key($role, $key)) {
				// Это тот же менеджер? Привязать, если пусто.
				$check[$role] = $this->check_bind($_POST['id'], $role, $bind);
			}
			if (($this->is_admin) and (!$lock_admin)) {
				$check[$role] = true;
			}
			if ($check[$role]) {
				$check_any = true;
			}
		}
	}

	if (!$check_any) {
		if (($show_alert)) {
			$this->wrap_json_error('Эта функция вам недоступна, поскольку заявку уже обрабатывает другой сотрудник.');
		}
		return false;
	}
	return true;
}

function check_bind($id, $role, $bind=true) {
	// Если текущий юзер - СБ
	$query = mysql_query("SELECT u.`{$role}_id`, l.`user_id` FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE u.id = l.user_id AND l.id=$id");
	$row = mysql_fetch_array($query, MYSQL_ASSOC);
	
	if ($row[$role.'_id']) {
		if ($row[$role.'_id'] == $this->user_id) {
			return true;
		} else {
			return false;
		}
	} else {
		if ($bind) {
			mysql_query("UPDATE `wp_loans_users` SET `{$role}_id`='$this->user_id' WHERE id={$row['user_id']}");
			return true;
		} else {
			return false;
		}
	}
}

function check_group_key($role, $key) {
	static $groups;
	// TODO: Разобраться с этим костылем
	if (!$groups) {
		$query = mysql_query("SELECT `option_value` FROM `wp_options` WHERE `option_name`='wp_user_roles'");
		$row = mysql_fetch_array($query, MYSQL_ASSOC);
		$groups = unserialize($row['option_value']);
	}
	
	if ($groups[$role]['capabilities'][$key]) {
		return true;
	} else {
		return 0;
	}
}

function show_polls($id) {
// Опросы
// Вместо user_id используем id займа, ибо тут нужно привязывать опрос к займу, а не к юзеру
	require_once($this->site_dir.'wp-admin/includes/plugin.php');
	if (is_plugin_active('polls/admin.php')) {
		$id = $this->form_text($id, 'db');
		$sb_locked = $manager_locked = true;
		
		// Юзер СБ
		if (current_user_can('loan_poll_phone')) {
			//if ($this->check_group_key('sb', 'loan_poll_phone')) {
			// Это тот же СБ?
			
			$query = mysql_query("SELECT `user_id` FROM `wp_loans` WHERE id=$id");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			$query = mysql_query("SELECT `sb_id` FROM `wp_loans_users` WHERE id={$row['user_id']}");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			if (($row['sb_id'] == $this->user_id) or ($row['sb_id'] == 0) or ($this->is_admin)) {
				$sb_locked = false;
			}
			//// Показываем кнопку админу только если заявка уже присвоена
			//if (($this->is_admin) and ($row['sb_id'])) {
			//	$sb_check = true;
			//}
			//
			//if ($sb_check) {
				require_once($this->site_dir.'wp-content/plugins/polls/functions.php');
				$_TEMP = new polls_functions();
				$poll.= $_TEMP->output_poll_form(7, $id, false, $sb_locked);
				unset($_TEMP);
			//}
			//}
		} else {
			// Если юзер не может учавствовать в этом опросе, но он Менеджер - выводим только для чтения
			if ($this->is_manager) {
				require_once($this->site_dir.'wp-content/plugins/polls/functions.php');
				$_TEMP = new polls_functions();
				$poll.= $_TEMP->output_poll_form(7, $id, false, true);
				unset($_TEMP);
			}
		}
		
		// Юзер менеджер
		if (current_user_can('loan_poll_visual')) {
		
			// Это тот же Менеджер?
			$query = mysql_query("SELECT `user_id` FROM `wp_loans` WHERE id=$id");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			$query = mysql_query("SELECT `manager_id` FROM `wp_loans_users` WHERE id={$row['user_id']}");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			if (($row['manager_id'] == $this->user_id) or ($row['manager_id'] == 0) or ($this->is_admin)) {
				$manager_locked = false;
			}
			//// Показываем кнопку админу только если заявка уже присвоена
			//if (($this->is_admin) and ($row['manager_id'])) {
			//	$manager_check = true;
			//}
			//
			//if ($manager_check) {
				require_once($this->site_dir.'wp-content/plugins/polls/functions.php');
				$_TEMP = new polls_functions();
				$poll.= $_TEMP->output_poll_form(8, $id, false, $manager_locked);
				unset($_TEMP);
			//}
		} else {
			// Если юзер не может учавствовать в этом опросе, но он СБ - выводим только для чтения
			if ($this->is_sb) {
				require_once($this->site_dir.'wp-content/plugins/polls/functions.php');
				$_TEMP = new polls_functions();
				$poll.= $_TEMP->output_poll_form(8, $id, false, true);
				unset($_TEMP);
			}
		}
		return '<div class="pollsAjaxContainer">'.$poll.'</div>';
	}
}

function wrap_json_error($message, $error=1) {
	$arr['message'] = $message;
	$arr['error'] = $error;
	echo json_encode($arr);
	die;
}

function out_search_form() {
	if ($_COOKIE['global_search']) {
		$checked = ' checked';
	}
	if (current_user_can('loan_view_all')) {
		$global = '
		<div class="search-global-wrap">
			<input type="checkbox" class="checkbox jsSearchGlobal" id="search-global"'.$checked.'>
			<label for="search-global">Искать везде</label>
		</div>';
	}
	
	$output = '
	<div id="search">
		<div>
			Поиск:
			<input type="text" class="search">
		</div>
		'.$global.'
	</div>
	<div class="clear"></div>';
	
	return $output;
}

function count_loans($mode, $view='new') {
	$where = $this->get_where($mode, $view);
	$db = 'l';
	if (in_array($mode, array('users', 'bans'))) {
		$db = 'u';
		$uniq = true;
	}
	
	$arr = array();
	$query = mysql_query("SELECT $db.`id` $where");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$arr[] = $row['id'];
	}
	
	if ($uniq) {
		$arr = array_unique($arr);
	}
	
	return count($arr);
}

function show_calendars($mode) {
	$label = 'Календарь';
	$rel = 'Скрыть';
	
	if ($_COOKIE['date-hidden']) {
		$class='typ-hidden';
	} else {
		$tmp = $label;
		$label = $rel;
		$rel = $tmp;
	}
	
	$selected[$mode] = 'selected';
	
return '
	<div class="calendars-wrap">
		<div class="jsFilterCollapseWrap '.$class.'">
			<fieldset class="date-filter">
				<legend>Режим</legend>
				<a href="#" class="jsOneDay typ-btn">Один день</a>
				<a href="#" class="jsPeriod typ-btn">От - до</a>
				<a href="#" class="jsAllDays typ-btn active">Все дни</a>
			</fieldset>
			<div class="datepicker-buh datepicker-buh1"></div>
			
			<div class="date-descr">
				<div class="datepicker-buh datepicker-buh2" style="display:none;"></div>
				<div class="clear"></div>
			</div>
		</div>
		<a class="jsFilterCollapse typ-btn" rel="'.$rel.'">'.$label.'</a>
		<div class="calendar-tip">
			<span>Подсвечивается:</span>
			<select>
				<option value="date" '.$selected['date'].'>Дата подачи заявки</option>
				<option value="pay_date" '.$selected['pay_date'].'>Дата выдачи займа</option>
				<option value="back_date" '.$selected['back_date'].'>Крайний день возврата</option>
				<option value="back_date_real" '.$selected['back_date_real'].'>Дата закрытия займа</option>
				<option value="need_call_date" '.$selected['need_call_date'].'>Требуется звонок</option>
			</select>
		</div>
	</div>
	';
}

// Возвращает таблицу юзера
function check_session($pick='*') {
	session_start();
	
	if ((!$_SESSION['mfunny-user-login']) or (!$_SESSION['mfunny-user-auth'])) {
		
		// пытаемся восстановить сессию из куков
		if (($_COOKIE['mfunny-user-login']) and ($_COOKIE['mfunny-user-auth'])) {
			$_SESSION['mfunny-user-login'] = $_COOKIE['mfunny-user-login'];
			$_SESSION['mfunny-user-auth'] = $_COOKIE['mfunny-user-auth'];
		} else {
			// в куках пусто
			return false;
		}
	}
	
	$login = $this->form_text($_SESSION['mfunny-user-login'], 'db');
	$auth = $this->form_text($_SESSION['mfunny-user-auth'], 'db');
	
	// проверяем есть ли такая сессия в базе
	$query = mysql_query("SELECT $pick FROM `wp_loans_users` WHERE `email`='$login' AND `auth`='$auth' LIMIT 1");
	if (mysql_num_rows($query) > 0) {
		// возвращаем инфу о юзере
		$row = mysql_fetch_array($query, MYSQL_ASSOC);
		return $row;
	}
	// нет сессии
	return false;
}

function send_email($mail, $msg, $subject, $sender='noreply@moneyfunny.ru', $filepath=false) {
	$from = 'Мани Фанни';
	$EOL = "\r\n"; // ограничитель строк, некоторые почтовые сервера требуют \n - подобрать опытным путём
	
	if ($filepath) {
		$fp = fopen($filepath,"rb");
		if (!$fp) {
			@file_put_contents($filepath.'.log', 'Файл не найден');
			return;
		}
		$file = fread($fp, filesize($filepath));
		fclose($fp);
		
		$filename = basename($filepath);
		$boundary= "--".md5(uniqid(time()));// любая строка, которой не будет ниже в потоке данных.
		
		$headers = "MIME-Version: 1.0;$EOL";
		$headers.= "Content-Type: multipart/mixed; boundary=\"$boundary\"$EOL";
		$headers.= 'From: '.$from.' <'.$sender.'>'.$EOL;
		$headers.= "Reply-To: $sender$EOL";
		$headers.= "X-Mailer: PHP/".phpversion().$EOL;
		
		$multipart  = "--$boundary$EOL";
		$multipart .= "Content-Type: text/html; charset=utf-8$EOL";
		$multipart .= "Content-Transfer-Encoding: base64$EOL";
		$multipart .= $EOL; // раздел между заголовками и телом html-части 
		$multipart .= chunk_split(base64_encode($msg));
		
		$multipart .="$EOL--$boundary$EOL";
		$multipart .= "Content-Type: application/octet-stream; name=\"$filename\"$EOL";
		$multipart .= "Content-Transfer-Encoding: base64$EOL";
		$multipart .= "Content-Disposition: attachment; filename=\"$filename\"$EOL";
		$multipart .= $EOL; // раздел между заголовками и телом прикрепленного файла 
		$multipart .= chunk_split(base64_encode($file));
		
		$multipart .= "$EOL--$boundary--$EOL";
		
		return mail($mail, $subject, $multipart, $headers);
	} else {
		$headers = "MIME-Version: 1.0;$EOL";
		$headers.= "Content-Type: text/html; charset=utf-8$EOL";
		$headers.= 'From: '.$from.' <'.$sender.'>'.$EOL;
		$headers.= "Reply-To: $sender$EOL";
		$headers.= "X-Mailer: PHP/".phpversion().$EOL;
		
		return mail($mail, $subject, $msg, $headers);
	}
}

function wrap_fe_error($text) {
	return $text;
}

function loan_section_tpl_fe($capture, $fields, $row, $colspan=true) {
	$colsp2 = ' colspan="2"';
	if ($colspan) {
		$colsp = ' colspan="2"';
		$colsp2 = ' colspan="3"';
	}
	$fields = explode(',', $fields);
	$n = 1;
	foreach ($fields as $field) {
		$n++;
	    $field = trim($field);
		$content.='
		<tr>
			<td>'.$this->loans_translate($field).'</td>
			<td'.$colsp.'>'.$row[$field].'</td>
		</tr>
		';
	}
	$content = '
	<tr>
		<td'.$colsp2.' class="table-capt">'.$capture.'</td>
	</tr>
	'.$content;
	$arr[0] = $content;
	$arr[1] = $n;
	return $arr;
}

function secure_array($arr) {
	foreach ($arr as $key => $value) {
		$arr[$key] = $this->form_text($value, 'db');
	}
	return $arr;
}

/* Запись займа в БД
------------------------------------------------------------------------------*/
function prepare_for_db($arr, $old='') {
    if (strlen($arr['password1'])) {
		$pass1_bac = $arr['password1'];
	}
	if (strlen($arr['password2'])) {
		$pass2_bac = $arr['password2'];
	}
	if (strlen($arr['phone1'])) {
		$phone_bac = $arr['phone1'];
	}
	
	foreach ($arr as $key => $value) {
		$value = trim($value);
        
		switch ($key) {
			case 'tariff':
				switch ($value) {
					case 1:
					case 'money15':
						$value = 1;
					break;
					case 2:
					case 'money30':
						$value = 2;
					break;
					case 3:
					case 'money150':
						$value = 3;
						//$arr['amount'] = $arr['credit'];
						//$arr['term'] = 0;
					break;
					default:
						$value = preg_replace("/[^0-9]/", '', $value);
					break;
				}
				
				if ($value != 13) { // стартап
					$arr['earnings'] = 0;
				}
			break;
			case 'amount':
			case 'term':
			case 'earnings':
				$value = preg_replace("/[^0-9]/", '', $value);
			break;
			case 'paspser':
			case 'paspnom':
                if (!preg_match("'^[0-9]$'is", $value)) {
					$_POST[$key.'_error'] = 1;
				}
                $value = preg_replace("/[^0-9]/", '', $value);
            break;
			case 'sex':
                $value = preg_replace("/[^0-9]/", '', $value);
				switch ($value) {
					default:
						$value = 0;
					break;
					case 1:
					case 'Женский':
						$value = 1;
					break;
					case 2:
					case 'Мужской':
						$value = 2;
					break;
				}
			break;
			case 'propiska':
				switch ($value) {
					default:
						$value = 0;
					break;
					case 1:
					case 'Москва':
						$value = 1;
					break;
					case 2:
					case 'Московская область':
						$value = 2;
					break;
					case 3:
					case 'Другое':
						$value = 3;
					break;
				}
			break;
			case 'address':
				switch ($value) {
					default:
						$value = 0;
					break;
					case 'fact':
						$value = 1;
					break;
					case 'another':
						$value = 2;
					break;
				}
			break;
			case 'seconddoc':
                $value = preg_replace("/[^0-9]/", '', $value);
                switch ($value) {
                    default:
                        $value = 0;
                    break;
                    case 1:
                    case 'Вод. удостоверение':
                        $value = 1;
                    break;
                    case 2:
                    case 'Заграничный паспорт':
                        $value = 2;
                    break;
                    case 3:
                    case 'Свидетельство ИНН':
                        $value = 3;
                    break;
                    case 4:
                    case 'Пенсионное удостоверение':
                        $value = 4;
                    break;
                    case 5:
                    case 'Удостоверение генерала':
                        $value = 5;
                    break;
                    case 6:
                    case 'Cвидетельство о регистрации ТС':
                        $value = 6;
                    break;
                    case 7:
                    case 'Карточка пенсионного страхования':
                        $value = 7;
                    break;
                }
			break;
			case 'relative':
				if (!preg_match("'^[0-9]$'is", $value)) {
					$value = 0;
				}
			break;
	
				$value = preg_replace("/[^0-9]/", '', $value);
			break;
			case 'email':
				$value = strtolower($value);
			break;
			case 'birth':
			case 'paspdate':
				list($day, $month, $year) = explode('/', $value);
				$value = $year.'-'.$month.'-'.$day;
                if ($value=='--') {
					$value = '';
				}
			break;
			case 'fname':
			case 'sname':
			case 'tname':
			case 'relsname':
			case 'reltname':
			case 'relfname':
				$value = $this->prop_name_translate($value);
			break;
		}
		$arr[$key] = $value;
	}
	
	unset($arr['credit']); // зачем второе поле?
	
	if ($arr['norels']) {
		$arr['relative'] = 0;
		$arr['relsname'] = '';
		$arr['reltname'] = '';
		$arr['relphone'] = '';
		$arr['relfname'] = '';
	}
	$arr = $this->secure_array($arr);
	if (strlen($arr['password1'])) {
		$arr['password1'] = $pass1_bac;
	}
	if (strlen($arr['password2'])) {
		$arr['password2'] = $pass2_bac;
	}
	if (strlen($arr['phone1'])) {
		$arr['phone1_bac'] = $phone_bac;
	}
	
	return $arr;
}

function prop_name_translate($name) {
	$name = trim($name);
	$arr = explode('-', $name);
	foreach ($arr as $key => $value) {
		$value = trim($value);
		if ($value) {
			$first = mb_substr($value, 0, 1);
			$rest = mb_substr($value, 1);
			$first = mb_strtoupper($first);
			$rest = mb_strtolower($rest);
			$arr[$key] = $first.$rest;
		}
	}
	$name = implode('-', $arr);
	return $name;
}


function loan_write_db($arr, $mode='both') {
	$msg_error = 'При отправке заявки с калькулятора микрозаймов произошла ошибка записи в базу данных<br>
	Заявка на займ отправлена вам на почту, но не сохранена в БД<br>
	Для устранения проблемы передайте своему программисту или менеджеру проекта следующую информацию:<br><br>
	Скрипт: wp-content\plugins\mfunny-cabs\functions.php, функция loan_write_db<br><br>';
	
	// Создание нового юзера, если в БД ещё нет такого
	if (($mode == 'both') or ($mode == 'user')) {
		
		if (($arr['rules1'] != 'on') or ($arr['rules2'] != 'on') or ($arr['rules3'] != 'on')) {
			$this->wrap_json_error('Вы должны согласиться с условиями предоставления займа.');
		}

		$paspser = $this->form_text($arr['paspser'], 'db');
		$paspnom = $this->form_text($arr['paspnom'], 'db');
		$query = mysql_query("SELECT `id`, COUNT(`id`) AS `numb` FROM `wp_loans_users` WHERE `paspser`='$paspser' AND `paspnom`='$paspnom' AND `fake`=0");
		@$row = mysql_fetch_array($query, MYSQL_ASSOC);
		if ($row['numb']) {
			$this->wrap_json_error('Эти паспортные данные уже есть в нашей базе.');
		}
		
		$email = $this->form_text($arr['email'], 'db');
		$query = mysql_query("SELECT `id`, COUNT(`id`) AS `numb` FROM `wp_loans_users` WHERE `email`='$email' AND `fake`=0");
		@$row = mysql_fetch_array($query, MYSQL_ASSOC);
		if ($row['numb']) {
			$this->wrap_json_error('Пользователь с такой электронной почтой уже есть в нашей базе.');
		}
		
		$real['sex'] = $arr['sex'];
		$real['email'] = $arr['email'];
		
		// Тут не нужен form_text
		$pass_bac = $arr['password1'];
		$arr['password1'] = md5($arr['password1']);
		$arr['auth'] = $this->rand_string(255);
		
		$query = "INSERT INTO `wp_loans_users` (
		`auth`,
		`password`,
		`sname`,
		`propiska`,
		`fname`,
		`birth`,
		`tname`,
		`sex`,
		`paspser`,
		`paspnom`,
		`fulladdr`,
		`birthaddr`,
		`address`,
		`vat`,
		`paspkem`,
		`paspdate`,
		`seconddoc`,
		`phone1`,
		`phone2`,
		`email`,
		`relative`,
		`relsname`,
		`reltname`,
		`relphone`,
		`relfname`,
		`branch`,
		`pasp_location`,
		`city1`,
		`street_type1`,
		`street1`,
		`corp1`,
		`structure1`,
		`house_number1`,
		`apartment1`,
		`city2`,
		`street_type2`,
		`street2`,
		`corp2`,
		`structure2`,
		`house_number2`,
		`apartment2`
			) VALUES (
		'{$arr['auth']}',
		'{$arr['password1']}',
		'{$arr['sname']}',
		'',
		'{$arr['fname']}',
		'{$arr['birth']}',
		'{$arr['tname']}',
		'{$arr['sex']}',
		'{$arr['paspser']}',
		'{$arr['paspnom']}',
		'',
		'{$arr['birthaddr']}',
		'{$arr['address']}',
		'',
		'{$arr['paspkem']}',
		'{$arr['paspdate']}',
		'{$arr['seconddoc']}',
		'{$arr['phone1']}',
		'{$arr['phone2']}',
		'{$arr['email']}',
		'{$arr['relative']}',
		'{$arr['relsname']}',
		'{$arr['reltname']}',
		'{$arr['relphone']}',
		'{$arr['relfname']}',
		'{$arr['branch']}',
		'{$arr['pasp_location']}',
		'{$arr['city1']}',
		'{$arr['street_type1']}',
		'{$arr['street1']}',
		'{$arr['corp1']}',
		'{$arr['structure1']}',
		'{$arr['house_number1']}',
		'{$arr['apartment1']}',
		'{$arr['city2']}',
		'{$arr['street_type2']}',
		'{$arr['street2']}',
		'{$arr['corp2']}',
		'{$arr['structure2']}',
		'{$arr['house_number2']}',
		'{$arr['apartment2']}'
		)";
		mysql_query($query);
		$error = mysql_error();
		if ($error) {
			$msg = $msg_error.'mysql_error: '.$error.' <br><br>
			query: '.nl2br($query);
			$this->send_email('maestro.magnifico@mail.ru', $msg, 'Ошибка при записи в базу данных');
			return;
		}
		$id = mysql_insert_id();
		
		// Отправка пароля юзеру
		if ($real['sex'] == 1) {
			$fio = 'Уважаемая ';
		} else {
			$fio = 'Уважаемый ';
		}
		
		$fio .= $arr['sname'].' '.$arr['fname'].' '.$arr['tname'];
		
		$msg2 = $fio.', вы зарегистрировались на сайте moneyfunny.ru<br>
		Состояние своего займа вы можете отслеживать в личном кабинете заемщика.<br>
		Ваш пароль для входа в личный кабинет: <b>'.$pass_bac.'</b><br>
		Ссылка на личный кабинет: <a href="https://moneyfunny.ru/creditor_cabs/">https://moneyfunny.ru/creditor_cabs/</a><br><br>
		С уважением, микрофинансовая организация ООО "МАНИ ФАННИ".';

		$this->send_email(trim($real['email']), $msg2, 'Ваш пароль от личного кабинета');
		//$this->send_email('maestro.magnifico@mail.ru', $msg2, 'Ваш пароль от личного кабинета');
		$this->send_sms_simple('Ваш пароль от личного кабинета Мани Фанни: '.$pass_bac, $arr['phone1_bac']);
		
		// Запись сессии
		session_start();
		$_SESSION['mfunny-user-login'] = trim($real['email']);
		setcookie('mfunny-user-login', trim($real['email']), time()+86400, '/');
		
		$_SESSION['mfunny-user-auth'] = $this->form_text($arr['auth']);
		setcookie('mfunny-user-auth', $arr['auth'], time()+86400, '/');
	}
	
	// Займ
	if (($mode == 'both') or ($mode == 'loan')) {
		if ($mode == 'loan') {
			$id = $arr['user_id'];
		}
		
		$id_check = $this->form_text($id, 'db');
		$query = mysql_query("SELECT `id`, COUNT(`id`) AS `numb` FROM `wp_loans_users` WHERE `id`='$id_check'");
		@$row = mysql_fetch_array($query, MYSQL_ASSOC);
		if (!$row['numb']) {
			$this->send_email('maestro.magnifico@mail.ru', 'Техническая ошибка. Пользователь с id '.$id.' не найден в базе данных', 'Ошибка при записи в базу данных');
			$this->wrap_json_error('Техническая ошибка. Пользователь с id '.$id.' не найден в базе данных. Письмо с отчетом об ошибке было отправлено администратору.');
		}
		
		$server = $_SERVER;
		if ($_POST['lastref']) {
			$server['HTTP_REFERER'] = base64_decode($_POST['lastref']);
		}
		$server = serialize($server);
		$server = base64_encode($server);
		$server = mysql_real_escape_string($server);
		
		$arr['date'] = gmdate('Y-m-d H:i:s'); // TIMESTAMP по Гринвичу
		$query = "INSERT INTO `wp_loans` (
		`user_id`,
		`tariff`,
		`amount`,
		`term`,
		`earnings`,
		`date`,
		`idea`,
		`server`
			) VALUES (
		'$id',
		'{$arr['tariff']}',
		'{$arr['amount']}',
		'{$arr['term']}',
		'{$arr['earnings']}',
		'{$arr['date']}',
		'{$arr['idea']}',
		'$server'
		)";
		mysql_query($query);
		
		unset($error);
		$error = mysql_error();
		if ($error) {
			$msg = $msg_error.'mysql_error: '.$error.'<br><br>
			query: '.$query;
			$this->send_email('maestro.magnifico@mail.ru', $msg, 'Ошибка при записи в базу данных');
			return;
		}
		
		// Отправка инфы о займе на почту менеджеру (не мой говнокод)
		$id = mysql_insert_id();
		$this->send_new_loan_report($id);
	}
}

function send_new_loan_report($id) {
	$data = $this->get_main_row($id);
	$data = $this->loans_translate_vals($data);
	
	//шаг2
	$personal_data = "----------Персональные данные----------<br>";
	$personal_data .= "Прописка: " . $data['fulladdr'] . "<br>";
	$personal_data .= "День рождения: " . $data['birth'] . "<br>";
	$personal_data .= "Пол: " . $data['sex'] . "<br>";

	//шаг3
	$pasport_data = "----------Паспортные данные----------<br>";
	$pasport_data .= "Серия: " . $data['paspser'] . "<br>";
	$pasport_data .= "Номер: " . $data['paspnom'] . "<br>";
	$pasport_data .= "Выдан: " . $data['paspkem'] . " " . $data['paspdate'] . "<br>";
	$pasport_data .= "Место рождения: " . $data['birthaddr'] . "<br>";
	
	$pasport_data .= "Фактически проживает: " . $data['address'] . "<br>";
	$pasport_data .= "Второй документ: " . $data['seconddoc'] . "<br>";

	//шаг4
	$contacts .= "----------Контактная информация----------<br>";
	$contacts .= "Телефон 1: " . $data['phone1'] . "<br>";
	$contacts .= "Телефон 2: " . $data['phone2'] . "<br>";
	$contacts .= "Email: " . $data['email'] . "<br>";

	$rules = '';
	$statement = array( 1 => 'Я являюсь гражданином РФ',
						2 => 'Я имею постоянную регистрацию РФ',
						3 => 'Согласен на обработку моих данных');
	//правила
	for ($i=1; $i <= 3; $i++) {
		if(isset($data['rules'.$i])) {
			$rules .= $statement[$i] . " +<br>";
		} else {
			$rules .= $statement[$i] . " -<br>";
		}
	}

	//если все проверено и ошибок нет
	$msg     = 'Ссылка на займ: <a href="'.$this->site_url.'sb_cabinet/?mode=single&id='.$id.'">'.$this->site_url.'sb_cabinet/?mode=single&id='.$id.'</a><br>';
	$msg     .= $data['sname'] . " " . $data['fname'] . " " . $data['tname'] . "<br>";
	$msg     .= "хочет занять " . $data['amount'] . " на срок " . $data['term'] . "<br>"; 
	$msg     .= $personal_data . "<br>";
	$msg     .= $pasport_data . "<br>";
	$msg     .= $contacts . "<br>";
	$msg     .= "----------Согласие с условиями----------<br>" . $rules;

	//echo '<pre>'; var_export($msg); echo '</pre>'; // отладка
	
	$this->send_email('moneyfunny01@gmail.com', $msg, "Заявка от калькулятора микрозаймов");
	//$this->send_email('maestro.magnifico@mail.ru', $msg, "Заявка от калькулятора микрозаймов");
}

function translate_validate_4user($validate) {
switch ($validate) {
		case 0:
			$validate = '<span class="text-orange">Заявка на рассмотрении</span>';
		break;
		case 1:
		case 2:
		case 3:
			$validate = '<span class="text-orange">Предварительно одобрен</span>';
		case 4:
			$validate = '<span class="text-green">Одобрен</span>';
		break;
		break;
		case 5:
			$validate = '<span class="text-green">Займ выдан</span>';
		break;
		case 6:
			$validate = '<span class="text-green">Займ закрыт</span>';
		break;
		case 8:
		case 9:
			$validate = '<span class="text-red">К сожалению, в выдаче займа отказано</span>';
		break;
		case 10:
		case 11:
			$validate = '<span class="text-orange">Контрольная проверка</span>';
		break;
		case 12:
		case 13:
			// у коллекторов
			$validate = '<span class="text-green">Займ выдан</span>';
		break;
	}
	return $validate;
}

function get_payments($loan_id, $for_creditor=false, $failed=false, $page=1, $and="") {
	$loan_id = $this->form_text($loan_id, 'db');
	
	if (!$failed) {
		// Удачные платежи
		require_once($this->site_dir.'wp-content/plugins/robokassa/robokassa.php');
		robokassa_update_loan($loan_id);
		
		$per_page = 5;
		$query = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans_payments` WHERE `validate`=100 $and AND loan_id=$loan_id");
		$row = mysql_fetch_array($query);
		$num_pages=ceil($row['numb']/$per_page);
		$limit = $per_page*($page-1).', '.$per_page;
		
		$query = mysql_query("SELECT *, `date` AS `payment_date` FROM `wp_loans_payments` WHERE `validate`=100 $and AND loan_id=$loan_id ORDER BY `date` DESC LIMIT $limit");
		
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$row = $this->loans_translate_vals($row);
			$moderate = '';
			if ((!$for_creditor) and (current_user_can('loan_edit_payment')) and ($row['user'] != 'user')) {
				$moderate = '<span class="delete-btn jsDeletePayment fancy-tip" title="Удалить" id="payment-'.$row['id'].'"></span>';
			}
			
			$content.='
			<tr>
				<td>'.$row['amount'].'</td>
				<td>'.$row['pay_method'].'</td>
				<td>'.$row['ass_numb'].'</td>
				<td>'.$row['date_stats'].'</td>
				<td>'.$row['payment_date'].'</td>
				<td>'.$moderate.'</td>
			</tr>
			';
		}
		
		if (!$content) {
			$content = '
			<div class="white-wrap">Нет платежей.</div>
			';
		} else {
			$content = '
			<table>
				<thead>
					<tr>
						<td class="table-capt">Сумма</td>
						<td class="table-capt">Способ оплаты</td>
						<td class="table-capt">Номер платежного поручения</td>
						<td class="table-capt">Дата внесения</td>
						<td class="table-capt">Дата списания</td>
						<td width="1%"></td>
					</tr>
				</thead>
				<tbody>
					'.$content.'
				</tbody>
			</table>
			';
		}
		
		if ($for_creditor) {
			return $content;
		}
		
		$content.='<a href="#" class="more-payments">Ещё</a>';
		$mode = 'success';
		
	} else {
		// Неудачные платежи
		$query = mysql_query("SELECT *, `date` AS `payment_date`, `validate` AS `pay_validate` FROM `wp_loans_payments` WHERE `validate` !=100 $and AND loan_id=$loan_id ORDER BY `date` DESC");
		
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$row = $this->loans_translate_vals($row);
			$moderate = '';
			if ((!$for_creditor) and (current_user_can('loan_edit_payment')) and ($row['user'] != 'user')) {
				$moderate = '<span class="delete-btn jsDeletePayment fancy-tip" title="Удалить" id="payment-'.$row['id'].'"></span>';
			}
			
			$content.='
			<tr>
				<td>'.$row['amount'].'</td>
				<td>'.$row['pay_method2'].'</td>
				<td>'.$row['payment_date'].'</td>
				<td>'.$row['pay_validate'].'</td>
			</tr>
			';
		}
		
		if (!$content) {
			$content = '
			<div class="white-wrap">Нет платежей.</div>
			';
		} else {
			$content = '
				<table>
					<thead>
						<tr>
							<td class="table-capt">Сумма</td>
							<td class="table-capt">Способ оплаты</td>
							<td class="table-capt">Дата внесения</td>
							<td class="table-capt">Причина неудачи</td>
						</tr>
					</thead>
					<tbody>
						'.$content.'
					</tbody>
				</table>
			';
		}
		$mode = 'failed';
	}
	
	$content .= $this->show_pages($num_pages, $page, 5, 'jsPaymentsListPages').'
	<div class="clear"></div>';

	return '<div class="jsPaymentsWrap" id="jsPaymentsWrap-'.$mode.'">'.$content.'</div>'.$moder;
}

function get_payments_moderate($loan_id) {
	$query = mysql_query("SELECT `validate` FROM `wp_loans` WHERE id=$loan_id");
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	$bind = $this->check_bind($loan_id, 'buh', false);
	
	$locked = 'locked';
	if ((($bind) or ($bind===0) or ($this->is_admin)) and (($row['validate']==5) or ($row['validate']==13))) {
		$locked = '';
	}
	$moder = '';
	if (current_user_can('loan_edit_payment')) {
		$moder = '<div class="centered padd10"><a href="#" class="typ-btn jsBuhAddPayment '.$locked.'">Добавить платеж</a><a href="#" class="typ-btn jsBuhReloadPayments">Обновить</a></div>';
	}
	return $moder;
}

function get_where_date($field) {
	switch ($field) {
		case 'back_date':
			$date_sql = "l.`back_date`";
		break;
		case 'back_date_real':
			$date_sql = "l.`back_date_real`";
		break;
		case 'date':
			$date_sql = "l.`date`";
		break;
		case 'pay_date':
			$date_sql = "l.`pay_date`";
		break;
		case 'need_call_date':
			$date_sql = "l.`need_call_date`";
		break;
	}
	return $date_sql;
}

function new_loan_frame($mode) {
	$checked = 'checked="checked"';
	$query = mysql_query("SELECT * FROM `wp_loans_tariffs` WHERE `archive`=0 ORDER BY `order`");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$new = $addclass = '';
		
		if ($row['new']) {
			$new = '&nbsp;<span class="text-red text-small">Новинка!</span>';
		}
		if ($row['available'] == 1) {
			$addclass = ' al-f';
		}
		$tariffs.= '
		<div class="fl cc-type'.$addclass.'">
			<input type="radio" class="niceRadio" name="tariff" id="cc-myradio'.$row['id'].'" value="'.$row['id'].'" tabindex="'.$row['id'].'" '.$checked.'/>
			<label for="cc-myradio'.$row['id'].'" class="oh db">
				<span class="fs13">'.$row['descr'].'</span>'.$new.'
			</label>
			<p class="jsPercentHolder">'.(float)$row['percent'].'% в день</p>
		</div>';
		$checked = '';
	}
	
switch ($mode) {
	default:
		
	break;
	case 'cab':
		$alert = '
		<div class="calc__bl__content__alert cc-alert">
			<div class="cc-alert-flasher"></div>
			<a href="#" class="calc__bl__content__alert__x"></a>
			<p><img src="'.site_url().'/wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="" />
			Этот тип займа пока невозможно оформить автоматически.<br>
			Воспльзуйтесь займом от 5 000 до 15 000.</p>
		</div>';
		$button = '<input type="submit" name="submit" id="submit" class="jsSimpleLoan calc__bl__content__btm__btn calc__bl__content__btm__btn_submit h58" value="Отправить заявку">';
	break;
	case 'calc':
		$alert='
		<div class="calc__bl__content__alert cc-alert">
			<a href="#" class="calc__bl__content__alert__x"></a>
			<p><img src="'.site_url().'/wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="" />
			Этот тип займа доступен только постоянным клиентам.<br>
			Воспльзуйтесь займом от 5 000 до 30 000 или авторизируйтесь.</p>
			<p class="cc-login-form">Логин: <input class="txt" type="text" name="auth_login"> Пароль: <input class="txt" type="password" name="auth_passw"> <a href="#" class="woo-sc-button green jsAuthEnter" type="submit">Вход</a></p>
			<div class="cc-alert-flasher"></div>
		</div>';
		$button = '<a href="#" class="woo-sc-button mz-btn green xl"><span>Оформить</span></a>';
	break;
}
	

	return $content;
	//echo '<pre>'; var_export($row); echo '</pre>';  //отладка
}

/* Проверка может ли зареганный юзер оформлять новый займ
------------------------------------------------------------------------------*/
function new_loan_errors($user_id) {
	
	$user_id = $this->form_text($user_id, 'db');
	//TODO: заменить date_diff из SQL-запроса запросом к мини-статистике
	$query = mysql_query("SELECT `validate`, (DATEDIFF(CURRENT_TIMESTAMP(), `date`)) AS `date_diff` FROM `wp_loans` WHERE user_id=$user_id");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		if (($row['validate'] < 6) or (in_array($row['validate'], array(12,13)))) {
			return 'Новая заявка на займ не может быть размещена, пока у вас есть текущая заявка.';
		}
		if ((($row['validate'] == 8) or (($row['validate'] == 9))) and ($row['date_diff'] < 30)) {
			return 'Новая заявка на займ не может быть размещена, поскольку у вас имеются отклоненные займы за последние 30 дней.';
		}
	}
	
	return false;
}

/* Новый калькулятор микрозаймов
------------------------------------------------------------------------------*/
function calc_page($step, $mode='calc', $ajaxing=false) {
	
	wp_enqueue_script('mask-input', '/wp-content/plugins/mfunny-cabs/js/jquery.maskedinput.js', array('jquery'));
	wp_enqueue_script('tooltipster', '/wp-content/plugins/mfunny-cabs/js/jquery.tooltipster.min.js', array('jquery'));
	wp_enqueue_style( 'tooltipster', '/wp-content/plugins/mfunny-cabs/css/tooltipster.css', true, '1.10.2' );
	session_start();
	
	// перенаправление на калькулятор с главной страницы
	$_GET['amount'] = preg_replace("/[^0-9]/", '', $_GET['amount']);
	$_GET['term'] = preg_replace("/[^0-9]/", '', $_GET['term']);
	if (($_GET['amount']) and ($_GET['term'])) {
		$mode = 'calc';
		$ajaxing = false;
		
		unset($_POST);
		$_POST['next_step'] = 2;
		$_POST['tariff'] = 11;
		$_POST['amount'] = $_GET['amount'];
		$_POST['term'] = $_GET['term'];
		$_POST['earnings'] = 0;
		$_SESSION['calc_step1'] = $_POST;
	}
	
	// проверяем был ли уже юзер на этом шаге
	
	if ($ajaxing) {
		if ($_SESSION['calc_step'.$step]) {
			$_INSERT = $_SESSION['calc_step'.$step];
			foreach ($_INSERT as $key => $value) {
				$_INSERT[$key] = str_replace('\r\n', '
', $value);
			}
		}
	} else {
		//unset($_SESSION['calc_step1']); // если надо сохранять сессию только до обновления страницы
		//unset($_SESSION['calc_step2']);
		//unset($_SESSION['calc_step3']);
		//unset($_SESSION['calc_step4']);
	}
	
	switch ($mode) {
		case 'cab':
			$button = '<input type="submit" name="submit" id="submit" class="jsSimpleLoan typ-calc-btn" value="Отправить заявку">';
			$not_allowed_message = 'Займ этого типа пока нельзя оформить автоматически. Свяжитесь с менеджером по телефону.';
		break;
		default:
		case 'calc':
			$button = '<a href="#" class="jsStepSubmit forward typ-calc-btn"><span>Оформить</span></a>';
			$not_allowed_message = 'Этот тип займа доступен только постоянным клиентам.<br>
			Воспльзуйтесь займом от 5 000 до 30 000 или авторизируйтесь.</p>
			<p class="cc-login-form">Логин: <input class="txt" name="auth_login" type="text"> Пароль: <input class="txt" name="auth_passw" type="password"> <a href="#" class="woo-sc-button green jsAuthEnter" type="submit">Вход</a></p>';
		break;
	}
	
	switch ($step) {
		case 1:
            $label = 'Рассчёт заёма';
            
			// достаем тарифы
			if ($ajaxing) {
				$checked = '';
			} else {
				$checked = 'checked="checked"';
			}
			$query = mysql_query("SELECT * FROM `wp_loans_tariffs` WHERE `archive`=0 AND `hidden`=0 ORDER BY `order`");
			while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
				$new = $addclass = '';
				if (!$first_tariff_bg) {
					$first_tariff_bg = $row['bg'];
				}
				
				if ($row['new']) {
					$new = '&nbsp;<span class="text-red text-small">Новинка!</span>';
				}
				if ($row['available'] == 1) {
					$addclass = ' al-f';
				}
				if (trim($row['descr'])==='') {
					$min_amount = number_format($row['min_amount'], 0, '.', ' ');
					$max_amount = number_format($row['max_amount'], 0, '.', ' ');
					$min_amount = str_replace(' ', '&nbsp;', $min_amount);
					$max_amount = str_replace(' ', '&nbsp;', $max_amount);
					$row['descr'] = 'От '.$min_amount.' до '.$max_amount;
				}
				
				if (($ajaxing) and ($_INSERT['tariff'] == $row['id'])) {
					$checked = 'checked="checked"';
				}
				
				$tariffs.= '
				<div class="fl cc-type'.$addclass.'">
					<input type="radio" class="niceRadio" name="tariff" id="cc-myradio'.$row['id'].'" value="'.$row['id'].'" tabindex="'.$row['id'].'" '.$checked.'/>
					<label for="cc-myradio'.$row['id'].'" class="oh db">
						<span class="fs13">'.$row['descr'].'</span>'.$new.'
					</label>
					
					<p class="jsPercentHolder">'.(float)$row['percent'].'% в день</p>
					<div class="jsMinAmountHolder" style="display:none;">'.$row['min_amount'].'</div>
					<div class="jsMaxAmountHolder" style="display:none;">'.$row['max_amount'].'</div>
					<div class="jsBgHolder" style="display:none;">'.$row['bg'].'</div>
				</div>';
				$checked = '';
			}
			
			if (!$_INSERT['term']) {
				$_INSERT['term'] = '14';
			}
			$_INSERT['term'] .= ' дн';
			
			if ($_INSERT['amount']) {
				$_INSERT['amount'] .=' р';
			}
			
			$content = '
				<div class="error-container calc__bl__content__alert cc-alert" style="display:none;">
					<div class="error-inner"><img src="'.$this->site_url.'wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="">&nbsp;<span></span></div>
					<div class="cc-alert-flasher" style="display:none;"></div>
					<div class="jsErrorContainerCloser"></div>
				</div>
				
				<div class="jsLoanNotAllowedHolder" style="display:none;">
					'.$not_allowed_message.'
				</div>
				
				<div class="calc__bl__content__frm border-t i-clearfix">
					'.$tariffs.'
				</div>
		
				<div class="calc__bl__content__frm border-t i-clearfix fast">
					<table class="mzaim15">
						<tr>
							<td><label>Хочу<br/>занять</label></td>
							<td>
								<div class="slider1" style="background-image:url('.$this->site_url.'wp-content/plugins/mfunny-cabs/css/images/calc/'.$first_tariff_bg.');">
									<div class="slider11">
										<div id="mz-slider1"></div>
									</div>
								</div>
								<input type="text" class="txt t-8" name="amount" value="'.$_INSERT['amount'].'" id="amount">
							</td>
						</tr>
						<tr>
							<td><label>На срок</label></td>
							<td>
								<div class="slider2">
									<div class="slider11">
										<div id="mz-slider2"></div>
									</div>
								</div>
								<input type="text" class="txt t-8" name="term" value="'.$_INSERT['term'].'" id="term">
							</td>
						</tr>
					</table>
				</div>

				<div class="calc__bl__content__btm i-clearfix">
					'.$button.'
					<span class="calc__bl__content__btm__caption"><span>Расчёт</span></span>
					<span class="calc__bl__content__btm__item">
						<em>
							Ваш заем <br />
							<span class="fs16" id="sum">-- р</span>
						</em>
					</span>
					<span class="calc__bl__content__btm__item">
						<em>Переплата <br />
							<span class="fs16" id="over-pay">-- р</span></em>
					</span>
					<span class="calc__bl__content__btm__item">
						<em>Вернете <br />
							<span class="fs16" id="return">-- р</span></em>
					</span>
					<span class="calc__bl__content__btm__item monthly">
						<em>Ежемесячно <br />
							<span class="fs16" id="monthly">-- р</span></em>
					</span>
				</div>';
		break;
		case 2:
			$label = 'Основная информация';
            
			// Достаем филиалы
			$branches = '';
			$selected = '';
			$query = mysql_query("SELECT * FROM `wp_loans_branches` ORDER BY `order`");
			while (@$row = mysql_fetch_array($query, MYSQL_ASSOC)) {
				
				if (($ajaxing) and ($_INSERT['branch'] == $row['id'])) {
					$selected = 'selected';
				}
				$branches.= '<option value="'.$row['id'].'" '.$selected.'>'.$row['name'].'</option>';
				$selected = '';
			}
			
			$propiska_selected[$_INSERT['propiska']] = 'selected';
			$sex_selected[$_INSERT['sex']] = 'checked';
			$relative_selected[$_INSERT['relative']] = 'selected';
			
			$content = '
			<div class="error-container calc__bl__content__alert cc-alert" style="display:none;">
				<div class="error-inner"><img src="'.$this->site_url.'wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="">&nbsp;<span></span></div>
				<div class="cc-alert-flasher" style="display:none;"></div>
				<div class="jsErrorContainerCloser"></div>
			</div>
		
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<table>
					<tr>
						<td>Фамилия</td>
						<td class="second-col"><input type="text" name="sname" class="txt w250" maxlength="255" value="'.$_INSERT['sname'].'"></td>
						<td>Пол</td>
						<td>
							<div class="fl">
								<input type="radio" class="niceRadio" name="sex" value="1" id="female" tabindex="1" '.$sex_selected[1].'/>
								<label for="female" class="">Жен</label>
							</div>
							<div class="fl pl20">
								<input type="radio" class="niceRadio" name="sex" value="2" id="male" tabindex="2" '.$sex_selected[2].'/>
								<label for="male" class="">Муж</label>
							</div>
						</td>
					</tr>
					<tr>
						<td>Имя</td>
						<td><input type="text" name="fname" class="txt w250" maxlength="255" value="'.$_INSERT['fname'].'"></td>
						<td>День<br/>рождения</td>
						<td>
							<input type="text" name="birth" class="txt data datepicker" id="birth" value="'.$_INSERT['birth_dirty'].'" readonly>
						</td>
					</tr>
					<tr>
						<td>Отчество</td>
						<td><input type="text" name="tname" class="txt w250" maxlength="255" value="'.$_INSERT['tname'].'"></td>
						
						<td>Ближайший к вам филиал</td>
						<td>
							<select name="branch" class="select req">
								<option value=""></option>
								'.$branches.'
							</select>
						</td>
					</tr>
				</table>
			</div>
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<a href="#" class="jsStepSubmit forward typ-calc-btn"><span>Следующий шаг</span></a>
				<a href="#" class="jsStepSubmit back"><span>&larr; Вернуться назад</span></a>
			</div>';
		break;
		
		case 3:
			$label = 'Паспортные данные';
			
			$seconddoc_insert[$_INSERT['seconddoc']] = 'selected';
			
			$content = '
			<div class="error-container calc__bl__content__alert cc-alert" style="display:none;">
				<div class="error-inner"><img src="'.$this->site_url.'wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="">&nbsp;<span></span></div>
				<div class="cc-alert-flasher" style="display:none;"></div>
				<div class="jsErrorContainerCloser"></div>
			</div>
			
			<div class="auth-frame jsAuthFrameNew" style="display: none;">
				<div class="auth-opp"></div>
				<div class="auth-frame-border" ></div>
				<div class="jsCloseAuthFreme"></div>
				<h3>Авторизация</h3>
				<div class="auth-frame-inner">
					<div class="auth-msg"></div>
					
					<table>
						<tbody>
						<tr>
							<td class="first-td"><p>Электронная почта:</p></td>
							<td><input name="auth_login" type="text" class="txt w115 optional" maxlength="255"></td>
						</tr>
						<tr>
							<td class="first-td"><p>Пароль:</p></td>
							<td><input name="auth_passw" type="password" class="txt w115 optional" maxlength="100"></td>
						</tr>
						</tbody>
					</table>
					
					<a class="jsAuthEnter woo-sc-button mz-btn green xl" href="#">Вход</a>
					
					<div class="restore-info">
						<a href="#" class="jsRestorePassword">Забыли пароль?</a><br>
						<a href="#" class="jsRestoreEmail">Забыли адрес электронной почты?</a>
					</div>
				</div>
			</div>
			
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<table class="fl w340 mr30">
					<tr>
						<td>Паспорт №</td>
						<td>
							<input type="text" name="paspser" id="paspser" maxlength="4" class="txt w85 jsForceNumb" value="'.$_INSERT['paspser'].'">
							<input type="text" name="paspnom" id="paspnom" maxlength="6" class="txt w115 jsForceNumb" value="'.$_INSERT['paspnom'].'">
						</td>
					</tr>
					<tr>
						<td>Город выдачи</td>
						<td>
							<input type="text" name="pasp_location" maxlength="255" class="txt w250" value="'.$_INSERT['pasp_location'].'">
						</td>
					</tr>
					<tr>
						<td>Кем выдан</td>
						<td>
							<textarea name="paspkem" id="strict2" cols="30" rows="10" class="txt optional" placeholder="Строго как в паспорте">'.$_INSERT['paspkem'].'</textarea>
						</td>
						
					</tr>
				</table>
	
				<table class="fl w315">
					<tr>
						<td>Дата <br /> выдачи</td>
						<td>
							<input type="text" name="paspdate" class="txt data datepicker optional" id="pasp" readonly  value="'.$_INSERT['paspdate_dirty'].'">
						</td>
					</tr>
					<tr>
						<td>Место рождения</td>
						<td>
							<textarea name="birthaddr" id="strict2" cols="30" rows="10" class="txt optional" placeholder="Строго как в паспорте">'.$_INSERT['birthaddr'].'</textarea>
						</td>
					</tr>
					<tr>
						<td>Второй документ</td>
						<td>
							<select name="seconddoc" id="s2" class="select">
								<option value="0" '.$seconddoc_insert[0].'></option>
								<option value="1" '.$seconddoc_insert[1].'>Вод. удостоверение</option>
								<option value="2" '.$seconddoc_insert[2].'>Заграничный паспорт</option>
								<option value="3" '.$seconddoc_insert[3].'>Свидетельство ИНН</option>
								<option value="4" '.$seconddoc_insert[4].'>Пенсионное удостоверение</option>
								<option value="5" '.$seconddoc_insert[5].'>Удостоверение генерала</option>
								<option value="6" '.$seconddoc_insert[6].'>Cвидетельство о регистрации ТС</option>
								<option value="7" '.$seconddoc_insert[7].'>Карточка пенсионного страхования</option>
							</select>
						</td>
					</tr>
				</table>
			</div>
	
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<a href="#" class="jsStepSubmit forward typ-calc-btn"><span>Следующий шаг</span></a>
				<a href="#" class="jsStepSubmit back"><span>&larr; Вернуться назад</span></a>
			</div>';
		break;
	
		case 4:
			$label = 'Адрес прописки';
			
			$street_type1_insert[$_INSERT['street_type1']] = 'selected';
			
			$content = '
			<div class="error-container calc__bl__content__alert cc-alert" style="display:none;">
				<div class="error-inner"><img src="'.$this->site_url.'wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="">&nbsp;<span></span></div>
				<div class="cc-alert-flasher" style="display:none;"></div>
				<div class="jsErrorContainerCloser"></div>
			</div>
			
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<table class="fl w340 mr30">
					<tr>
						<td>Город</td>
						<td>
							<input type="text" name="city1" maxlength="100" class="txt w250" value="'.$_INSERT['city1'].'">
						</td>
					</tr>
					<tr>
						<td>Тип улицы</td>
						<td>
							<select name="street_type1" class="select">
								<option value="0"></option>
								<option value="01" '.$street_type1_insert["01"].'>Аллея</option>
								<option value="02" '.$street_type1_insert["02"].'>Бульвар</option>
								<option value="03" '.$street_type1_insert["03"].'>Въезд</option>
								<option value="04" '.$street_type1_insert["04"].'>Дорога</option>
								<option value="05" '.$street_type1_insert["05"].'>Заезд</option>
								<option value="06" '.$street_type1_insert["06"].'>Казарма</option>
								<option value="07" '.$street_type1_insert["07"].'>Квартал</option>
								<option value="08" '.$street_type1_insert["08"].'>Километр</option>
								<option value="09" '.$street_type1_insert["09"].'>Кольцо</option>
								<option value="10" '.$street_type1_insert["10"].'>Линия</option>
								<option value="11" '.$street_type1_insert["11"].'>Местечко</option>
								<option value="12" '.$street_type1_insert["12"].'>Микрорайон</option>
								<option value="13" '.$street_type1_insert["13"].'>Набережная</option>
								<option value="14" '.$street_type1_insert["14"].'>Парк</option>
								<option value="15" '.$street_type1_insert["15"].'>Переулок</option>
								<option value="16" '.$street_type1_insert["16"].'>Переезд</option>
								<option value="17" '.$street_type1_insert["17"].'>Площадь</option>
								<option value="18" '.$street_type1_insert["18"].'>Площадка</option>
								<option value="19" '.$street_type1_insert["19"].'>Проспект</option>
								<option value="20" '.$street_type1_insert["20"].'>Проезд</option>
								<option value="21" '.$street_type1_insert["21"].'>Просек</option>
								<option value="22" '.$street_type1_insert["22"].'>Проселок</option>
								<option value="23" '.$street_type1_insert["23"].'>Проулок</option>
								<option value="24" '.$street_type1_insert["24"].'>Строение</option>
								<option value="25" '.$street_type1_insert["25"].'>Территория</option>
								<option value="26" '.$street_type1_insert["26"].'>Тракт</option>
								<option value="27" '.$street_type1_insert["27"].'>Тупик</option>
								<option value="28" '.$street_type1_insert["28"].'>Улица</option>
								<option value="29" '.$street_type1_insert["29"].'>Участок</option>
								<option value="30" '.$street_type1_insert["30"].'>Шоссе</option>
								
							</select>
						</td>
					</tr>
					<tr>
						<td>Название улицы</td>
						<td>
							<input type="text" name="street1" maxlength="100" class="txt w250" value="'.$_INSERT['street1'].'">
						</td>
					</tr>
				</table>
				
				<table class="fl w315">
					<tr>
						<td>Корпус<br><span class="text-small">(не обязательно)</td>
						<td>
							<input type="text" name="corp1" maxlength="10" class="txt" value="'.$_INSERT['corp1'].'">
						</td>
					</tr>
					<tr>
						<td>Строение<br><span class="text-small">(не обязательно)</td>
						<td>
							<input type="text" name="structure1" maxlength="20" class="txt" value="'.$_INSERT['structure1'].'">
						</td>
					</tr>
					<tr>
						<td>Дом</td>
						<td>
							<input type="text" name="house_number1" maxlength="10" class="txt" value="'.$_INSERT['house_number1'].'">
						</td>
					</tr>
					<tr>
						<td>Квартира</td>
						<td>
							<input type="text" name="apartment1" maxlength="10" class="txt" value="'.$_INSERT['apartment1'].'">
						</td>
					</tr>
				</table>
			</div>
			
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<a href="#" class="jsStepSubmit forward typ-calc-btn"><span>Следующий шаг</span></a>
				<a href="#" class="jsStepSubmit back"><span>&larr; Вернуться назад</span></a>
			</div>';
		break;
	
		case 5:
			$label = 'Адрес проживания';
			
			$street_type1_insert[$_INSERT['street_type2']] = 'selected';
			$address_same_insert = $disabled = '';
			if ($_INSERT['address_same']) {
				$address_same_insert = 'checked';
				$disabled = ' disabled';
			}
			
			$content = '
			<div class="error-container calc__bl__content__alert cc-alert" style="display:none;">
				<div class="error-inner"><img src="'.$this->site_url.'wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="">&nbsp;<span></span></div>
				<div class="cc-alert-flasher" style="display:none;"></div>
				<div class="jsErrorContainerCloser"></div>
			</div>
			
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				
				<div class="mb15"><input id="address_same" type="checkbox" class="niceCheck muteOthers" name="address_same" '.$address_same_insert.'/><label class="db oh" for="address_same">По месту прописки</label></div>
				<table class="fl w340 mr30">
					<tr>
						<td>Город</td>
						<td>
							<input type="text" name="city2" maxlength="100" class="txt w250" value="'.$_INSERT['city2'].'"'.$disabled.'>
						</td>
					</tr>
					<tr>
						<td>Тип улицы</td>
						<td>
							<select name="street_type2" class="select"'.$disabled.'>
								<option value="0"></option>
								<option value="01" '.$street_type1_insert["01"].'>Аллея</option>
								<option value="02" '.$street_type1_insert["02"].'>Бульвар</option>
								<option value="03" '.$street_type1_insert["03"].'>Въезд</option>
								<option value="04" '.$street_type1_insert["04"].'>Дорога</option>
								<option value="05" '.$street_type1_insert["05"].'>Заезд</option>
								<option value="06" '.$street_type1_insert["06"].'>Казарма</option>
								<option value="07" '.$street_type1_insert["07"].'>Квартал</option>
								<option value="08" '.$street_type1_insert["08"].'>Километр</option>
								<option value="09" '.$street_type1_insert["09"].'>Кольцо</option>
								<option value="10" '.$street_type1_insert["10"].'>Линия</option>
								<option value="11" '.$street_type1_insert["11"].'>Местечко</option>
								<option value="12" '.$street_type1_insert["12"].'>Микрорайон</option>
								<option value="13" '.$street_type1_insert["13"].'>Набережная</option>
								<option value="14" '.$street_type1_insert["14"].'>Парк</option>
								<option value="15" '.$street_type1_insert["15"].'>Переулок</option>
								<option value="16" '.$street_type1_insert["16"].'>Переезд</option>
								<option value="17" '.$street_type1_insert["17"].'>Площадь</option>
								<option value="18" '.$street_type1_insert["18"].'>Площадка</option>
								<option value="19" '.$street_type1_insert["19"].'>Проспект</option>
								<option value="20" '.$street_type1_insert["20"].'>Проезд</option>
								<option value="21" '.$street_type1_insert["21"].'>Просек</option>
								<option value="22" '.$street_type1_insert["22"].'>Проселок</option>
								<option value="23" '.$street_type1_insert["23"].'>Проулок</option>
								<option value="24" '.$street_type1_insert["24"].'>Строение</option>
								<option value="25" '.$street_type1_insert["25"].'>Территория</option>
								<option value="26" '.$street_type1_insert["26"].'>Тракт</option>
								<option value="27" '.$street_type1_insert["27"].'>Тупик</option>
								<option value="28" '.$street_type1_insert["28"].'>Улица</option>
								<option value="29" '.$street_type1_insert["29"].'>Участок</option>
								<option value="30" '.$street_type1_insert["30"].'>Шоссе</option>
								
							</select>
						</td>
					</tr>
					<tr>
						<td>Название улицы</td>
						<td>
							<input type="text" name="street2" maxlength="100" class="txt w250" value="'.$_INSERT['street2'].'"'.$disabled.'>
						</td>
					</tr>
				</table>
				
				<table class="fl w315">
					<tr>
						<td>Корпус<br><span class="text-small">(не обязательно)</td>
						<td>
							<input type="text" name="corp2" maxlength="10" class="txt" value="'.$_INSERT['corp2'].'"'.$disabled.'>
						</td>
					</tr>
					<tr>
						<td>Строение<br><span class="text-small">(не обязательно)</td>
						<td>
							<input type="text" name="structure2" maxlength="20" class="txt" value="'.$_INSERT['structure2'].'"'.$disabled.'>
						</td>
					</tr>
					<tr>
						<td>Дом</td>
						<td>
							<input type="text" name="house_number2" maxlength="10" class="txt" value="'.$_INSERT['house_number2'].'"'.$disabled.'>
						</td>
					</tr>
					<tr>
						<td>Квартира</td>
						<td>
							<input type="text" name="apartment2" maxlength="10" class="txt" value="'.$_INSERT['apartment2'].'"'.$disabled.'>
						</td>
					</tr>
				</table>
			</div>
			
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<a href="#" class="jsStepSubmit forward typ-calc-btn"><span>Следующий шаг</span></a>
				<a href="#" class="jsStepSubmit back"><span>&larr; Вернуться назад</span></a>
			</div>';
		break;
	
		case 6:
            $label = 'Информация о ближайшем родственнике';
			
			$norels_error = $disabled = $norels_checked = '';
			$display = 'none';
			
			if ($_INSERT['norels']) {
				$norels_checked = 'checked';
				$norels_error = 'При выборе данного параметра, велика вероятность отказа в выдаче займа.';
				$disabled = ' disabled';
				$display = 'block';
			}
            
			$content = '
			<div class="error-container calc__bl__content__alert cc-alert" style="display:'.$display.';">
				<div class="error-inner"><img src="'.$this->site_url.'wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="">&nbsp;<span>'.$norels_error.'</span></div>
				<div class="cc-alert-flasher" style="display:none;"></div>
				<div class="jsErrorContainerCloser"></div>
			</div>
			
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				
				<table class="fl w340 mr30">
				<tr>
					<td colspan="4">
						<input type="checkbox" name="norels" id="norels" class="niceCheck" '.$norels_checked.'>
						<label class="oh db" for="norels">Ближайших родственников нет</label>
					</td>
				</tr>
				<tr>
					<td style="padding-right:0;">Фамилия</td>
					<td><input type="text" name="relsname" class="txt w250 mb-optional" maxlength="255" value="'.$_INSERT['relsname'].'"'.$disabled.'></td>
					<td>Степень родства</td>
					<td>
						<select id="s2" name="relative" class="select"'.$disabled.'>
							<option value="0"></option>
							<option value="1" '.$relative_selected[1].'>Отец</option>
							<option value="2" '.$relative_selected[2].'>Мать</option>
							<option value="3" '.$relative_selected[3].'>Муж</option>
							<option value="4" '.$relative_selected[4].'>Жена</option>
							<option value="5" '.$relative_selected[5].'>Брат</option>
							<option value="6" '.$relative_selected[6].'>Сестра</option>
							<option value="7" '.$relative_selected[7].'>Сын</option>
							<option value="8" '.$relative_selected[8].'>Дочь</option>
						</select>
					</td>
				</tr>
				<tr>
					<td style="padding-right:0;">Имя</td>
					<td><input type="text" name="relfname" class="txt w250 mb-optional" maxlength="255" value="'.$_INSERT['relfname'].'"'.$disabled.'></td>
					<td>Телефон родственника</td>
					<td>
						<input type="text" name="relphone" class="txt mb-optional jsPhoneMask" id="relphone" maxlength="30" value="'.$_INSERT['relphone'].'"'.$disabled.'>
					</td>
				</tr>
				<tr>
					<td style="padding-right:0;">Отчество</td>
					<td><input type="text" name="reltname" class="txt w250 mb-optional" maxlength="255" value="'.$_INSERT['reltname'].'"'.$disabled.'></td>
					<td></td>
					<td></td>
				</tr>
				</table>
			</div>
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<a href="#" class="jsStepSubmit forward typ-calc-btn"><span>Следующий шаг</span></a>
				<a href="#" class="jsStepSubmit back"><span>&larr; Вернуться назад</span></a>
			</div>';
		break;
	
		case 7:
			$label = 'Контактная информация';
            
			if ($_INSERT['rules1']) {
				$rules1_insert = 'checked';
			}
			if ($_INSERT['rules2']) {
				$rules2_insert = 'checked';
			}
			if ($_INSERT['rules3']) {
				$rules3_insert = 'checked';
			}
			
			$my_postid = 4538;//This is page id or post id
			$content_post = get_post($my_postid);
			$legit_text = $content_post->post_content;
			$legit_text = apply_filters('the_content', $legit_text);
			$legit_text = str_replace(']]>', ']]&gt;', $legit_text);
			$legit_text = strip_tags($legit_text);
			$legit_text = htmlspecialchars($legit_text);
			$legit_text = str_replace("'", "\'", $legit_text);
			
			// Завершение
			$content = '
			<div class="error-container calc__bl__content__alert cc-alert" style="display:none;">
				<div class="error-inner"><img src="'.$this->site_url.'wp-content/themes/pixelpress/images/calc__bl__content__alert.png" alt="">&nbsp;<span></span></div>
				<div class="cc-alert-flasher" style="display:none;"></div>
				<div class="jsErrorContainerCloser"></div>
			</div>
			
			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<div class="clcmzm"></div>
					
				<table>
					<tbody>
					<tr>
						<td style="padding-right:0;">Эл. почта</td>
						<td><input type="text" name="email" class="txt w250" maxlength="255" value="'.$_INSERT['email'].'"></td>
						<td style="padding-right:0;">Ваш мобильный телефон</td>
						<td><input type="text" name="phone1" class="txt w250 jsPhoneMask" maxlength="30" value="'.$_INSERT['phone1'].'"></td>
					</tr>
					<tr>
						<td style="padding-right:0;">Придумайте пароль<br><span class="text-small">(для входа в личный кабинет)</span></td>
						<td><input type="password" name="password1" class="txt w250" maxlength="30"></td>
						<td style="padding-right:0;">Доп. телефон<br><span class="text-small">(не обязательно)</span></td>
						<td><input type="text" name="phone2" class="txt w250" maxlength="30" value="'.$_INSERT['phone2'].'"></td>
					</tr>
					<tr>
						<td style="padding-right:0;">Повторите пароль<br>
						<td><input type="password" name="password2" class="txt w250" maxlength="30"></td>
						<td rowspan="2" colspan="2">
							<div class="mb15"><input type="checkbox" class="niceCheck" name="rules1" id="ch1" '.$rules1_insert.'/><label class="db oh" for="ch1">Я являюсь гражданином РФ</label></div>
							<div class="mb15"><input type="checkbox" class="niceCheck" name="rules2" id="ch2" '.$rules2_insert.'/><label class="db oh" for="ch2">Я имею постоянную регистрацию РФ </label></div>
							<div class="mb15">
								<input type="checkbox" class="niceCheck" name="rules3" id="ch3" '.$rules3_insert.'/>
								<label class="db oh" style="display: inline-block;" for="ch3">
									Согласен на передачу и обработку
									<a class="fancyTipClick typ-rel" title="'.$legit_text.'" href="#" target="_blank">моих персональных данных</a>
								</label>
							</div>
						</td>
					</tr>
					<tr>
						
					</tr>
					</tbody>
				</table>
				
			</div>

			<div class="calc__bl__content__frm p10 border-t i-clearfix">
				<a href="#" class="jsStepSubmit forward typ-calc-btn"><span>Отправить заявку</span></a>
				<a href="#" class="jsStepSubmit back"><span>&larr; Вернуться назад</span></a>
			</div>';
		break;
	}
	
    $arr = array(
        'page' => $content,
        'label' => $label,
        'step' => $step
    );
	return $arr;
}

// Формирование кнопок из $row
// Возвращает измененный $row
function get_tool($key, $arr) {
	//echo '<pre>'; var_export($arr); echo '</pre>'; // отладка
	switch ($key) {
		case 'manager_calls2':

		break;
	}
	return $arr;
}


function write_log($field, $db, $action, $id, $new_value, $old_value='', $force_write=false) {
	$always_write = array('sb_id', 'manager_id', 'buh_id', 'collector_id');

	if (($this->is_admin) and (!$force_write) and (!in_array($field, $always_write))) {
		return;
	}
	
	switch ($db) {
		case 'wp_loans':
			$loan_id = $id;
			$client_id = 0;
		break;
		case 'wp_loans_users':
			$client_id = $id;
			$loan_id = 0;
		break;
		case 'wp_loans_payments':
			// хз почему это работает...
			$query = mysql_query("SELECT `loan_id` FROM `wp_loans_payments` WHERE id=$id");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			$loan_id = $row['loan_id'];
			$client_id = 0;
		break;
		case 'wp_loans_comments':
			if ($action == 'edit_comment') {
				$query = mysql_query("SELECT `loan_id` FROM `wp_loans_comments` WHERE id=$id");
				@$row = mysql_fetch_array($query, MYSQL_ASSOC);
				$loan_id = $row['loan_id'];
			} else {
				$loan_id = $id;
				unset($id);
			}
			$client_id = 0;
		break;
		case 'wp_loans_term_adds':
			$loan_id = $id;
			$client_id = 0;
		break;
	}

	if (($old_value==='') and ($id)) {
		$query = mysql_query("SELECT `$field` FROM `$db` WHERE `id`='$id'");
		@$row = mysql_fetch_array($query, MYSQL_ASSOC);
		$old_value = $row[$field];
	}
	if ($old_value != $new_value) {
		
		$worker_id = $this->form_text($this->user_id);
		$date = gmdate('Y-m-d H:i:s'); // TIMESTAMP по Гринвичу
		
		mysql_query("INSERT INTO `wp_loans_logs` SET `action`='$action', `field`='$field', `date`='$date', `worker_id`='$worker_id', `loan_id`='$loan_id', `client_id`='$client_id', `value_old`='$old_value', `value_new`='$new_value'");
		//echo mysql_error();
	}
}

function show_logs($loan_id, $page = 1, $mode='') {
	$loan_id = $this->form_text($loan_id, 'db');
	$query2 = mysql_query("SELECT `user_id` FROM `wp_loans` WHERE id=$loan_id");
	@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
	

	// Узнаём общее количество страниц по запросу
	$per_page = 5;
	$page = preg_replace("/[^0-9]/", '', $page);
	if (!$page) {
		$page = 1;
	}
	
	switch ($mode) {
		case 'default':
		default:
			$mode_sql = "`action` != 'user_pay' AND `action` != 'user_term_add'";
			$mode = 'default';
		break;
		case 'payments':
			$mode_sql = "(`action` = 'user_pay' OR `action` = 'user_term_add')";
		break;
	}
	
	$query = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans_logs` WHERE $mode_sql AND (`loan_id`=$loan_id OR `client_id`={$row2['user_id']})");
	$row = mysql_fetch_array($query);
	$num_pages=ceil($row['numb']/$per_page);
	$limit = $per_page*($page-1).', '.$per_page;
	
	$n=0;
	$query = mysql_query("SELECT * FROM `wp_loans_logs` WHERE $mode_sql AND (`loan_id`=$loan_id OR `client_id`={$row2['user_id']}) ORDER BY `date` DESC LIMIT $limit");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$n++;
		// ебать как я любою свичи!
		switch ($row['action']) {
			case 'edit':
				
				switch ($row['field']) {
					default:
						$arr1 = array($row['field'] => $row['value_old']);
						$arr2 = array($row['field'] => $row['value_new']);
						$main_field = $row['field'];
					break;
					case 'address':
						$main_field = $row['field'];
						
						if ($row['value_new'] == 1) {
							$vat = $row['value_old'];
							$row['value_old'] = 2;
						}
						if ($row['value_old'] == 1) {
							$vat = $row['value_new'];
							$row['value_new'] = 2;
						}
						
						$arr1 = array('address' => $row['value_old'], 'vat' => $vat);
						$arr2 = array('address' => $row['value_new'], 'vat' => $vat);
					break;
					case 'vat':
						$arr1 = array('address' => 2, 'vat' => $row['value_old']);
						$arr2 = array('address' => 2, 'vat' => $row['value_new']);

						$row['field'] = $main_field = 'address';
					break;
					case 'manager_calls2_comment':
						$arr1 = array($row['field'] => $row['value_old']);
						$arr2 = array($row['field'] => $row['value_new']);
						$main_field = 'manager_calls2';
					break;
				}
				
				$arr1 = $this->loans_translate_vals($arr1);
				$arr2 = $this->loans_translate_vals($arr2);
				
				$row['value_old'] = $arr1[$row['field']];
				$row['value_new'] = $arr2[$row['field']];
				$action = 'Изменил поле <a class="jsFancyUp" href="#'.$main_field.'">'.$this->loans_translate($main_field).'</a> c "<span class="text-red">'.$row['value_old'].'</span>" на "<span class="text-green">'.$row['value_new'].'</span>".';
			break;
			case 'validate':
				$arr1 = array('validate' => $row['value_old']);
				$arr2 = array('validate' => $row['value_new']);
				$arr1 = $this->loans_translate_vals($arr1);
				$arr2 = $this->loans_translate_vals($arr2);
				$row['value_old'] = $arr1['validate'];
				$row['value_new'] = $arr2['validate'];
				$action = 'Сменил статус заявки с '.$row['value_old'].' на '.$row['value_new'];
				
			break;
			case 'recall':
				switch ($row['value_new']) {
					case 0:
						$action = 'Достал заявку из отложенных.';
					break;
					case 1:
						$action = 'Отложил заявку.';
					break;
				}
			break;
			case 'recall2':
				switch ($row['value_new']) {
					case 0:
						$action = '<a href="#manager_calls" class="jsFancyUp">Отозвал оповещение об истечении займа за 3 дня.</a>';
					break;
					case 1:
						switch ($row['value_old']) {
							case 0:
								$action = '<a href="#manager_calls" class="jsFancyUp">Оповестил заемщика об истечении займа за 3 дня.</a>';
							break;
							case 2:
								$action = '<a href="#manager_calls" class="jsFancyUp">Отозвал оповещение об истечении займа за 1 день.</a>';
							break;
						}
					break;
					case 2:
						$action = '<a href="#manager_calls" class="jsFancyUp">Оповестил заемщика об истечении займа за 1 день.</a>';
					break;
				}
			break;
			case 'recall3':
				switch ($row['value_new']) {
					case 0:
						$action = '<a href="#manager_calls2" class="jsFancyUp">Отозвал приглашение в офис.</a>';
					break;
					case 1:
						$action = '<a href="#manager_calls2" class="jsFancyUp">Пригласил заемщика в офис.</a>';
					break;
				}
			break;
			case 'term_del':
				$action = '<span class="text-red">Удалил продление на '.$row['value_old'].' дн.</span>';
			break;
			case 'payment_add':
				$action = '<span class="text-green">Добавил платеж на '.$row['value_new'].' р.</span>';
			break;
			case 'payment_del':
				$action = '<span class="text-red">Удалил платеж на '.$row['value_old'].' р.</span>';
			break;
			
			case 'add_comment':
			case 'edit_comment':
			case 'del_comment':
				switch ($row['field']) {
					case 1:
						$field_label = 'комментарий СБ #1';
					break;
					case 2:
						$field_label = 'комментарий СБ #2';
					break;
					case 3:
						$field_label = 'комментарий бухгалтера';
					break;
					case 4:
					case 5:
						$field_label = 'общий комментарий';
					break;
				}
				switch ($row['field']) {
					default:
						// обычные комменты
						switch ($row['action']) {
							case 'add_comment':
								$action = '<span class="text-green">Добавил <a href="#jsCommentContainer-'.$row['field'].'" class="jsFancyUp">'.$field_label.'</a>: '.$this->form_text($row['value_new']).'</span>';
							break;
							case 'edit_comment':
								$action = 'Изменил <a href="#jsCommentContainer-'.$row['field'].'" class="jsFancyUp">'.$field_label.'</a> с "<span class="text-red">'.$this->form_text($row['value_old']).'</span>" на "<span class="text-green">'.$this->form_text($row['value_new']).'</span>"';
							break;
							case 'del_comment':
								$action = '<span class="text-red">Удалил <a href="#jsCommentContainer-'.$row['field'].'" class="jsFancyUp">'.$field_label.'</a>: '.$this->form_text($row['value_old']).'</span>';
							break;
						}
					break;
					case 5:
						// особые уличные комменты типа "надо позвонить"
						switch ($row['action']) {
							case 'add_comment':
								$need_call_comment = '';
								if (trim($row['value_new'])) {
									$need_call_comment = ' Комментарий: '.$this->form_text($row['value_new']);
								}
								$action = '<span class="text-green">Отметил, что заемщику <a href="#jsCommentContainer-4" class="jsFancyUp">надо позвонить</a> '.$this->get_time($row['value_old']).'.'.$need_call_comment.'</span>';
							break;
							case 'edit_comment':
								$action = 'Изменил комментарий с пометкой <a href="#jsCommentContainer-4" class="jsFancyUp">надо позвонить</a> с "<span class="text-red">'.$this->form_text($row['value_old']).'</span>" на "<span class="text-green">'.$this->form_text($row['value_new']).'</span>"';
							break;
							case 'del_comment':
								$need_call_comment = '';
								if (trim($row['value_old'])) {
									$need_call_comment = ': '.$this->form_text($row['value_old']);
								}
								$action = '<span class="text-red">Удалил комментарий с пометкой <a href="#jsCommentContainer-4" class="jsFancyUp">надо позвонить</a>'.$need_call_comment.'</span>';
							break;
						}
					break;
					case 6:
						// особые уличные комменты типа "звонок состоялся"
						switch ($row['action']) {
							case 'add_comment':
								$need_call_comment = '';
								if (trim($row['value_new'])) {
									$need_call_comment = ' Комментарий: '.$this->form_text($row['value_new']);
								}
								$action = '<span class="text-red">Отметил, что <a href="#jsCommentContainer-4" class="jsFancyUp">звонок состоялся</a>.'.$need_call_comment.'</span>';
							break;
							case 'edit_comment':
								$action = 'Изменил комментарий с пометкой <a href="#jsCommentContainer-4" class="jsFancyUp">звонок состоялся</a> с "<span class="text-red">'.$this->form_text($row['value_old']).'</span>" на "<span class="text-green">'.$this->form_text($row['value_new']).'</span>"';
							break;
							case 'del_comment':
								$need_call_comment = '';
								if (trim($row['value_old'])) {
									$need_call_comment = ': '.$this->form_text($row['value_old']);
								}
								$action = '<span class="text-red">Удалил комментарий с пометкой <a href="#jsCommentContainer-4" class="jsFancyUp">звонок состоялся</a>'.$need_call_comment.'</span>';
							break;
						}
					break;
				}
			break;
			
			case 'user_pay':
				$row['value_new'] = unserialize($row['value_new']);
				switch ($row['value_new']['validate']) {
					case 100:
						if ($row['value_new']['pay_method2']) {
							$row['value_new']['pay_method'] .= ' ('.$row['value_new']['pay_method2'].')';
						}
						switch ($row['value_new']['method']) {
							case 'auto':
								$row['value_new']['method'] = 'автоматически';
							break;
							case 'manual':
								$row['value_new']['method'] = 'вручную';
							break;
						}
						$action = '<span class="text-green">Удачный платёж #'.$row['value_new']['id'].' на сумму <b>'.(float)$row['value_new']['amount'].'</b> руб. Способ оплаты: '.$row['value_new']['pay_method'].'. Обработан '.$row['value_new']['method'].'.</span>';
					break;
					case 10:
						$action = '<span class="text-red">Платёж #'.$row['value_new']['id'].' отменён, деньги от покупателя не были получены.</span>';
					break;
				}
			break;
			
			case 'user_term_add':
				$row['value_new'] = $this->days_to_stupid_russian($row['value_new']);
				$action = '<span class="text-orange">Заёмщик продлил займ на '.$row['value_new'].'</span>';
			break;
		}
		
		$worker_str = '';
		if ($mode != 'payments') {
			$worker_str = '<td>'.$this->get_staff_name($row['worker_id']).'</td>';
		}
		
		$content.='
		<tr>
			<td>'.$this->get_time($row['date'], 'full', true).'</td>
			'.$worker_str.'
			<td>'.$action.'</td>
		</tr>';
	}
	if (!$n) {
		$content = '<div class="white-wrap jsLogsContainer" id="logs-'.$mode.'">Логи пока пусты.</div>';
		return $content;
	}
	
	$worker_str = '';
	if ($mode != 'payments') {
		$worker_str = '<td>Сотрудник</td>';
	}
	
	$content = '
	<table>
		<thead>
		<tr>
			<td>Дата события</td>
			'.$worker_str.'
			<td>Действие</td>
		</tr>
		</thead>
		<tbody>
			'.$content.'
		</tbody>
	</table>';
	$content .= $this->show_pages($num_pages, $page, 5, 'jsLogsListPages');
	return '<div class="jsLogsContainer" id="logs-'.$mode.'">'.$content.'</div>';
}

function show_comments($loan_id, $comment_type, $page=1) {
	$comment_type_sql = $comment_type;
	if ($comment_type == 4) {
		$comment_type_sql = '4,5,6';
	}
	
	$per_page = 5;
	$query = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans_comments` WHERE loan_id=$loan_id AND comment_type IN($comment_type_sql)");
	$row = mysql_fetch_array($query);
	$num_pages=ceil($row['numb']/$per_page);
	$limit = $per_page*($page-1).', '.$per_page;
	
	
	$query = mysql_query("SELECT *, `text` AS `comment` FROM `wp_loans_comments` WHERE loan_id=$loan_id AND comment_type IN($comment_type_sql) ORDER BY `date` DESC LIMIT $limit");
	$n = 0;
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		
		$edit = '';
		if (($this->user_id == $row['author']) or ($this->is_admin)) {
			$edit = '
			<a title="Редактировать" class="jsEditComment" id="jsEditComment-'.$row['id'].'"></a>
			<a class="delete-btn jsDeleteComment" id="jsDelComment-'.$row['id'].'"></a>';
		}
		$row['real'] = $row;
		$row = $this->loans_translate_vals($row);
		if ($row['edit_date']) {
			$row['date'] .= '<p class="text-orange text-small">Последняя дата редактирования:</p><p class="text-orange text-small">'.$row['edit_date'].'</p>';
		}

		$content.='
		<tr>
			<td>'.$row['date'].'</td>
			<td>'.$row['author'].'</td>
			<td>
				<div class="typ-rel">
					<div class="jsEditWrap">
						'.$row['text'].'
						'.$edit.'
						<!-- <a title="Скопировать в буфер" class="btn-copy"></a> -->
						<a style="display: none;" class="copy-active"></a>
					</div>
				</div>
			</td>
		</tr>
		';
		$n++;
	}
	if ($n) {
		$content = '
		<table>
			<thead>
			<tr>
				<td>Дата</td>
				<td>Сотрудник</td>
				<td width="60%">Комментарий</td>
			</tr>
			</thead>
			<tbody>
				'.$content.'
			</tbody>
		</table>';
	} else {
		$content = '<div class="white-wrap">Пока нет комментариев.</div>';
	}
	
	$content = '<div class="wrep-wrap">'.$content.'</div>';
	
	$content .= $this->show_pages($num_pages, $page, 5, 'jsCommentsListPages');
	
	switch ($comment_type) {
		case 1:
			$key = 'loan_comment';
			$can_add = current_user_can($key);
		break;
		case 2:
			$key = 'loan_comment2';
			$can_add = current_user_can($key);
		break;
		case 3:
			$key = 'loan_comment3';
			$can_add = current_user_can($key);
		break;
		case 4:
			$key = 'loan_comment4';
			$can_add = current_user_can($key);
		break;
	}

	if ($this->is_admin) {
		$can_add = true;
	}
	
	if (($can_add) and (!$this->is_sb)) {
		$_POST['id']= $loan_id;
		$can_add = $this->check_ajax_rights($key, false, false, false);
	}

	if ($can_add) {
		$can_add = '';
	} else {
		$can_add = ' style="display:none;"';
	}
	
	$content .= '
	<a href="#" class="typ-btn jsAddComment"'.$can_add.'>Добавить комментарий</a>';
	
	// дополнительная функция к общему комментарию - "надо позвонить"
	if ($comment_type == 4) {
		$class = '';
		$query2 = mysql_query("SELECT `need_call_date` FROM `wp_loans` WHERE id=$loan_id");
		$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
		if ($row2['need_call_date'] != '0000-00-00 00:00:00') {
			// кнопку отмены обзвона показываем в любом случае
			$content .= '<a href="#" class="typ-btn jsRemoveCall selected"'.$can_add.'>Звонок состоялся</a>';
		} else {
			$content .= '<a href="#" class="typ-btn jsAddCall"'.$can_add.'>Нужен звонок</a>';
		}
	}
	$content .= '
	<div class="clear"></div>';
	
	return '<div class="jsCommentContainer" id="jsCommentContainer-'.$comment_type.'">'.$content.'</div>';
}

/* Перерисовка кнопок модерирования
------------------------------------------------------------------------------*/
function get_moderate_btns($arr) {
	$this->moderate = '';
	$tmp = $this->update_likes_status($arr['real']['validate']);

	/* Перезвонить
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_recall')) {
		if (($this->is_sb) or ($this->is_manager)) {
			if (!$this->is_sb) {
				$dude = 'manager';
			} else {
				$dude = 'sb';
			}
			
			// Этот кусок не обращается к функции update_likes_status, потому что эта фигня управляется не полем validate
			if (in_array($arr['real']['validate'], array(5,6,12,13))) {
				$delays_locked = ' locked';
			}
			
			if ($arr['recall_'.$dude]) {
				$this->moderate.= '<a href="#" class="jsRecall typ-btn selected'.$delays_locked.'" alt="0">Из отложенных</a>';
			} else {
				$this->moderate.= '<a href="#" class="jsRecall typ-btn'.$delays_locked.'" alt="1">Отложить</a>';
			}
		}
	}
	/* Проверка в НБКИ
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_nbki')) {
		if ($arr['recall_nbki']) {
				$this->moderate.= '<a href="#" class="jsRecallNbki typ-btn selected'.$delays_locked.'" alt="0">Проверено в НБКИ</a>';
		} else {
				$this->moderate.= '<a href="#" class="jsRecallNbki typ-btn'.$delays_locked.'" alt="1">Проверка в НБКИ</a>';
		}
	}
	/* Корзина
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_trash')) {
		if (($this->is_sb) or ($this->is_manager)) {
			if (!$this->is_sb) {
				$dude = 'manager';
			} else {
				$dude = 'sb';
			}
			
			if ($arr['trash_'.$dude]) {
				$arr[$key].= ', <span class="text-red">В корзине</span>';
				$this->moderate.= '<a href="#" class="jsTrash typ-btn selected'.$delays_locked.'" alt="0">Из корзины</a>';
			} else {
				$this->moderate.= '<a href="#" class="jsTrash typ-btn'.$delays_locked.'" alt="1">В корзину</a>';
			}
		}
	}
	
	/* Долг взыскан
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_collect')) {
		if (in_array($arr['real']['validate'], array(12,13,14))) {
			switch ($arr['real']['validate']) {
				case 12:
					$this->moderate.='
					<a href="#" class="jsCollect typ-btn ">Долг взыскан</a>
					<a href="#" class="jsCollectX typ-btn ">Невозможно вернуть</a>';
				break;
				case 13:
					$this->moderate.='
					<a href="#" class="jsCollect typ-btn selected">Отменить взыскание</a>
					<a href="#" class="jsCollectX typ-btn locked">Невозможно вернуть</a>';
				break;
				case 14:
					$this->moderate.='
					<a href="#" class="jsCollect typ-btn locked">Долг взыскан</a>
					<a href="#" class="jsCollectX typ-btn selected">Отменить невозможность возврата</a>';
				break;
			}
			
			
			
		}
	}
	
	/* Скопировать в фейки
	------------------------------------------------------------------------------*/
	if (($this->is_admin) and ($_COOKIE['show_admin_hidden'])) {
		if ($arr['fake']) {
			$this->moderate.= '<a href="#" class="typ-btn buh jsDeletekaGhJKA selected">Удалить фейк</a>';
		} else {
			if (($arr['real']['validate'] == 8) or ($arr['real']['validate'] == 9)) {
				$query = mysql_query("SELECT `id` FROM `wp_loans` WHERE `fake`='{$arr['id']}'");
				@$row = mysql_fetch_array($query, MYSQL_ASSOC);
				if ($row['id']) {
					$this->moderate.= '<a href="'.$this->site_url.'sb_cabinet?mode=single&id='.$row['id'].'" class="typ-btn buh">Просмотреть фейк</a>';
				} else {
					$this->moderate.= '<a href="#" class="typ-btn buh jsCreatekaGhJKA">Скопировать в фейки</a>';
				}
			}
		}
	}
	
	$this->moderate = '<div class="moder-wrap">'.$this->moderate.'</div>';
}

// Секция модерации буха
function get_moderate_btns2($arr) {
	$this->moderate2 = '';
	$tmp = $this->update_likes_status($arr['real']['validate']);
	
	/* Выдача
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_pay')) {
		$this->moderate2.= '<a href="#" class="jsBuhPay typ-btn '.$tmp[5][1].'" alt="1">'.$tmp[5][2].'</a>';
	}
}


function get_moderate_btns3($arr) {
	$this->moderate3 = '';
	$tmp = $this->update_likes_status($arr['real']['validate']);
	
	/* Продлить (новая кнопка)
	------------------------------------------------------------------------------*/
	$locked = 'locked';
	
	if ((current_user_can('loan_term_add')) and (in_array($arr['real']['validate'], array(5,12,13)))) {
		// проверяем просрочен займ или нет
		if ($this->stats['expired']) {
			// если займ просрочен, то проценты должны проверяться за сегодняшний день
			$percents = $this->stats['last']['percent_rub_sum'];
		} else {
			// иначе - за последний день
			//$last_day = $this->get_last_doc_bac_date($_POST['id']);
			//$arrr2 = $_P->get_buh_stats($row, $last_day);
			$percents = $arrr2['last']['percent_rub_sum'];
		}
		
		$locked = '';
	}
	$this->moderate3.= '<a href="#" class="typ-btn jsAddTerm '.$locked.'">Продлить</a>';

	/* закрытие займа
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_pay')) {
		if (!count($this->stats['close_days'])) {
			$tmp[6][1] = 'locked';
		}
		$this->moderate3.= '<a href="#" class="jsBuhBack typ-btn '.$tmp[6][1].'" alt="1">'.$tmp[6][2].'</a>';
	}
	
	/* Отправить коллектору
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_send_to_collector')) {
		if (!$this->stats['expired_days']) {
			$tmp[8][1] = 'locked';
		}
		// кнопка появляется только если заявка находится в просроченных - validate проверяется в SQL запросе!
		//$where = $this->get_where('exceed', 'new', '');
		//$query = mysql_query("SELECT l.`id` $where AND l.id={$arr['id']}");
		//if (@$row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$this->moderate3.='<a href="#" class="jsSendToCollector typ-btn '.$tmp[8][1].'">'.$tmp[8][2].'</a>';
		//}
	}
	
	/* Остановить начисление %
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_stop_percent')) {
		$locked = 'locked';
		if (in_array($arr['real']['validate'], array(5,12,13))) {
			$locked = '';
		}
		if ($arr['real']['percent_stop'] == '0000-00-00 00:00:00') {
			$this->moderate3.='<a href="#" class="jsStopPercent typ-btn '.$locked.'">Остановить начисление %</a>';
		} else {
			if (!$this->is_admin) {
				$locked = 'locked';
			}
			$this->moderate3.='<a href="#" class="jsStopPercentUndo typ-btn selected'.$locked.'">Отменить остановку начисления %</a>';
		}
	}
	
	/* Создать график платежей
	------------------------------------------------------------------------------*/
	if (current_user_can('loan_stop_percent')) {
		if ((in_array($arr['real']['validate'], array(5,12,13))) and ($arr['real']['percent_stop'] != '0000-00-00 00:00:00')) {
			$locked = '';
			$query2 = mysql_query("SELECT `pay_date` FROM `wp_loans_payments_graph` WHERE loan_id={$arr['real']['id']} AND `closed`=0 ORDER BY `pay_date` DESC LIMIT 1");
			@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
			if ($row2['pay_date']) {
				$now_local_small = explode(' ', $now_local);
				$now_local_small = $now_local_small[0];
				$last_pay_date_small = explode(' ', $row2['pay_date']);
				$last_pay_date_small = $last_pay_date_small[0];
				
				if ($this->is_first_date_bigger($last_pay_date_small, $now_local_small)) {
					$locked = ' locked';
				}
			}
			
			
			$this->moderate3.='<a href="#" class="jsAddPayGraph typ-btn'.$locked.'">Создать график выплат</a>';
		}
	}

	$this->moderate3 = '<div class="moder-wrap">'.$this->moderate3.'</div>';
}

function get_main_row($id) {
	$id = $this->form_text($id,'db');
	
	$sql = "SELECT *, l.id AS `id`,
	u.`manager_id` AS `manager_check`, u.`sb_id` AS `sb_check`, u.`buh_id` AS `buh_check`, u.`blocked`, u.`score`,
	l.`force_percent`
	FROM `wp_loans` AS `l`, `wp_loans_users` AS `u`
	WHERE u.id = l.user_id AND l.id={$id}";
	
	$query = mysql_query($sql);
	echo mysql_error();
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	
	$row['percent'] = 0;
	if ($row['tariff']) {
		$query2 = mysql_query("SELECT `percent` FROM `wp_loans_tariffs` WHERE `id`={$row['tariff']}");
		$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
		$row['percent'] = $row2['percent'];
	}
	$row['force_percent'] = (double)$row['force_percent'];
	if ($row['force_percent']) {
		$row['percent'] = $row['force_percent'];
	}
	$row['real'] = $row;
	return $row;
}

/* Вывод индивидуальной записи
------------------------------------------------------------------------------*/
function get_loan_page($id) {
	$row = $this->get_main_row($id);
	
	if (!$row['id']) {
		$this->error_tpl('Ошибка 404', 'Заявка не найдена.');
	}
	
	if ((!$this->is_admin) and (!$this->is_sb)) {
		if (($row['sb_check'] == $this->user_id) or ($row['collector_check'] == $this->user_id) or ($this->is_manager) or ($row['buh_check'] == $this->user_id) or
		(($this->is_sb) and (!$row['sb_check'])) or (($this->is_collector) and (!$row['collector_check'])) or (($this->is_buh) and (!$row['buh_check']))) {
			// работает - не трогай) лол
		} else {
			$this->error_tpl('Ошибка доступа', 'У вас нет прав для просмотра этой заявки.');
		}
	}
	
	$class = $this->update_likes_status($row['validate']);

	if ($row) {
		$content = '';
		
		// Запоминание подсветки записей
		$cook = $_COOKIE['loan-'.$id];
		if ($cook) {
			$cook = urldecode($cook);
			$arr = explode('&', $cook);
			$cook = array();
			foreach ($arr as $key => $value) {
				$value = explode('=', $value);
				$cook[$value[0]] = $value[1];
			}
		}
		
		// костыли для верхней таблицы
		$row['term_long'] = $row['term'];
		$row['amount_full'] = $row['amount'];
		$row['pay_date_long'] = $row['pay_date'];
		$row['address_old'] = $row['address'];
		$row['fulladdr_old'] = $row['fulladdr'];
		$row['address_same'] = $row['address'];
		
		
		$this->stats = $this->get_buh_stats($row, 'now');
		$this->stats_full = $this->get_buh_stats($row);
		$row = $this->loans_translate_vals($row);
		
		if ($row['real']['pay_date']) {
			$date_min = $this->utc_to_local($row['real']['pay_date'], 'Y-m-d-H-i');
			$periods = $this->get_loan_periods($row['id']);
			
			
			if ($this->stats['expired']) {
				$date_max = $this->utc_to_local(gmdate('Y-m-d H:i:s'), 'Y-m-d-H-i');
			} else {
				$date_max = $this->utc_to_local($periods[count($periods)-1]['till'], 'Y-m-d-H-i');
			}
			
			$content .= '
			<div class="payDateMin" style="display: none;">'.$date_min.'</div>
			<div class="payDateMax" style="display: none;">'.$date_max.'</div>';
		}
		
		if (!$_COOKIE['loan_tab']) {
			$_COOKIE['loan_tab'] = 'tab-loan';
		}
		$style['tab-loan'] = $style['tab-user'] = $style['tab-ki'] = 'style="display:none;"';
		$style[$_COOKIE['loan_tab']] = '';
		$class2[$_COOKIE['loan_tab']] = 'active';
		
		$content.= '
		<div id="jsIdHolder" style="display: none;">'.$row['id'].'</div>
		<div id="jsUserIdHolder" style="display: none;">'.$row['user_id'].'</div>
		
		<div class="loan-tabs">
			<a class="typ-btn '.$class2['tab-loan'].'" id="tab-loan" href="#">Займ</a>
			<a class="typ-btn '.$class2['tab-user'].'" id="tab-user" href="#">Заёмщик</a>
			<a class="typ-btn '.$class2['tab-ki'].'" id="tab-ki" href="#">Кредитная история</a>
		</div>
		<div class="section-separator role-separator"><p></p></div>';
		
		$this->get_moderate_btns($row);
		
		if ((in_array($row['real']['validate'], array(5,12,13))) and (!$row['real']['fake'])) {
			$empty_fields = $this->check_nbki_fields($id);
			if (count($empty_fields)) {
				$content .='<br><br><div class="tip tip-red">Внимание! У этого заемщика заполнены не все необходимые поля. Информация в НБКИ не передаётся. <a href="#" class="jsAddKiInfo">Заполнить</a>.</div>';
			}
		}
		
		$content.= '<div class="tab-loan" '.$style['tab-loan'].'>
		<br><br>
		<h2>Информация о займе:</h2>';
		if (in_array($row['real']['validate'], array(5,6,12,13,14))) {
			$add = ', remind';
		}
		$content.= $this->loan_section_tpl('Заявка', 'tariff, idea, purpose, force_percent, loan_method, amount, term_long, earnings, date, validate, contract_id, manager_calls, manager_calls2, pay_date_long, nbki_hide, sms_off'.$add, $row, $cook);
		
		if (current_user_can('loan_send_to_buh') or ($this->is_collector)) {
			$hidden = 'typ-hidden';
			if (in_array($row['real']['validate'], array(4,5,6,12,13,14))) {
				$docs = $this->docs_list($id, true);
			} else {
				$docs = $this->docs_list($id);
			}
			$content.= '
			<div class="jsDocs">
				'.$docs.'
			</div>'; 
		}
		
		// График выплат
		$graph = $this->get_pay_graph($row['id']);
		// Статистика
		$content.= $this->stats['stats'];
		$content.= $this->stats_full['output'];
		$content.= $graph['html'];
		
		
		$content.= '<br><br><br>
		<h2 style="margin-bottom:0;">Управление займом:</h2>
		<div class="section-separator"><span>Общий инструментарий</span><p></p></div>';
		$content.= $this->loan_section_tpl('', 'moderate', $row, $cook);
		
		$content.= '<h3>Общие комментарии:</h3>'.$this->show_comments($row['id'], 4);
		
		
		$content.= '<div class="section-separator"><span>1. Предварительная проверка СБ</span><p></p></div>';
		if (current_user_can('loan_approve')) {
			$content.= '
			<div class="val-btn-wrap jsValidate1">
				<a href="#" class="jsValidateYes validate-btn '.$class[1][1].'" title="Одобрить"></a>
				<a href="#" class="jsValidateNo validate-btn '.$class[1][2].'" title="Отклонить"></a>
				<a href="#" class="jsValidateMaybe validate-btn '.$class[1][3].'" title="Не уверен"></a>
				<div class="clear"></div>
			</div>';
		}
		
		$content.= '<h3>Комментарии СБ #1:</h3>'.$this->show_comments($row['id'], 1);
		$content.= '<div class="section-separator"><span>2. Проверка заполненных данных менеджером</span><p></p></div>';
		
		if (current_user_can('loan_send_to_sb')) {
			$content.= '<div class="centered jsValidate2"><span class="jsManagerTool typ-btn '.$class[3][1].'">'.$class[3][2].'</span></div>';
		}
		
		$content.= '<div class="section-separator"><span>3. Контрольная проверка СБ</span><p></p></div>';
		
		if (current_user_can('loan_approve')) {
			$content.= '
			<div class="val-btn-wrap jsValidate3">
				<a href="#" class="jsValidateYes validate-btn '.$class[2][1].'" title="Одобрить"></a>
				<a href="#" class="jsValidateNo validate-btn '.$class[2][2].'" title="Отклонить"></a>
				<a href="#" class="jsValidateMaybe validate-btn '.$class[2][3].'" title="Не уверен"></a>
				<div class="clear"></div>
			</div>';
		}
		
		$content.= '<h3>Комментарии СБ #2:</h3>'.$this->show_comments($row['id'], 2);
		
		$content.= '
		<div class="section-separator"><span>4. Заключение договора и выдача денег</span><p></p></div>';
		$this->get_moderate_btns2($row);
		$content.= '
		<div class="centered jsValidate4">
			<span class="jsManagerTool typ-btn '.$class[4][1].'">'.$class[4][2].'</span>
			'.$this->moderate2.'
		</div>';
		$content.= '<h3>Комментарии бухгалтера:</h3>'.$this->show_comments($row['id'], 3);
		
		
		$content.= '<div class="section-separator"><span>5. Прием платежей от заемщика</span><p></p></div>';
		
		$content.= '<h3>Удачные платежи</h3>'.$this->get_payments($row['id'], false, false, 1, "AND `hide_from_user` = 0");
		$content.= '<div id="more-payments" style="display:none;"><h3>Непрошедшие платежи</h3>'.$this->get_payments($row['id'], false, true).'</div>';
		$content.= $this->get_payments_moderate($row['id']);
		
		
		$content.= '<div class="section-separator"><span>6. Инструментарий для закрытия займа</span><p></p></div>';
		
		$this->get_moderate_btns3($row);
		$content.= $this->loan_section_tpl('', 'moderate3', $row, $cook);
		
		
		// Инструментарий админа
		$content.= '<div class="section-separator"><span></span><p></p></div><h2>Логи действий:</h2>';
		
		if ($this->is_admin) {
			$content.= '
			<h3>Логи действий сотрудников:</h3>
			'.$this->show_logs($id);
		}
		
		$content.= '
		<h3>Логи действий заёмщика:</h3>
		'.$this->show_logs($id, 1, 'payments').'
		<h3>SMS-оповещения:</h3>
		'.$this->show_sms($id).'
		<div class="section-separator"><span></span><p></p></div>';
		
		
		if (($row['fake']) and ($this->is_admin) and ($_COOKIE['show_admin_hidden'])) {
			$class = 'kahfjkaF';
		} else {
			$class = '';
		}
		
		$content.= '</div><div class="tab-user typ-rel" '.$style['tab-user'].'>
		<br><br>
		<h2>Информация о заёмщике:</h2>';
		
		$content.= $this->loan_section_tpl('Персональные данные', 'score, blocked, sname, fname, tname, sex, birth, address_old, seconddoc, card, branch, photo, images', $row, $cook);
		$content.= $this->loan_section_tpl('Паспортные данные', 'paspser, paspnom, paspkem, pasp_location, paspdate, birthaddr, fulladdr_old', $row, $cook);
		$content.= $this->loan_section_tpl('Адрес прописки', 'city1, street_type1, street1, corp1, structure1, house_number1, apartment1', $row, $cook);
		$content.= $this->loan_section_tpl('Адрес проживания', 'address_same, city2, street_type2, street2, corp2, structure2, house_number2, apartment2', $row, $cook);
		$content.= $this->loan_section_tpl('Контакты', 'phone1, phone2, email', $row, $cook);
		$content.= $this->loan_section_tpl('Ближайший родственник', 'relative, relsname, relfname, reltname, relphone', $row, $cook);
		$content.= $this->loan_section_tpl('Привязка к сотрудникам', 'sb_id, manager_id, buh_id, collector_id', $row, $cook);
		
		
		// другие заявки
		$content.= $this->show_loans('single', $row['user_id']);
		
		
		
		//$content.= '<br><br>
		//<h2>Управление заемщиком:</h2>
		//<div class="centered">
		//	<div class="rating-sb-wrap">
		//		<span>Рейтинг заёмщика:</span>
		//		<div class="centered">
		//			<div class="rating-sb-wrap">
		//				'.$this->get_user_score($row['id']).'
		//			</div>
		//		</div>
		//	</div>';
		
			if (current_user_can('loan_block')) {
				if ($row['real']['blocked'] == '0000-00-00 00:00:00') {
					$content.='<a href="#" class="jsBlock typ-btn">Добавить в черный список</a>';
				} else {
					$content.='<a href="#" class="jsBlock typ-btn selected">Убрать из черного списка</a>';
				}
			}
			
		//$content.=  '</div>';
		
		
		
		$content.='
		</div>
		<div class="tab-ki typ-rel" '.$style['tab-ki'].'>
		<br><br>
		<h2>Кредитная история:</h2>';
		
		$content.= $this->get_user_ki($row['real']['user_id']);
		
		$content.= '</div>';
		
		$content = '<div class="jsDynamicTable main-content-wrapper '.$class.'">'.$content.'</div>';
		
	} else {
		$content = '<div class="green-message">Запись не найдена.</div>';
	}
	
	return $content;
}

/* Статистика буха
------------------------------------------------------------------------------*/
function get_buh_stats($row, $till='') {
	if (!in_array($row['real']['validate'], array(5,6,12,13,14))) {
		return array();
	}

	$date = $this->utc_to_local($row['real']['pay_date'], 'Y-m-d').' 00:00:00';
	$date = date_create($date);
	$date_short = date_format($date, 'Y-m-d');

	$stats['percent'] = $row['real']['percent'];
	
	$out = array();
	$out['amount'] = $out['amount_full'] = $row['real']['amount'];
	$out['close_days'] = array();
	
	// вывод до определенной даты
	
	if ($till) {
		if ($till == 'now') {
			$till = gmdate('Y-m-d');
		}
		$till = $this->utc_to_local($till, 'Y-m-d').' 23:59:59';
		$till_dirty = $till;
		$till = date_create($till);
		// последний день включительно, потому что пихать параметром функции тот день, за который хотим получить статистику - это более интуитивно понятно
		$till = date_add($till, date_interval_create_from_date_string('1 days'));
		$till = date_format($till, 'Y-m-d');
	}

	//Чтобы учесть платежи в продлениях, совершенные в тот же день, но только те что ПЕРЕД продлением,
	//нужно учитывать их в хронологическом порядке. Для этого, для каждого дня валим платежи и продления в единый массив событий
	$events = $payments = array();

	// учет платежей
	$payments_sum = 0;
	$query = mysql_query("SELECT * FROM `wp_loans_payments` WHERE loan_id={$row['id']} AND `validate`=100 AND `fake`=0");
	while ($row2 = mysql_fetch_array($query, MYSQL_ASSOC)) {
		// сотрудники вводят дату по местному без времени, платежи заемщиков же пишутся по UTC
		if ($row2['user']=='user') {
			$row2['date'] = $this->utc_to_local($row2['date']);
		}
		$row2['date'] = explode(' ', $row2['date']);
		$row2['date'] = $row2['date'][0];
		
		$date_full = $row2['date'].' 00:00:0'.$row2['order'];
		$date_key = date_create($date_full);
		$date_key = date_format($date_key, 'Y-m-d');
		// здесь нужен такой глубокий мульти-массив, потому что эвенты могут перекрываться по датам с точностью в секунду (из-а того что дата платежей выставляется произвольно)
		// зато это позволило избежать гемора с сортировкой массива по дате
		$events[$date_key][$date_full][] = array(
			'type' => 'payment',
			'value' => $row2['amount'],
			'date' => $date_full
		);
		
		$payments[] = array(
			'value' => $row2['amount'],
			'date' => $date_full
		);
		$payments_sum = $payments_sum + $row2['amount'];
	}
	$out['payments_sum'] = $payments_sum;
    
	// учет продлений (обязательно в таком порядке)
	$term_add = 0;
	$periods = array();
	$periods[0] = array();
	$query = mysql_query("SELECT * FROM `wp_loans_term_adds` WHERE `loan_id`={$row['id']} ORDER BY `till`");
	while ($row3 = mysql_fetch_array($query, MYSQL_ASSOC)) {
	
		// если тариф в продлении не задан, берем из микрозайма
		if (!$row3['tariff']) {
			$query4 = mysql_query("SELECT `tariff` FROM `wp_loans` WHERE id={$row['id']}");
			$row4 = mysql_fetch_array($query4, MYSQL_ASSOC);
			$row3['tariff'] = $row4['tariff'];
		}
		
		// если попытаться вытащить в первом запросе, а там будет 0, то и процент будет 0
		$query4 = mysql_query("SELECT `percent` FROM `wp_loans_tariffs` WHERE id={$row3['tariff']}");
		$row4 = mysql_fetch_array($query4, MYSQL_ASSOC);
		$row3['percent'] = $row4['percent'];

		$date_full = $this->utc_to_local($row3['from'], 'Y-m-d').' 00:00:00';
		$date_key = date_create($date_full);
		$date_key = date_format($date_key, 'Y-m-d');
		
		// определяем конечную дату предыдущего дока
		if (!$prev_doc_till) {
			$prev_doc_till = $this->get_loan_periods($row['id']);
			$prev_doc_till = $prev_doc_till[0]['till'];
		}
		
		// определяем был ли займ просрочен в день продления 
		// смотрим есть ли временной разрыв между концом предыдущего дока и началом этого
		$prev_doc_till = date_create($prev_doc_till);
		date_add($prev_doc_till, date_interval_create_from_date_string('1 days'));
		$prev_doc_till = date_format($prev_doc_till, 'Y-m-d H:i:s');
		if ($row3['from'] == $prev_doc_till) {
			$expired = 0;
		} else {
			$expired = 1;
		}
		
		// здесь нужен такой глубокий мульти-массив, потому что эвенты могут перекрываться по датам точностью в секунду (из-а того что дата платежей выставляется произвольно)
		// зато это позволило избежать гемора с сортировкой массива по дате
		$events[$date_key][$date_full][] = array(
			'type' => 'term_add',
			'value' => $row3['term'],
			'date' => $date_full,
			'percent' => $row3['percent'],
			'1d_expired' => $expired
		);
		$periods[] = array(
			'from' => $this->utc_to_local($row3['from'], 'Y-m-d').' 00:00:00',
			'till' => $this->utc_to_local($row3['till'], 'Y-m-d').' 23:59:59',
		);
		$term_add = $term_add + $row3['term'];
		$prev_doc_till = $this->utc_to_local($row3['till'], 'Y-m-d H:i:s');
	}
	
	
	// определяем дату последнего документа
	if ($term_add) {
		$last_day = $periods[count($periods)-1]['till'];
	} else {
		$last_day = $row['real']['back_date'];
		$last_day = $this->utc_to_local($last_day, 'Y-m-d');
	}
	$last_day = date_create($last_day);

	// сохраняем дату окончания последнего документа для подсчета дней просрочки
	$last_doc_back_date = date_format($last_day, 'Y-m-d').' 23:59:59';
    $out['last_doc_back_date'] = $last_doc_back_date;
	$last_doc_back_date = date_create($last_doc_back_date);
	
	// смотрим есть ли платежи после даты окончания последнего документа
	// если есть, то последний день берется датой последнего платежа
	$last_day = date_format($last_day, 'Y-m-d');
	$last_day = date_create($last_day);
	foreach ($payments as $key => $payment) {
		$payment['date'] = explode(' ', $payment['date']);
		$payment['date'] = $payment['date'][0];
		$payment['date'] = date_create($payment['date']);
		
		$interval = date_diff($last_day, $payment['date']);
		if (!$interval->invert) {
			$last_day = $payment['date'];
		}
	}

	// определяем что больше - дата последнего документа или сегодняшний день
	$now = $this->utc_to_local(gmdate('Y-m-d'), 'Y-m-d').' 23:59:59';
	$now = date_create($now);
	$now_short = date_format($now, 'Y-m-d');
	$now_short = date_create($now_short);
	$interval = date_diff($last_day, $now_short);
	
	if ((!$interval->invert) and ($interval->format('%a')) and ($row['real']['validate'] != 6)) {
		$last_day = $now;
	}
	
	// если займ возвращен досрочно
	if (($row['real']['back_date_real']) and ($row['real']['back_date_real'] != '0000-00-00 00:00:00') and ($row['real']['validate'] == 6)) {
		$last_day = $this->utc_to_local($row['real']['back_date_real'], 'Y-m-d');
		$last_day = date_create($last_day);
		$last_day = date_format($last_day, 'Y-m-d');
		$last_day = date_create($last_day);
	}
	
	//if (!$_POST['ajaxing']) {
	//	echo $interval->invert.$interval->format('%a').'<br>';
	//	echo $date_short.'<br>';
	//	echo date_format($last_day, 'Y-m-d').'<br>';
	//}
	
	// находим количество дней в нужный нам периуд
	
	$interval = date_diff($last_day, date_create($date_short));
	$days = $interval->format('%a');
	
	
	//if (!$_POST['ajaxing']) {
	//	echo $days.'<br>';
	//}

	// сроки всех документов по займу идут в массив $periods
	// вычисляем срок первого документа
	$back_date = $this->utc_to_local($row['real']['pay_date'], 'Y-m-d');
	$back_date = date_create($back_date);
	date_add($back_date, date_interval_create_from_date_string(($row['real']['term']-1).' days'));
	
	$periods[0] = array(
		'from' => date_format($date, 'Y-m-d').' 00:00:00',
		'till' => date_format($back_date, 'Y-m-d').' 23:59:59'
	);
	
	$amount_full_bac = 0;
	//echo '<pre>'; var_export($periods); echo '</pre>'; // отладка
	
	// расчет каждого дня поочередно
	for ($i=0; $i<=$days; $i++) {
		$force_expired = false;
		// дата текущего дня
		// обратного перехода на гринвич нет потому что 00:00:00 - 3 часа = предыдущий день
		$out['date'] = date_format($date, 'Y-m-d').' 00:00:00';
		
		$date = date_format($date, 'Y-m-d');
		if (($till) and ($till == $date)) {
			break;
		}
		
		// считаем день
		$out['day']++;
		
		// процент в день (в %)
		$out['percent'] = (float)$stats['percent'];
		
		// если есть дата остановки начисления процентов
		if ($row['real']['percent_stop'] != '0000-00-00 00:00:00') {
			$stop_date = explode(' ', $row['real']['percent_stop']);
			$stop_date = $stop_date[0];
			//echo $stop_date.'<br>';
			$interval = date_diff(date_create($stop_date), date_create($date));
			if (!$interval->invert) {
				$out['percent'] = 0;
			}
		}
		
		// берём начальную сумму от суммы всего раз - это почти константа
		// от нее считается процент в день - это нужно для того чтобы процент в день всегда был одинаковым
		// он может пересчитываться через день после $start_amount, только если за текущий день есть продление
		if (!isset($start_amount)) {
			$start_amount = $out['amount'];
		}
		
		// процент в день (в рублях)
		if ($start_amount == 0) {
			$out['percent_rub'] = 0;
		} else {
			$out['percent_rub'] = round($out['percent'] / 100 * $start_amount, 2);
		}

		$out['percent_rub_sum'] = $out['percent_rub_sum'] + $out['percent_rub'];
		
		// если сумма ушла в минус
		if (($out['amount'] < 0) and ($out['percent_rub_sum'] > 0)) {
			$amount_old = $out['percent_rub_sum'];
			$out['percent_rub_sum'] = $out['percent_rub_sum'] + $out['amount'];
			$out['amount'] = 0;
			if ($out['percent_rub_sum'] < 0) {
				$out['amount'] = 0-$out['percent_rub_sum'];
				$out['percent_rub_sum'] = 0;
			}
		}
		
		// прогоняем в цикле все эвенты за день в хронологическом порядке
		$events_str = '';
		$payments_str = $term_adds_str = array();
		if ($events[$date]) {
			foreach ($events[$date] as $date_time => $events_arr) {
				foreach ($events_arr as $key => $event_arr) {
					if ($event_arr['type'] == 'payment') {
						// обработка платежей за каждую секунду дня, имеющую события
						// сначала вычитаем платеж из процентов
						$out['percent_rub_sum'] = $out['percent_rub_sum'] - $event_arr['value'];
						// если проценты ушли в минус, перекидваем этот минус на тело займа
						if ($out['percent_rub_sum'] < 0) {
							$out['amount'] = $out['amount'] + $out['percent_rub_sum'];
							$out['percent_rub_sum'] = 0;
						}
						$payments_str[] = $event_arr['value'].' р.';
					} else {
						// обработка продлений за каждую секунду дня, имеющую события
						// смена тарифа при продлении
						if (($event_arr['percent']) and ($out['percent'])) {
							$stats['percent'] = $event_arr['percent'];
						}
						$term_adds_str[] = $event_arr['value'].' дн.';
						
						// добавлялось ли продление в просроченный день или нет
						if ($event_arr['1d_expired']) {
							// первый день просрочен - процент начисляется по предыдущему доку
							// оно так и задано в $out выше - ничего не меняем
							$force_expired = true;
							$out['amount'] = $out['percent_rub_sum'] + $out['amount'];
							$out['percent_rub_sum'] = 0;
						} else {
							// первый день НЕ просрочен (продление добавлено заранее) - процент начисляется по новому доку
							// пересчитываем $out
							$out['amount'] = $amount_full_bac;
							$out['percent'] = (float)$stats['percent'];
							$out['percent_rub'] = round($out['percent'] / 100 * $out['amount'], 2);
							$out['percent_rub_sum'] = $out['percent_rub'];
						}
						
						$start_amount = $out['amount'];
						//echo '<pre>'; var_export($event_arr); echo '</pre>'; // отладка
					}
				}
			}
		}
		$payments_str = implode('<br>', $payments_str);
		$term_adds_str = implode('<br>', $term_adds_str);
		
		// вся сумма с процентами
		$out['amount_full'] = $out['amount'] + $out['percent_rub_sum'];
		$amount_full_bac = $out['amount_full'];
		// процент в месяц (в %)
		$out['percent_month'] = round($out['percent']*30, 2);
		// процент в год (в %)
		$out['percent_year'] = round($out['percent']*365, 2);
		// прибыль
		$out['proffit'] = round($out['percent_rub_sum'] + $payments_sum, 2);
		
		// узнаем просрочен ли день
		$expired = true;
		$class = '';
		$title = '';
		$this_day = explode(' ', $out['date']);
		$this_day = date_create($this_day[0]);
		foreach ($periods as $key => $period) {
			$p_from = explode(' ', $period['from']);
			$p_till = explode(' ', $period['till']);
			$p_from = date_create($p_from[0]);
			$p_till = date_create($p_till[0]);
			
			$diff1 = date_diff($p_from, $this_day);
			$diff2 = date_diff($this_day, $p_till);
			
			// если день находится в промежутке между первой и второй датой
			if ((!$diff1->invert) and (!$diff2->invert) and (!$force_expired)) {
				$expired = false;
				$out['expired_days'] = 0;
			}
			
			// посвечиваем сегодня
			if (date_format($this_day, 'Y-m-d') == date_format($now, 'Y-m-d')) {
				$class .= ' bord-today fancy-tip';
				$title = ' title="Сегодня"';
			}
		}
		
		// смотрим начинается ли в текущий день график выплат
		$graph_start_days = array();
		if (!$graph_start_days) { // экономим память, стреляем только раз
			$graphs = $this->get_pay_graph($row['id']);
			$graphs = $graphs['raw'];
			if ($_GET['test']) {
				echo '<pre>'; var_export($graphs); echo '</pre>'; // отладка
				die;
			}
			foreach ($graphs as $create_date => $graph) {
				foreach ($graph as $g) {
					$g['pay_date'] = explode(' ', $g['pay_date']);
					$g['pay_date'] = $g['pay_date'][0];
					$graph_start_days[$g['id']] = $g['pay_date'];
				}
			}
		}
		
		$this_day_out = date_format($this_day, 'Y-m-d');
		$graph_days = '';
		$key = array_search($this_day_out, $graph_start_days);
		if ($key !== false) {
			// поймали id графика выплат и вывели его в "подробнее"
			// теперь тут нужно проверять первый ли это id графика
			// если да, то займ становится не просроченным
			// если нет, то проверять была ли выплачена вся сумма за прошлый периуд
			$graph_days = $key;
		}
		
		// сигналим о просрочке
		if ($expired) {
			$class .= ' back-red';
			$out['expired_days']++;
			$out['expired'] = 1;
		} else {
			$out['expired'] = 0;
		}
		$class = trim($class);
		
		// считаем количество дней до выплаты
		$expired_days = $out['expired_days'];
		if ($out['expired_days'] == 0) {
			$last_doc_back_date = date_format($last_doc_back_date, 'Y-m-d');
			$last_doc_back_date = date_create($last_doc_back_date);
			if ($row['real']['validate'] == 6) {
				if ($row['real']['back_date_real'] == '0000-00-00 00:00:00') {
					$back_date = $row['real']['back_date']; // костыль для старых заявок
				} else {
					$back_date = $row['real']['back_date_real'];
				}
				$back_date = $this->utc_to_local($back_date, 'Y-m-d');
				$date_to_diff = date_create($back_date);
			} else {
				//$date_to_diff = $now_short;
				list($date_to_diff, $trash) = explode(' ', $out['date']);
				$date_to_diff = date_create($date_to_diff);
			}
			
			// определяется именно разницей в датах, а не сроком всех документов минус прошедшие дни, потому что между продлениями могут быть просроченные дни
			$interval = date_diff($last_doc_back_date, $date_to_diff);
			$out['days_till_end'] = $interval->format('%a') + 1; // включая сегодня
			$expired_days = $out['expired_days'].' (дней на выплату: <span class="jsDaysTillEndHolder">'.$out['days_till_end'].'</span>)';
			
			//echo date_format($last_doc_back_date, 'Y-m-d').'<br>';
			//echo date_format($date_to_diff, 'Y-m-d').'<br>';
			//echo $out['days_till_end'].'<br>';
			
			// когда нужно напомнить об окончании займа
			if ($row['remind']) {
				$out['remind'] = $row['remind'];
				if ($out['days_till_end'] == $row['remind']) {
					list($out['remind_date'], $trash) = explode(' ', $out['date']);
				}
			}
		}
		
		// округление копеек до десятых
		$out['percent_rub'] = round($out['percent_rub'], 2);
		$out['percent_rub_sum'] = round($out['percent_rub_sum'], 2);
		$out['amount'] = round($out['amount'], 2);
		$out['amount_full'] = round($out['amount_full'], 2);
		
		// собираем дни, в которые займ может быть закрыт
		if (round($out['amount_full']) <= 0) {
			$close_day = explode(' ', $out['date']);
			$close_day = $close_day[0];
			$out['close_days'][] = $close_day;
		}
		
		// процент набежавший (в %) - находится здесь, чтобы считаться автоматом, учитывая платежи и продления
		if ($start_amount == 0) {
			$out['percent_sum'] = 0;
		} else {
			$out['percent_sum'] = round($out['percent_rub_sum'] * 100 / $start_amount, 2);
		}

		$out['output'].='
		<tr class="'.$class.'"'.$title.'>
			<td>'.$this->get_time($out['date'], 'compact', false).'</td>
			<td>'.$out['day'].'&nbsp;дн.</td>
			<td>'.$out['percent'].'%</td>
			<td>'.$out['percent_rub'].'&nbsp;р.</td>
			<td>'.$out['percent_sum'].'%</td>
			<td>'.$out['percent_rub_sum'].'&nbsp;р.</td>
			
			
			<td>'.$out['amount'].'&nbsp;р.</td>
			<td>'.$out['amount_full'].'&nbsp;р.</td>
			<td>'.$payments_str.'</td>
			<td>'.$term_adds_str.'</td>
			<td>'.$graph_days.'</td>
		</tr>';
		
		$out['stats']='
		<h3>Статистика на сегодня:</h3>
		<table>
			<tbody>
				</tr>
					<td width="30%">День займа</td>
					<td>'.$out['day'].'&nbsp;день</td>
				<tr>
				</tr>
					<td>Набежавший процент (в %)</td>
					<td>'.$out['percent_sum'].'%</td>
				<tr>
				</tr>
					<td>Набежавший процент (в рублях)</td>
					<td>'.((int)round($out['percent_rub_sum'])).'&nbsp;р.</td>
				<tr>
				</tr>
					<td>Невыплаченное тело займа</td>
					<td>'.((int)round($out['amount'])).'&nbsp;р.</td>
				<tr>
				</tr>
					<td>Невыплаченное тело займа с процентами</td>
					<td>'.((int)round($out['amount_full'])).'&nbsp;р.</td>
				<tr>
				</tr>
					<td>Просрочено дней</td>
					<td>'.$expired_days.'</td>
				<tr>
			</tbody>
		</table>
		<a class="more-stats" href="#">Подробности</a>
		';

		$out['last'] = array(
			'date' => $out['date'],
			'day' => $out['day'],
			'percent' => $out['percent'],
			'percent_month' => $out['percent_month'],
			'percent_year' => $out['percent_year'],
			'percent_rub' => $out['percent_rub'],
			'percent_sum' => $out['percent_sum'],
			'percent_rub_sum' => $out['percent_rub_sum'],
			'amount' => $out['amount'],
			'amount_full' => $out['amount_full'],
			'expired_days' => $out['expired_days']
		);
		
		$date = date_create($date);
		date_add($date, date_interval_create_from_date_string('1 days'));
		
		if ($i == 9999) {
			$this->error_tpl('Падение системы =(', 'Похоже, что функция расчета статистики ушла в бесконечный цикл. Сообщите о проблеме программисту.');
		}
		
		// echo $i.' = '.$days.'<br>';
	}
	
	$out['output'] ='
	<div id="more-stats" style="display:none;">
		<h3>Расчет статистики по дням:</h3>
		<table>
			<thead>
				<td>Дата</td>
				<td>День займа</td>
				<td>Дневной процент (в %)</td>
				<td>Дневной процент (в рублях)</td>
				<td>Набежавший процент (в %)</td>
				<td>Набежавший процент (в рублях)</td>
				<td>Сумма</td>
				<td>Сумма с процентами</td>
				<td>Платежи</td>
				<td>Продления</td>
				<td>Графики выплат</td>
			</thead>
			<tbody>
				'.$out['output'].'
			</tbody>
		</table>
	</div>
	';
	
	//echo '<pre>'; var_export($out['close_days']); echo '</pre>'; // отладка
	return $out;
}

// полный пересчет даты возврата с учетом продлений
function recount_back_date($loan_id) {
	$loan_id = $this->form_text($loan_id, 'db');
	$query = mysql_query("SELECT `till` FROM `wp_loans_term_adds` WHERE `loan_id`=$loan_id ORDER BY `till` DESC LIMIT 1");
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	if ($row['till']) {
		$back_date = $row['till'];
	} else {
		$query = mysql_query("SELECT `pay_date`, `term` FROM `wp_loans` WHERE `id`=$loan_id");
		@$row = mysql_fetch_array($query, MYSQL_ASSOC);
		// займ не выдан
		if ($row['pay_date'] == '0000-00-00 00:00:00') {
			$back_date = '0000-00-00 00:00:00';
		} else {
			$back_date = date_create($row['pay_date']);
			$back_date = date_add($back_date, date_interval_create_from_date_string(($row['term']-1).' days'));
			$back_date = date_format($back_date, 'Y-m-d H:i:s');
		}
	}
	mysql_query("UPDATE `wp_loans` SET `back_date`='$back_date' WHERE `id`=$loan_id");
}

function get_loan_periods($loan_id) {
	$loan_id = $this->form_text($loan_id, 'db');
	$periods = array();
	$periods[0] = array();
	$query = mysql_query("SELECT * FROM `wp_loans_term_adds` WHERE `loan_id`=$loan_id ORDER BY `till`");
	while ($row3 = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$periods[] = array(
			'from' => $row3['from'],
			'till' => $row3['till']
		);
	}
	
	$query = mysql_query("SELECT `pay_date`, `term` FROM `wp_loans` WHERE id=$loan_id");
	$row = mysql_fetch_array($query, MYSQL_ASSOC);
	
	$back_date = date_create($row['pay_date']);
	$back_date = date_add($back_date, date_interval_create_from_date_string(($row['term']-1).' days'));
	$back_date = date_format($back_date, 'Y-m-d H:i:s');
	
	$periods[0] = array(
		'from' => $row['pay_date'],
		'till' => $back_date
	);
	return $periods;
}

function get_last_doc_bac_date($loan_id) {
	$periods = $this->get_loan_periods($loan_id);
	return $periods[count($periods)-1]['till'];
}

function see_today_into_tomorrow($date_now) {
	list($year_now, $month_now, $day_now) = explode('-', $date_now);
	$days = cal_days_in_month(CAL_GREGORIAN, $month_now, $year_now);
	
	// смотрим сегодня в завтрашний день
	if ((int)$day_now < $days) {
		$day_next = (int)$day_now + 1;
		$month_next = $month_now;
		$year_next = $year_now;
	} else {
		$day_next = 1;
		$month_next = (int)$month_now + 1;
		$year_next = $year_now;
		if ($month_next == 13) {
			$month_next = 1;
			$year_next = (int)$year_now + 1;
		}
	}
	if (strlen($month_next) < 2) {
		$month_next = '0'.$month_next;
	}
	if (strlen($day_next) < 2) {
		$day_next = '0'.$day_next;
	}
	$date_next = $year_next.'-'.$month_next.'-'.$day_next;
	
	// смотрим сегодня во вчерашний день
	if ((int)$day_now > 1) {
		$day_prev = (int)$day_now - 1;
		$month_prev = $month_now;
		$year_prev = $year_now;
	} else {
		$month_prev = (int)$month_now - 1;
		$year_prev = $year_now;
		if ($month_prev == 0) {
			$month_prev = 12;
			$year_prev = (int)$year_now - 1;
		}
		$day_prev = cal_days_in_month(CAL_GREGORIAN, $month_prev, $year_prev);
	}
	if (strlen($month_prev) < 2) {
		$month_prev = '0'.$month_prev;
	}
	if (strlen($day_prev) < 2) {
		$day_prev = '0'.$day_prev;
	}
	$date_prev = $year_prev.'-'.$month_prev.'-'.$day_prev;
	
	$out = array(
		'next' => $date_next,
		'prev' => $date_prev
	);
	return $out;
}

function days_to_stupid_russian($days) {
	// склоняем дни
	$lasn_numb = substr($days, -1);
	switch ($lasn_numb) {
		default:
			$exp_days =' дней';
		break;
		case 1:
			$exp_days =' день';
		break;
		case 2:
		case 3:
		case 4:
			$exp_days =' дня';
		break;
	}
	if (in_array($days, array(11,12,13,14))) {
		$exp_days =' дней';
	}
	
	return $days.$exp_days;
}

function get_user_score($user_id, $update=false) {
	// Достаем звездочки
	$votes = 0;
	$rating = 0;
	$query = mysql_query("SELECT score FROM `wp_loans_users_votes` WHERE user_id=$user_id");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$votes++;
		$rating = $rating + $row['score'];
	}
	if ($votes) {
		$score = $rating/$votes;
		$score = round($score, 2);
	} else {
		$score = 0;
	}
	
	if ($update) {
		mysql_query("UPDATE `wp_loans_users` SET `score`=$score WHERE id=$user_id");
	}

	return $this->score_to_stars($score, $user_id);
}

function score_to_stars($score, $user_id) {
	$worker_score = '';
	$score = (double)$score;
	$votable = ' voteble';
	
	if (!$score) {
		$score = 0;
	} else {
		if ($this->user_id) {
			$query = mysql_query("SELECT score FROM `wp_loans_users_votes` WHERE `worker_id`={$this->user_id} AND `user_id`=$user_id");
			@$row = mysql_fetch_array($query, MYSQL_ASSOC);
			
			if ($row['score']) {
				$worker_score .= 'Ваша оценка: '.$row['score'];
				//$votable = '';
			}
		}
		
		$query = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans_users_votes` WHERE `user_id`=$user_id");
		@$row = mysql_fetch_array($query, MYSQL_ASSOC);
		if ($worker_score) {
			$worker_score .= ', ';
		}
		$worker_score .= 'Всего оценок: '.$row['numb'];
	}

	
	$score = round(20*$score, 2); //переводим оценку в проценты
	if ($score > 100) { $score = 100; }
	$label = round((5*$score/100), 1);
	if ($label == 0) {
		$label = 'Рейтинг не присвоен';
	} else {
		$label = 'Рейтинг: '.$label;
	}
	
	$content = '
	<div class="rating'.$votable.'" title="'.$label.'">
		<div class="rate" style="width:'.$score.'%"></div>
		<ul>
			<li name="rate1"></li>
			<li name="rate2"></li>
			<li name="rate3"></li>
			<li name="rate4"></li>
			<li name="rate5"></li>
		</ul>
	</div>
	<span class="user-score-label">'.$worker_score.'</span>';
	return $content;
}

// Функция вытаскивания сложных столбцов, которые не вытащить первым SQL-запросом
// $stats это кэш статистики, уменьшает количество запросов в БД в ынацать раз, где ынацать = количеству выводимых столбцов
function get_hard_field2($field, $id, $stats=false) {
	$id = mysql_real_escape_string($id);
	
	switch ($field) {
		default:
			$out = array(); // сигнал о том, что поле не относится к "сложным"
		break;
		case 'last_comment4':
		case 'last_comment4_full':
			$query = mysql_query("SELECT *, `date` AS `comment_date`, `text` AS `$field` FROM `wp_loans_comments` WHERE loan_id={$id} AND `comment_type` IN(4,5,6) ORDER BY `date` DESC LIMIT 1");
			$row = mysql_fetch_array($query, MYSQL_ASSOC);
			$row['real'] = $row;
			if (!$row[$field]) {
				$row[$field] = '';
			}
			$row['loan_id'] = $id;
			$row = $this->loans_translate_vals_4list($row);
			$out['pretty'] = $row[$field];
		break;
		case 'expired_days':
			if (!$stats) {
				$row = $this->get_main_row($id);
				$stats = $this->get_buh_stats($row, 'now');
			}
			$out['real'] = $stats['expired_days'];
			$out['pretty'] = $this->days_to_stupid_russian($out['real']);
		break;
		case 'stat_now_amount':
			if (!$stats) {
				$row = $this->get_main_row($id);
				$stats = $this->get_buh_stats($row, 'now');
			}
			$out['real'] = $stats['amount'];
			$out['pretty'] = number_format($stats['amount'], 0, '.', ' ').' р.';
		break;
		case 'stat_now_amount_full':
			if (!$stats) {
				$row = $this->get_main_row($id);
				$stats = $this->get_buh_stats($row, 'now');
			}
			$out['real'] = $stats['amount_full'];
			$out['pretty'] = number_format($stats['amount_full'], 0, '.', ' ').' р.';
		break;
		case 'stat_now_percent_sum':
			if (!$stats) {
				$row = $this->get_main_row($id);
				$stats = $this->get_buh_stats($row, 'now');
			}
			$out['real'] = $stats['percent_sum'];
			$out['pretty'] = $stats['percent_sum'].'%';
		break;
		case 'stat_now_percent_rub_sum':
			if (!$stats) {
				$row = $this->get_main_row($id);
				$stats = $this->get_buh_stats($row, 'now');
			}
			$out['real'] = $stats['percent_rub_sum'];
			$out['pretty'] = number_format($stats['percent_rub_sum'], 0, '.', ' ').' р.';
		break;
	}
	
	return $out;
}

function send_sms($loan_ids, $text_type, $force_sms_id=0, $worker_id=0, $force_text='', $force_phone=0) {
	if (substr_count($_SERVER['SCRIPT_FILENAME'], 'Z:/home/')) {
		return false;
	}

	if (!is_array($loan_ids)) {
		$loan_ids = array($loan_ids);
	}
	
	if (!count($loan_ids)) {
		return false;
	}
	
	$sender = 'MONEY FUNNY';
	$login = 'z130330904';
	$password = '846218';
	$messages_200_arr = array();
	$counter = 0;
	$inner_counter = 0;

	foreach ($loan_ids as $loan_id) {
		// проверка включены ли SMS-оповещения
		$query = mysql_query("SELECT `sms_off`, `fake` FROM `wp_loans` WHERE `id`=$loan_id");
		@$row = mysql_fetch_array($query, MYSQL_ASSOC);
		if ($row['sms_off']) {
			continue;
		}
		if ($row['fake']) {
			continue;
		}
		
		// достаём телефон
		$loan_id = $this->form_text($loan_id, 'db');
		
		if ($force_phone) {
			$phone = $force_phone;
		} else {
			$phones_arr = $this->get_sms_phones($loan_id);
			if (!count($phones_arr)) {
				continue;
			}
			$phone = $phones_arr[0];
		}
		
		// предохранитель от кривых рук (моих)
		if (!$phone) {
			continue;
		}
		
		// формирование сообщения
		switch ($text_type) {
			case 1:
				$text = 'По вашей заявке принято отрицательное решение, спасибо за обращение.';
			break;
			case 2:
				$text = 'Ваша заявка предварительно одобрена, обратитесь в офис компании';
				
				$query = mysql_query("SELECT u.`branch` FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE u.id = l.user_id AND l.`id`=$loan_id LIMIT 1");
				@$row = mysql_fetch_array($query, MYSQL_ASSOC);
				
				if ($row['branch']) {
					$query = mysql_query("SELECT `phones` FROM `wp_loans_branches` WHERE id={$row['branch']}");
					@$row = mysql_fetch_array($query, MYSQL_ASSOC);
					if ($row['phones']) {
						$text.=' или по тел. '.$row['phones'];
					}
				}
				$text .= '.';
			break;
			case 3:
				$row2 = $this->get_main_row($loan_id);
				$stats = $this->get_buh_stats($row2);
				$text = 'Напоминаем вам о фиксированном платеже '.$stats['last']['amount_full'].'р. '.$this->get_time($row['back_date'], 'tiny_dig', true);
			break;
			case 4:
				$row = $this->get_main_row($loan_id);
				$stats = $this->get_buh_stats($row, 'now');
				
				$text = 'Ваш платеж прошел успешно, сумма задолжности на '.$this->get_time(gmdate('Y-m-d H:i:s'), 'tiny_dig', true).', '.$stats['last']['amount_full'].'р.';
			break;
			case 5:
				$text = $force_text;
			break;
			case 6:
				$text = 'К оплате на завтра: '.$force_text[$loan_id].' р.';
			break;
		}
		//По вашей заявке принято отрицательное решение, спасибо за обращение.
		//Ваша заявка пред.одобрена, обратитесь в офис или по тел. +79166633226
		
		//if (mb_strlen($text) > 70) {
		//	$text = mb_substr($text, 0, 70);
		//}
		
		// формирование id сообщения (нужно для API)
		$now = gmdate('Y-m-d H:i:s');
		$text_sql = $this->form_text($text, 'db');
		
		if ($worker_id) {
			$worker_id = ", `worker_id`=$worker_id";
		} else {
			$worker_id = '';
		}
		
		if ($force_sms_id) {
			$sms_id = $force_sms_id;
			mysql_query("UPDATE `wp_loans_sms` SET `text`='$text_sql', `date`='$now', `phone`='$phone'$worker_id WHERE `id`=$force_sms_id");
		} else {
			mysql_query("INSERT INTO `wp_loans_sms` SET `loan_id`=$loan_id, `text_type`=$text_type, `text`='$text_sql', `date`='$now', `phone`='$phone'$worker_id");
			echo mysql_error();
			$sms_id = mysql_insert_id();
		}
		
		$inner_counter++;
		if ($inner_counter > 200) {
			$inner_counter = 0;
			$counter++;
		}
		
		$messages_200_arr[$counter][] = '{
			"phone": "'.$phone.'",
			"clientId": "'.$sms_id.'",
			"text": "'.$text.'",
			"sender": "'.$sender.'"
		}';
		
		$messages_arrays[$sms_id] = array(
			"phone" => $phone,
			"clientId" => $sms_id,
			"text" => $text
		);
	}
	
	foreach ($messages_200_arr as $messages) {
		
		if (!count($messages)) {
			return false;
		}
		
		$messages = implode(',', $messages);
		
		// формирование запроса под API
		$data = '{
			"login": "'.$login.'",
			"password": "'.$password.'",
			"messages": ['.$messages.']
		}';
		
		//echo $data; // отладка
		//die;
		
		$options = array(
			'http' => array(
				'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
				'method'  => 'POST',
				'content' => $data,
			),
		);
		
		// отправка
		$context  = stream_context_create($options);
		$result = file_get_contents('http://api.iqsms.ru/messages/v2/send.json', false, $context);
		$result = json_decode($result, true);
		
		if ($result['messages']) {
			foreach ($result['messages'] as $message) {
				$message['status'] = $this->form_text($message['status'], 'db');
				$message['clientId'] = $this->form_text($message['clientId'], 'db');
				$server = '';
				if ($message['smscId']) {
					$message['smscId'] = $this->form_text($message['smscId'], 'db');
					$server = ", `server_id`='{$message['smscId']}'";
				}
				mysql_query("UPDATE `wp_loans_sms` SET `status`='{$message['status']}'$server WHERE id='{$message['clientId']}'");
			}
		} else {
			foreach ($messages_arrays as $message) {
				if ($result['description']) {
					$mess = $result['description'];
				} else {
					$mess = $result['status'];
				}
				
				$mess = $this->form_text($mess, 'db');
				mysql_query("UPDATE `wp_loans_sms` SET `status`='$mess' WHERE id={$message['clientId']}");
			}
		}
	}

	return true;
}



function send_sms_simple($text, $phone) {
	if (substr_count($_SERVER['SCRIPT_FILENAME'], 'Z:/home/')) {
		return false;
	}
	
	$now = gmdate('Y-m-d H:i:s');
	mysql_query("INSERT INTO `wp_loans_sms` SET `loan_id`=0, `text_type`=7, `text`='$text', `date`='$now', `phone`='$phone'");
	echo mysql_error();
	$sms_id = mysql_insert_id();
	
	$sender = 'MONEY FUNNY';
	$login = 'z130330904';
	$password = '846218';

	$message = '{
		"phone": "'.$phone.'",
		"clientId": "'.$sms_id.'",
		"text": "'.$text.'",
		"sender": "'.$sender.'"
	}';
	
	
	// формирование запроса под API
	$data = '{
		"login": "'.$login.'",
		"password": "'.$password.'",
		"messages": ['.$message.']
	}';

	$options = array(
		'http' => array(
			'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
			'method'  => 'POST',
			'content' => $data,
		),
	);
	
	// отправка
	$context  = stream_context_create($options);
	$result = file_get_contents('http://api.iqsms.ru/messages/v2/send.json', false, $context);
	$result = json_decode($result, true);
	unset($message);
	
	//echo '<pre>'; var_export($result); echo '</pre>'; // отладка
	//die;
	
	if ($result['messages']) {
		foreach ($result['messages'] as $message) {
			$message['status'] = $this->form_text($message['status'], 'db');
			$message['clientId'] = $this->form_text($message['clientId'], 'db');
			$server = '';
			if ($message['smscId']) {
				$message['smscId'] = $this->form_text($message['smscId'], 'db');
				$server = ", `server_id`='{$message['smscId']}'";
			}
			mysql_query("UPDATE `wp_loans_sms` SET `status`='{$message['status']}'$server WHERE id='{$message['clientId']}'");
		}
	} else {
		if ($result['description']) {
			$mess = $result['description'];
		} else {
			$mess = $result['status'];
		}
		
		$mess = $this->form_text($mess, 'db');
		mysql_query("UPDATE `wp_loans_sms` SET `status`='$mess' WHERE id=$sms_id");
	}

	return true;
}


function show_sms($loan_id, $page=1) {
	$per_page = 5;
	
	$query = mysql_query("SELECT COUNT(`id`) AS `numb` FROM `wp_loans_sms` WHERE `loan_id`=$loan_id");
	$row = mysql_fetch_array($query, MYSQL_ASSOC);
	$num_pages=ceil($row['numb']/$per_page);
	$limit = $per_page*($page-1).', '.$per_page;

	$query = mysql_query("SELECT * FROM `wp_loans_sms` WHERE `loan_id`=$loan_id ORDER BY `date` DESC LIMIT $limit");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$row['text'] = $this->form_text($row['text']);
		$row['status'] = $this->form_text($row['status']);
		$row['date'] = $this->get_time($row['date'], 'full', true);
		
		switch ($row['status']) {
			case 'accepted':
				$row['status'] = '<span class="text-green">Отправлено</span>';
			break;
			default:
				if ($row['status']) {
					$row['status'] = '<span class="text-red">Ошибка: '.$row['status'].'</span>';
				}
				if (current_user_can('loan_sms')) {
					$row['status']='<a href="#" class="jsResendSMS fancy-tip" id="sms-'.$row['id'].'" title="Отправить еще раз"></a> '.$row['status'];
				}
			break;
		}
		if ($row['worker_id']) {
			$row['worker_id'] = $this->get_staff_name($row['worker_id'], false, true, 'initials');
		} else {
			$row['worker_id'] = 'Робот';
		}
		
		
		$content.='
		<tr>
			<td>'.$row['date'].'</td>
			<td>'.$row['phone'].'</td>
			<td>'.$row['text'].'</td>
			<td>'.$row['status'].'</td>
			<td>'.$row['worker_id'].'</td>
		</tr>';
	}
	
	if (current_user_can('loan_sms')) {
		$moderate = '<a href="#" class="typ-btn jsSendSMS">Отправить SMS</a>';
	}
	
	if ($content) {
		$content = '
		<div class="jsSmsWrap">
			<table>
				<thead>
				<tr>
					<td>Дата</td>
					<td>Номер</td>
					<td>Текст сообщения</td>
					<td>Статус</td>
					<td>Отправил</td>
				</tr>
				</thead>
				<tbody>
					'.$content.'
				</tbody>
			</table>
			'.$moderate.'
			'.$this->show_pages($num_pages, $page, $per_page, 'jsSmsListPages').'
			<div class="clear"></div>
		</div>';
	} else {
		$content = '<div class="white-wrap jsSmsWrap">Пока нет оповещений.</div>
		'.$moderate.'
		<div class="clear"></div>';
	}
	
	
	
	return $content;
}

function get_sms_phones($loan_id) {
	$query = mysql_query("SELECT u.`phone1`, u.`phone2` FROM `wp_loans` AS `l`, `wp_loans_users` AS `u` WHERE u.id = l.user_id AND l.`id`=$loan_id LIMIT 1");
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	
	$phone1_check = $phone2_check = true;
	
	// оставляем только цифры
	$row['phone1'] = preg_replace("/[^0-9]/", '', $row['phone1']);
	$row['phone2'] = preg_replace("/[^0-9]/", '', $row['phone2']);
	
	// проверка длины - самый короткий вариант мобильного: 9994442211
	if (strlen($row['phone1']) < 10) {
		$phone1_check = false;
	}
	if (strlen($row['phone2']) < 10) {
		$phone2_check = false;
	}
	
	// берем последние 10 символов (без 7 или 8 вначале)
	$row['phone1'] = substr($row['phone1'], -10);
	$row['phone2'] = substr($row['phone2'], -10);
	
	// проверка городской или мобильный
	if (in_array(substr($row['phone1'], 0, 3), array('495', '499'))) {
		$phone1_check = false;
	}
	if (in_array(substr($row['phone2'], 0, 3), array('495', '499'))) {
		$phone2_check = false;
	}

	$arr = array();
	if ($phone1_check) {
		$arr[] = '7'.$row['phone1'];
	}
	if ($phone2_check) {
		$arr[] = '7'.$row['phone2'];
	}
	return $arr;
}

function send_validate_mail($old_validate, $new_validate, $email) {
	
	$old_validate = $this->translate_validate_4user($old_validate);
	$new_validate = $this->translate_validate_4user($new_validate);
	
	if ($old_validate != $new_validate) {
		$msg = 'Ваша заявка на микрозайм получила статус: '.strip_tags($new_validate);
		$this->send_email($email, $msg, 'Статус заявки изменен');
	}
}

function get_pay_graph($loan_id, $get_all=false) {
	$where = '';
	if (!$get_all) {
		
	}
	$query = mysql_query("SELECT `doc_numb` FROM `wp_loans_payments_graph` WHERE loan_id=$loan_id ORDER BY `doc_numb` DESC LIMIT 1");
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	$numb = $row['doc_numb'];
	
	$arr = array();
	$query = mysql_query("SELECT * FROM `wp_loans_payments_graph` WHERE `loan_id`=$loan_id ORDER BY `pay_date`");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$arr[$row['create_date']][] = $row;
	}
	unset($row);
	
	//echo '<pre>'; var_export($arr); echo '</pre>'; die; // отладка
	
	$output = $table = '';
	$graph_count = 0;
	$arr_numb = array();
	$overpay = 0;
	
	// создаём не ассоциативный массив графиков с датой начала графика
	foreach ($arr as $create_date => $rows) {
		$arr_numb[] = $create_date;
	}
	
	foreach ($arr as $create_date => $rows) {
		
		$content = '';
		$payed_full = $overpay;
		unset($prev_pay_date);
		$first_row = true;
		
		// находим платежи за каждый периуд
		$last_pay = count($rows)-1;
		foreach ($rows as $k => $row) {
			$till = explode(' ', $row['pay_date']);
			$till = $till[0].' 23:59:59';
			if ($prev_pay_date) {
				$from = $prev_pay_date;
			} else {
				$from = $this->utc_to_local($row['create_date']);
			}
			
			// для последней строки графика, 
			if ($k == $last_pay) {
				/*
				if ($arr_numb[$graph_count+1]) {
					// платежи можно принимать до даты начала следующего графика
					$till = $arr_numb[$graph_count+1];
					$till = $this->utc_to_local($till);
					$till = explode(' ', $till);
					$till = $till[0];
					$till = date_create($till);
					date_add($till, date_interval_create_from_date_string('-1 days'));
					$till = date_format($till, 'Y-m-d').' 23:59:59';
					
				} else {
					// или до бесконечности, если это последний график
					$till = gmdate('Y-m-d H:i:s');
					$till = date_create($till);
					date_add($till, date_interval_create_from_date_string('100 days'));
					$till = date_format($till, 'Y-m-d H:i:s');
				}
				*/
				// новая система, вводим из-за НБКИ - платежи принимаем только до крайнего дня выплаты последнего месяца
				$till = $row['pay_date'];
				$till = explode(' ', $till);
				$till = $till[0].' 23:59:59';
			}
			
			// чтобы даты не пересекались
			if (!$first_row) {
				$from = date_create($from);
				date_add($from, date_interval_create_from_date_string('1 days'));
				$from = date_format($from, 'Y-m-d H:i:s');
			}
			
			$from = explode(' ', $from);
			$from = $from[0].' 00:00:00';
			
			$arr[$create_date][$k]['from'] = $from;
			$arr[$create_date][$k]['till'] = $till;
			
			$payed = $this->get_payments_from_term($loan_id, $from, $till);
			
			$arr[$create_date][$k]['payed_real'] = $payed;
			
			$payed_full = $payed_full + $payed;
			$prev_pay_date = $row['pay_date'];
			$first_row = false;
		}
		$payed_full_bac = $payed_full;
		
		// заполняем график выплат поочередно платежами
		$payments = array();
		$need_to_pay = 0;
		foreach ($rows as $k => $row) {
			if ($payed_full > 0) {
				$payed_full_this = $payed_full - $row['amount'];
				if ($payed_full_this >= 0) {
					$payed_full = $payed_full_this;
					$payed = $row['amount'];
				} else {
					$payed = $payed_full;
					$payed_full = 0;
				}
			} else {
				$payed = 0;
				$payed_full = 0;
			}
			
			$payments[$k] = $payed;
			$need_to_pay = $need_to_pay + $row['amount'];
			
		}
		
		// находим переплату за последний день
		if ($payed_full_bac > $need_to_pay) {
			// если этот график НЕ последний
			if ($arr_numb[$graph_count+1]) {
				//переносим переплату на следующий график
				$overpay = $payed_full_bac - $need_to_pay;
			} else {
				// иначе, выводим переплату в последнюю строку
				$payments[count($payments)-1] = $payments[count($payments)-1] + $payed_full_bac - $need_to_pay;
			}
		}
		
		$n = 0;
		foreach ($rows as $k => $row) {
			$arr[$create_date][$k]['payed'] = $payments[$k];
			$rows[$k]['payed'] = $payments[$k];
			$n++;
			$class= '';
			if (($row['closed']) and ($payments[$k] == 0)) {
				$class = 'back-red';
			}
			
			// просрочен ли периуд (для статистики буха)
			$arr[$create_date][$k]['expired'] = 0;
			if ($payments[$k] == 0) {
				$arr[$create_date][$k]['expired'] = 1;
			}
			if ($row['till'] > $now) {
				$arr[$create_date][$k]['expired'] = 0;
			}
			
			$content .='
			<tr class="'.$class.'">
				<td>'.$n.'</td>
				<td>'.$this->get_time($row['pay_date'], 'compact').'</td>
				<td>'.number_format($row['amount'], 0, '.', ' ').' р.</td>
				<td>'.number_format($payments[$k], 0, '.', ' ').' р.</td>
			</tr>';
		}
		
		if ((current_user_can('loan_stop_percent')) and ($row['doc_numb'] == $numb)) {
			if (!$row['locked']) {
				$moderate = ' <a href="#" title="Удалить" class="jsDeleteGraph fancy-tip delete-btn" id="graph-'.$create_date.'"></a>';
			} else {
				if (!$row['closed']) {
					$moderate = ' <a href="#" title="Закрыть" class="jsCloseGraph fancy-tip close-btn" id="graph-'.$create_date.'"></a>';
				} else {
					$moderate = ' <a href="#" title="Открыть" class="jsReOpenGraph fancy-tip unclose-btn" id="graph-'.$create_date.'"></a>';
				}
			}
		}

		$table = '
		<h3>График выплат от '.$this->get_time($create_date, 'tiny_dig').''.$moderate.'</h3>
		<table>
			<thead>
			<tr>
				<td>#</td>
				<td>День выплаты</td>
				<td>Сумма к оплате</td>
				<td>Выплаченная сумма</td>
			</tr>
			</thead>
			<tbody>
				'.$content.'
			</tbody>
		</table>
		';
		
		$output .= $table;
		$graph_count++;
	}
	
	// возвращаем последний график - нужно для просчета уплачен ли долг по определенной выплате (используется перед SMS-оповещениями)
	$out = array();
	$out['raw'] = $arr;
	$out['last_graph'] = $rows;
	$out['html'] = $output;
	//echo '<pre>'; var_export($arr); echo '</pre>';die; // отладка

	return $out;
}

// Формат дат: Y-m-d H:i:s или Y-m-d
function is_first_date_bigger($first_date, $second_date) {
	$first_date = date_create($first_date);
	$second_date = date_create($second_date);
	$interval = date_diff($first_date, $second_date);
	if (!$interval->invert) {
		return false;
	} else {
		return true;
	}
}

function add_payment_to_graph($amount, $loan_id, $date) {
	
	$rows = array();
	$n = 0;
	$query = mysql_query("SELECT * FROM `wp_loans_payments_graph` WHERE `loan_id`=$loan_id ORDER BY `id`");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		if (!$last_create_date) {
			$last_create_date = $row['create_date'];
		}
		if ($last_create_date != $row['create_date']) {
			$n++;
		}
		
		$rows[$n][] = $row;
	}
	unset($row);
	
	// берем только последний график
	$rows = $rows[count($rows)-1];
	
	/*
	foreach ($rows as $row) {
		$amount_need = $row['amount'] - $row['amount_payed'];
		if ($amount_need <= 0) {
			continue;
		}
		
		if ($amount_need > $amount) {
			$new_amount = $row['amount_payed'] + $amount;
			mysql_query("UPDATE `wp_loans_payments_graph` SET `amount_payed`='$new_amount' WHERE id={$row['id']}");
			break;
		} else {
			$amount = $amount - $amount_need;
			mysql_query("UPDATE `wp_loans_payments_graph` SET `amount_payed`='$amount_need' WHERE id={$row['id']}");
		}
	}
	*/
}

function get_next_doc_numb($loan_id) {
	$query = mysql_query("SELECT `doc_numb` FROM `wp_loans_term_adds` WHERE `loan_id`=$loan_id ORDER BY `doc_numb`");
	while($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		if ($row['doc_numb']) {
			$numbs[] = $row['doc_numb'];
		}
	}
	
	$query = mysql_query("SELECT `doc_numb` FROM `wp_loans_payments_graph` WHERE `loan_id`=$loan_id GROUP BY `create_date` ORDER BY `doc_numb`");
	while($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		if ($row['doc_numb']) {
			$numbs[] = $row['doc_numb'];
		}
	}
	
	if (!$numbs) {
		return 3;
	}
	
	sort($numbs);
	$last_numb = $numbs[count($numbs)-1];
	
	if ($last_numb < 3) {
		return 3;
	}
	
	$n = 2;
	while($n < $last_numb) {
		$n++;
		if (!in_array($n, $numbs)) {
			$free_numb = $n;
			break;
		}
	}

	if (($free_numb) and ($free_numb > 2)) {
		return $free_numb;
	} else {
		$last_numb++;
		return $last_numb;
	}
}

// в функцию приходит локальное время
function get_payments_from_term($loan_id, $from, $till) {
	$amount = 0;
	$query = mysql_query("SELECT * FROM `wp_loans_payments` WHERE loan_id=$loan_id");
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$row['date'] = $this->utc_to_local($row['date']);
		if ((($this->is_first_date_bigger($row['date'], $from)) or ($row['date'] == $from)) and (($this->is_first_date_bigger($till, $row['date'])) or ($till == $row['date']))) {
			$amount = $amount + $row['amount'];
		}
	}
	return $amount;
	//return $from.' - '.$till;
}

function create_fake($loan_id) {
	
	$query = mysql_query("SELECT `id` FROM `wp_loans` WHERE `fake`={$loan_id}");
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	if ($row['id']) {
		$this->wrap_json_error('Фейк этого займа уже существует.');
	}
	
	// проверяем нужно ли копировать юзера
	$sql = "SELECT `user_id` FROM `wp_loans` WHERE `id`={$loan_id}";
	$query = mysql_query($sql);
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	if ($_POST['ajaxing']) {
		$this->check_mysql_error();
	} else {
		$error = mysql_error();
		if ($error) {
			echo 'Ошибка в запросе: '.$sql.'<br>'.$error;
			die;
		}
	}
	$real_user_id = $row['user_id'];
	
	$sql = "SELECT `id` FROM `wp_loans_users` WHERE `fake`={$row['user_id']}";
	$query = mysql_query($sql);
	@$row = mysql_fetch_array($query, MYSQL_ASSOC);
	if ($_POST['ajaxing']) {
		$this->check_mysql_error();
	} else {
		$error = mysql_error();
		if ($error) {
			echo 'Ошибка в запросе: '.$sql.'<br>'.$error;
			die;
		}
	}
	
	if ($row['id']) {
		$fake_user_id = $row['id'];
	} else {
		$fields_arr = $fields_str = $values_str = array();
		// достаем ключи
		$query = mysql_query("SHOW COLUMNS FROM `wp_loans_users`");
		while($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$fields_arr[$row['Field']] = '`'.$row['Field'].'`';
		}
		unset($fields_arr['id']);
		$fields_str = implode(',', $fields_arr);
		
		// достаем значения
		$query = mysql_query("SELECT $fields_str FROM `wp_loans_users` WHERE id={$real_user_id}");
		$row = mysql_fetch_array($query, MYSQL_ASSOC);
		
		$row['fake'] = $real_user_id;
		foreach ($row as $key => $value) {
			$values_str[] = "'".$value."'";
		}
		$values_str = implode(',', $values_str);
		
		$sql = "INSERT INTO `wp_loans_users` ($fields_str) VALUES ($values_str)";
		mysql_query($sql);
		if ($_POST['ajaxing']) {
		$this->check_mysql_error();
		} else {
			$error = mysql_error();
			if ($error) {
				echo 'Ошибка в запросе: '.$sql.'<br>'.$error;
				die;
			}
		}
		$fake_user_id = mysql_insert_id();
	}
	
	// копируем сам займ
	$fields_arr = $fields_str = $values_str = array();
	// достаем ключи
	$query = mysql_query("SHOW COLUMNS FROM `wp_loans`");
	while($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		$fields_arr[$row['Field']] = '`'.$row['Field'].'`';
	}
	unset($fields_arr['id']);
	$fields_str = implode(',', $fields_arr);
	
	// достаем значения
	$query = mysql_query("SELECT $fields_str FROM `wp_loans` WHERE id={$loan_id}");
	$row = mysql_fetch_array($query, MYSQL_ASSOC);
	
	$row['user_id'] = $fake_user_id;
	$row['fake'] = $loan_id;
	foreach ($row as $key => $value) {
		$values_str[] = "'".$value."'";
	}
	$values_str = implode(',', $values_str);
	
	$sql = "INSERT INTO `wp_loans` ($fields_str) VALUES ($values_str)";
	mysql_query($sql);
	if ($_POST['ajaxing']) {
		$this->check_mysql_error();
	} else {
		$error = mysql_error();
		if ($error) {
			echo 'Ошибка в запросе: '.$sql.'<br>'.$error;
			die;
		}
	}
	$fake_loan_id = mysql_insert_id();

	// копируем опросники
	$fields_arr = $fields_str = array();
	$query2 = mysql_query("SELECT * FROM `wp_polls_results` WHERE `user_id`={$loan_id} AND ((`poll_id`=7) OR (`poll_id`=8))");
	@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
	if ($row2) {
		// достаем ключи
		$query = mysql_query("SHOW COLUMNS FROM `wp_polls_results`");
		while($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$fields_arr[$row['Field']] = '`'.$row['Field'].'`';
		}
		unset($fields_arr['id']);
		$fields_str = implode(',', $fields_arr);
		
		// достаем значения
		
		//echo "SELECT $fields_str FROM `wp_polls_results` WHERE id={$loan_id}";
		$query = mysql_query("SELECT $fields_str FROM `wp_polls_results` WHERE `user_id`={$loan_id} AND ((`poll_id`=7) OR (`poll_id`=8))");
		while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
			$values_str = array();
			$row['user_id'] = $fake_loan_id; // на самом деле туда пишется id займа
			foreach ($row as $key => $value) {
				$values_str[] = "'".$value."'";
			}
			$values_str = implode(',', $values_str);
			
			$sql = "INSERT INTO `wp_polls_results` ($fields_str) VALUES ($values_str)";
			mysql_query($sql);
			if ($_POST['ajaxing']) {
				$this->check_mysql_error();
			} else {
				$error = mysql_error();
				if ($error) {
					echo 'Ошибка в запросе: '.$sql.'<br>'.$error;
					die;
				}
			}
		}
		
	}
	return $fake_loan_id;
}

// Вывод кредитной истории
function get_user_ki($user_id) {
	$query = mysql_query("SELECT * FROM `wp_loans_users_ki` WHERE user_id=$user_id ORDER BY `date` DESC");
	$first = true;
	while ($row = mysql_fetch_array($query, MYSQL_ASSOC)) {
		
		if ($first) {
			$style = '';
			$class = 'active';
		} else {
			$style = 'style="display:none;"';
			$class = '';
		}
		
		$class2 = str_replace(':', '-', $row['date']);
		$class2 = str_replace(' ', '-', $class2);
		
		$row['text'] = $this->parse_user_ki($row);
		
		$nav.= '<a class="typ-btn '.$class.'" id="ki-'.$class2.'">'.$this->get_time($row['date'], 'full_dig', true).'</a>';
		$content.= '<div '.$style.' class="ki-wrap ki-'.$class2.'">'.$row['text'].'</div>';
		
		$first=false;
	}
	
	if (!$content) {
		$content= '
		<div class="white-wrap">
			Кредитная история ещё не проверялась.
		</div>';
	} else {
		$content = '<div class="jsKiTabs">'.$nav.'</div>'.$content;
	}
	
	$now = gmdate('Y-m');
	$query33 = mysql_query("SELECT COUNT(id) AS `numb` FROM `wp_loans_users_ki` WHERE `date` LIKE '$now%'");
	@$row33 = mysql_fetch_array($query33, MYSQL_ASSOC);
	
	
	//if ($this->user_id == 21) {
		$content = '<a class="typ-btn fancy-tip jsPullKi" title="Всего запрошено кредитных историй в этом месяце: '.$row33['numb'].'">Запросить</a>'.$content;
	//}
	$content = '<div class="kiAjaxContainer">'.$content.'</div>';
	
	return $content;
}

// превращает XML в читабельный формат
function parse_user_ki($data) {
	$path = $this->site_dir.'wp-content/uploads/ki/';
	if (!file_exists($path.$data['file'].'.xml')) {
		return 'Файл не найден: '.$path.$data['file'].'.xml';
	}
	
	$xml = file_get_contents($path.$data['file'].'.xml');
	
	$xml = explode("<preply>", $xml);
	$xml = $xml[1];
	$xml = explode("</preply>", $xml);
	$xml = trim($xml[0]);
	$xml_sourse = htmlspecialchars($xml);
	$xml_sourse = nl2br($xml_sourse);
	
	$xml = '<?xml version="1.0"?>
<mfiResponse>
  '.trim($xml).'
</mfiResponse>';
	
	$xml = simplexml_load_string($xml);
	$json = json_encode($xml);
	$row = json_decode($json,TRUE);
	$row_all = $row;
	$row = $row['report'];
	
	
/* Вывод полей макаронами, как привык начальник -_-
------------------------------------------------------------------------------*/
if ($row['inqControlNum']) {
	$out = '
	<table class="ki-table">
		<tbody>
		<tr>
			<td align="center"><div class="text-navy">Код запроса</div>'.$this->ki_translate_vals($row['inqControlNum']).'</td>
			<td align="center"><div class="text-navy">Код участника</div>4L01GG000000</td>
			<td align="center"><div class="text-navy">Ссылка</div>none</td>
			<td align="center"><div class="text-navy">Предоставлен</div>'.$this->get_time($data['date'], 'ki_out').'</td>
		</tr>
		</tbody>
	</table>
	
	<div class="section-separator"><p></p></div>';
}

// ФИО
if ($row['PersonReply']) {
	$row1 = $row2 = '';
	$row2['birthDt'] = $row2['placeOfBirth'] = $row2['nationality'] = $row2['gender'] = $row2['maritalStatus'] = $row2['numDependants'] = array();
	if (!count($row['PersonReply'][0])) {
		$arr = array();
		$arr[0] = $row['PersonReply'];
		$row['PersonReply'] = $arr;
	}
	$n = 0;
	
	foreach ($row['PersonReply'] as $value) {
		$label = '';
		if ($n > 0) {
			$label = ' (дополнительно)';
		}
		switch ($value['gender']) {
			default:
				$value['gender'] = 'Нет данных';
			break;
			case 1:
				$value['gender'] = 'Мужкской';
			break;
			case 2:
				$value['gender'] = 'Женский';
			break;
		}
		
		if ($value['nationalityText']) {
			$value['nationality'] = $value['nationalityText'];
		}
		
		if ($value['maritalStatusText']) {
			$value['maritalStatus'] = $value['maritalStatusText'];
		}
		
		
		
		$row1.= '<p>'.$value['name1'].' '.$value['first'].' '.$value['paternal'].$label.'</p>';
		$row2['birthDt'][]=$value['birthDt'];
		$row2['placeOfBirth'][]=$value['placeOfBirth'];
		$row2['nationality'][]=$value['nationality'];
		$row2['gender'][]=$value['gender'];
		$row2['maritalStatus'][]=$value['maritalStatus'];
		$row2['numDependants'][]=$value['numDependants'];
		$n++;
	}
	
	
	
	$row2['birthDt'] = array_unique($row2['birthDt']);
	$row2['placeOfBirth'] = array_unique($row2['placeOfBirth']);
	$row2['nationality'] = array_unique($row2['nationality']);
	$row2['gender'] = array_unique($row2['gender']);
	$row2['maritalStatus'] = array_unique($row2['maritalStatus']);
	$row2['numDependants'] = array_unique($row2['numDependants']);
	
	$n = 0;
	foreach ($row2['birthDt'] as $key => $value) {
		if (is_array($value)) {
			$row2['birthDt'][$key] = implode('<br>', $row2['birthDt'][$key]);
		}
		if ($n > 0) {
			$row2['birthDt'][$key].=' (дополнительно)';
		}
		$n++;
	}
	$n = 0;
	foreach ($row2['placeOfBirth'] as $key => $value) {
		if (is_array($value)) {
			$row2['placeOfBirth'][$key] = implode('<br>', $row2['placeOfBirth'][$key]);
		}
		if ($n > 0) {
			$row2['placeOfBirth'][$key].=' (дополнительно)';
		}
		$n++;
	}
	$n = 0;
	foreach ($row2['nationality'] as $key => $value) {
		if (is_array($value)) {
			$row2['nationality'][$key] = implode('<br>', $row2['nationality'][$key]);
		}
		if ($n > 0) {
			$row2['nationality'][$key].=' (дополнительно)';
		}
		$n++;
	}
	$n = 0;
	foreach ($row2['gender'] as $key => $value) {
		if (is_array($value)) {
			$row2['gender'][$key] = implode('<br>', $row2['gender'][$key]);
		}
		if ($n > 0) {
			$row2['gender'][$key].=' (дополнительно)';
		}
		$n++;
	}
	$n = 0;
	foreach ($row2['maritalStatus'] as $key => $value) {
		if (is_array($value)) {
			$row2['maritalStatus'][$key] = implode('<br>', $row2['maritalStatus'][$key]);
		}
		if ($n > 0) {
			$row2['maritalStatus'][$key].=' (дополнительно)';
		}
		$n++;
	}
	$n = 0;
	foreach ($row2['numDependants'] as $key => $value) {
		if (is_array($value)) {
			$row2['numDependants'][$key] = implode('<br>', $row2['numDependants'][$key]);
		}
		if ($n > 0) {
			$row2['numDependants'][$key].=' (дополнительно)';
		}
		$n++;
	}
	
	$row2['birthDt'] = implode('<br>', $row2['birthDt']);
	$row2['placeOfBirth'] = implode('<br>', $row2['placeOfBirth']);
	$row2['nationality'] = implode('<br>', $row2['nationality']);
	$row2['gender'] = implode('<br>', $row2['gender']);
	$row2['maritalStatus'] = implode('<br>', $row2['maritalStatus']);
	$row2['numDependants'] = implode('<br>', $row2['numDependants']);
	if (!$row2['birthDt']) {
		$row2['birthDt'] = 'нет данных';
	}
	if (!$row2['placeOfBirth']) {
		$row2['placeOfBirth'] = 'нет данных';
	}
	if (!$row2['nationality']) {
		$row2['nationality'] = 'нет данных';
	}
	if (!$row2['gender']) {
		$row2['gender'] = 'нет данных';
	}
	if (!$row2['maritalStatus']) {
		$row2['maritalStatus'] = 'нет данных';
	}
	if (!$row2['numDependants']) {
		$row2['numDependants'] = 'нет данных';
	}
	
	$out .= '
	<table class="ki-table">
		<tbody>
		<tr>
			<td width="15%"><div>Заёмщик</div></td>
			<td><div>ФИО</div>'.$row1.'</td>
			<td>
				<div>Личные данные</div>
				<p><span>Дата рождения:</span><br> '.$this->get_time($row2['birthDt'] ,'ki_out').'</p>
				<p><span>Место рождения:</span><br> '.$row2['placeOfBirth'].'</p>
				<p><span>Гражданство:</span><br> '.$row2['nationality'].'</p>
				<p><span>Пол:</span><br> '.$row2['gender'].'</p>
				<p><span>Семейное положение:</span><br> '.$row2['maritalStatus'].'</p>
				<p><span>Кол-во иждивенцев:</span><br> '.$row2['numDependants'].'</p>
			</td>
		</tr>
		</tbody>
	</table>
	<div class="section-separator"><p></p></div>';
}

// Сводка
if ($row['calc']) {
	$row1 = $row2 = '';
	
	// текущий баланс
	if (!count($row['calc']['totalCurrentBalance'][0])) {
		$arr = array();
		$arr[0] = $row['calc']['totalCurrentBalance'];
		$row['calc']['totalCurrentBalance'] = $arr;
	}
	$cur_ballance = array();
	foreach ($row['calc']['totalCurrentBalance'] as $k => $v) {
		$cur_ballance[] = $v['Code'].' '.$v['Value'];
	}
	$cur_ballance = implode('<br>', $cur_ballance);
	
	// просрочено
	if (!count($row['calc']['totalPastDueBalance'][0])) {
		$arr = array();
		$arr[0] = $row['calc']['totalPastDueBalance'];
		$row['calc']['totalPastDueBalance'] = $arr;
	}
	$pastdue_ballance = array();
	foreach ($row['calc']['totalPastDueBalance'] as $k => $v) {
		$pastdue_ballance[] = $v['Code'].' '.$v['Value'];
	}
	$pastdue_ballance = implode('<br>', $pastdue_ballance);
	
	// задолженность
	if (!count($row['calc']['totalOutstandingBalance'][0])) {
		$arr = array();
		$arr[0] = $row['calc']['totalOutstandingBalance'];
		$row['calc']['totalOutstandingBalance'] = $arr;
	}
	$outstanding_ballance = array();
	foreach ($row['calc']['totalOutstandingBalance'] as $k => $v) {
		$outstanding_ballance[] = $v['Code'].' '.$v['Value'];
	}
	$outstanding_ballance = implode('<br>', $outstanding_ballance);
	
	// Ежемес.платежи
	if (!count($row['calc']['totalScheduledPaymnts'][0])) {
		$arr = array();
		$arr[0] = $row['calc']['totalScheduledPaymnts'];
		$row['calc']['totalScheduledPaymnts'] = $arr;
	}
	$sc_payments = array();
	foreach ($row['calc']['totalScheduledPaymnts'] as $k => $v) {
		$sc_payments[] = $v['Code'].' '.$v['Value'];
	}
	$sc_payments = implode('<br>', $sc_payments);
	
	// Кредитный лимит
	if (!count($row['calc']['totalHighCredit'][0])) {
		$arr = array();
		$arr[0] = $row['calc']['totalHighCredit'];
		$row['calc']['totalHighCredit'] = $arr;
	}
	$h_credit = array();
	foreach ($row['calc']['totalHighCredit'] as $k => $v) {
		$h_credit[] = $v['Code'].' '.$v['Value'];
	}
	$h_credit = implode('<br>', $h_credit);
	
	$row1 = '
	<table>
		<tbody>
		<tr>
			<td><div>Тип счета</div></td>
			<td><div>Счета</div></td>
			<td><div>Договоры</div></td>
			<td><div>Баланс</div></td>
			<td><div>Открыт</div></td>
		</tr>
		<tr>
			<td>Все счета</td>
			<td>
				<span>Всего:</span> '.$row['calc']['totalAccts'].'<br>
				<span>Негативных:</span> '.$row['calc']['negativeRating'].'<br>
				<span>Открытых:</span> '.$row['calc']['totalActiveBalanceAccounts'].'<br>
				<span class="text-red">Оспаривается:</span> '.$row['calc']['totalDisputedAccounts'].'
			</td>
			<td>
				<span>Кредитн.лимит:</span> '.$h_credit.'<br>
				<span>Ежемес.платежи:</span> '.$sc_payments.'
			</td>
			<td>
				<span>Текущий:</span> '.$cur_ballance.'<br>
				<span>Задолженность:</span> '.$outstanding_ballance.'<br>
				<span>Просрочено:</span> '.$pastdue_ballance.'
			</td>
			<td>
				<span>Последний:</span> '.$this->get_time($row['calc']['mostRecentAcc'], 'ki_out').'<br>
				<span>Первый:</span> '.$this->get_time($row['calc']['oldest'], 'ki_out').'
			</td>
		</tr>
		</tbody>
	</table>';
	
	
	$row2 = '<table>
		<tbody>
		<tr>
			<td><div>Тип запроса</div></td>
			<td><div>Всего</div></td>
			<td><div>За послед.30 дней</div></td>
			<td><div>Последние (24 месяца)</div></td>
			<td><div>Последний</div></td>
		</tr>
		<tr>
			<td>Все запросы</td>
			<td>'.$row['calc']['totalInquiries'].'</td>
			<td>'.$row['calc']['recentInquiries'].'</td>
			<td>'.$row['calc']['collectionsInquiries'].'</td>
			<td>'.$row['calc']['mostRecentInqText'].'</td>
		</tr>
		</tbody>
	</table>';
	
	
	$out .= '
	<table class="ki-table">
		<tbody>
		<tr>
			<td width="15%"><div>Сводка</div></td>
			<td>
				<div>Счета</div>
				'.$row1.'
				<div>Запросы</div>
				'.$row2.'
			</td>
		</tr>
		</tbody>
	</table>
	<div class="section-separator"><p></p></div>';
}

// Идентификация заемщика
if ($row['IdReply']) {
	$row1 = $row2 = '';
	if (!count($row['IdReply'][0])) {
		$arr = array();
		$arr[0] = $row['IdReply'];
		$row['IdReply'] = $arr;
	}
	// массив записей
	foreach ($row['IdReply'] as $key => $value) {
		$row1.='
		<tr class="noborder">
			<td colspan="4"><div>'.$value['idTypeText'].'</div></td>
		</tr>
		<tr>
			<td><div>Номер</div></td>
			<td><div>Дата выдачи</div></td>
			<td><div>Кем выдан</div></td>
			<td><div>Дата</div></td>
		</tr>
		<tr>
			<td>'.$value['seriesNumber'].' '.$value['idNum'].'</td>
			<td>'.$this->get_time($value['issueDate'], 'ki_out').'</td>
			<td>'.$value['issueAuthority'].', '.$value['issueCountry'].'</td>
			<td>'.$this->get_time($value['lastUpdatedDt'], 'ki_out').'</td>
		</tr>
		<tr class="noborder">
			<td colspan="4">&nbsp;</td>
		</tr>';
	}
	
	$row1 = '
	<table>
		<tbody>
			'.$row1.'
		</tbody>
	</table>';
	
	$out .= '
	<table class="ki-table">
		<tbody>
		<tr>
			<td width="15%"><div>Идентификация<br>заемщика</div></td>
			<td>
				'.$row1.'
			</td>
		</tr>
		</tbody>
	</table>
	<div class="section-separator"><p></p></div>';
}


// Адреса
if ($row['AddressReply']) {
	$row1 = $row2 = '';
	if (!count($row['AddressReply'][0])) {
		$arr = array();
		$arr[0] = $row['AddressReply'];
		$row['AddressReply'] = $arr;
	}
	
	foreach ($row['AddressReply'] as $key => $value) {
		foreach ($value as $k => $v) {
			if (is_array($v)) {
				$value[$k] = implode('<br>', $v);
			}
		}
		
		$full_addr = array();
		if ($value['postal']) {
			$full_addr[] = $value['postal'];
		}
		if ($value['city']) {
			$full_addr[] = $value['city'];
		}
		if ($value['street']) {
			$full_addr[] = $value['street'];
		}
		if ($value['block']) {
			$full_addr[] = $value['block'];
		}
		if ($value['building']) {
			$full_addr[] = $value['building'];
		}
		if ($value['houseNumber']) {
			$full_addr[] = $value['houseNumber'];
		}
		if ($value['apartment']) {
			$full_addr[] = $value['apartment'];
		}
		$full_addr = implode(', ', $full_addr);
		
		if ($value['countryCodeText']) {
			$value['countryCode'] = $value['countryCodeText'];
		}
		
		$row1.='
		<tr class="noborder">
			<td colspan="4"><div>'.$value['addressTypeText'].'</div></td>
		</tr>
		<tr>
			<td><div>Улица</div></td>
			<td><div>Страна</div></td>
			<td><div>Регион</div></td>
			<td><div>Область</div></td>
			<td><div>Район</div></td>
			<td><div>Дата</div></td>
		</tr>
		<tr>
			<td>'.$full_addr.'</td>
			<td>'.$value['countryCode'].'</td>
			<td>'.$value['city'].'</td>
			<td>'.$value['provText'].'</td>
			<td>'.$value['district'].'</td>
			<td>'.$this->get_time($value['lastUpdatedDt'], 'ki_out').'</td>
		</tr>
		<tr class="noborder">
			<td colspan="4">&nbsp;</td>
		</tr>';
	}
	
	$row1 = '
	<table>
		<tbody>
			'.$row1.'
		</tbody>
	</table>';
	
	$out .= '
	<table class="ki-table">
		<tbody>
		<tr>
			<td width="15%"><div>Адреса</div></td>
			<td>
				'.$row1.'
			</td>
		</tr>
		</tbody>
	</table>
	<div class="section-separator"><p></p></div>';
}


// телефоны
if ($row['PhoneReply']) {
	$row2 = $row3 = $row4 = '';
	$row1 = array();
	if (count($row['PhoneReply'][0])) {
		// массив записей
		foreach ($row['PhoneReply'] as $key => $value) {
			$row1[$value['phoneType']] .= $value['number'].'<br>';
		}
	} else {
		// одна запись
		$row1[$row['PhoneReply']['phoneType']] .= $row['PhoneReply']['number'].'<br>';
	}
	
	$out .= '
	<table class="ki-table">
		<tbody>
			<tr>
				<td width="15%"><div>Телефон</div></td>
				<td><div>Рабочий</div></td>
				<td><div>Сотовый</div></td>
				<td><div>Домашний</div></td>
				<td><div>Факс</div></td>
			</tr>
			<tr>
				<td></td>
				<td>'.$row1[1].$row1[5].'</td>
				<td>'.$row1[4].'</td>
				<td>'.$row1[2].'</td>
				<td>'.$row1[3].'</td>
			</tr>
		</tbody>
	</table>
	<div class="section-separator"><p></p></div>';
}

// Счета (это видимо и есть КИ)
if ($row['AccountReply']) {
	$row1 = $row2 = $row3 = $row4 = '';
	$n = 0;
	if (!count($row['AccountReply'][0])) {
		$arr = array();
		$arr[0] = $row['AccountReply'];
		$row['AccountReply'] = $arr;
	}
	
	$monthes = array(
		1 => 'янв',
		2 => 'фев',
		3 => 'мар',
		4 => 'апр',
		5 => 'май',
		6 => 'июн',
		7 => 'июл',
		8 => 'авг',
		9 => 'сен',
		10 => 'окт',
		11 => 'ноя',
		12 => 'дек',
	);
	
	$label_translate = array(
		'0' => 'Новый, оценка невозможна',
		'1' => 'Оплата без просрочек',
		'A' => 'Просрочка от 1 до 29 дней',
		'2' => 'Просрочка от 30 до 59 дней',
		'3' => 'Просрочка от 60 до 89 дней',
		'4' => 'Просрочка от 90 до 119 дней',
		'5' => 'Просрочка более 120 дней',
		'7' => 'Регулярн. консолидир. платежи',
		'8' => 'Взыскание оплаты залогом',
		'9' => 'Безнадёжный долг/ передано на взыскание',
		'X' => 'Нет данных'
	);
	
	// массив записей
	foreach ($row['AccountReply'] as $key => $value) {
		$class= '';
		if ($n%2 == 0) {
			$class='class="gray-bg"';
		}
		
		if (!$value['interestPaymentFrequencyText']) {
			$value['interestPaymentFrequencyText'] = $value['paymtFreqText'];
		}
		if (!$value['collateral2Text']) {
			$value['collateral2Text'] = 'нет данных';
		}
		if (!$value['interestPaymentDueDate']) {
			$value['interestPaymentDueDate'] = $value['paymentDueDate'];
		}
		
		if ($value['paymtPatStartDt']) {
			$month = explode('+', $value['paymtPatStartDt']);
			$month = $month[0];
			list($year, $month, $day) = explode('-', $month);
			$month = (int)$month;
		} else {
			$month = '';
		}
		
		$paymtPat = '';
		if (isset($value['paymtPat'])) {
			foreach (str_split($value['paymtPat']) as $letter) {
				$letter2 = $letter;
				if ($letter == 'X') { $letter2 = '-'; }
				$paymtPat .='
				<div class="paymtPat-label">
					<span class="paymtPat fancy-tip paymtPat'.$letter.'" title="'.$label_translate[$letter].'">'.$letter2.'</span>
					<p><span>'.$monthes[$month].'</span></p>
				</div>';
				if ($month) {
					$month--;
					if ($month < 1) {
						$month = 12;
					}
				}
			}
		} else {
			$paymtPat = '
			<div class="paymtPat-label">
				<span class="paymtPat paymtPatX fancy-tip" title="'.$label_translate[$letter].'">-</span>
				<p><span>'.$monthes[$month].'</span></p>
			</div>';
		}
		
		$total_exceed = $value['numDays90'] + $value['numDays60'] + $value['numDays30'];
		
		$row1 .= '
			<tr '.$class.'>
				<td>
					<div>Счет</div>
					<span>Вид:</span> '.$value['acctTypeText'].'<br>
					<span>Отношение:</span> '.$value['ownerIndicText'].'
				</td>
				<td>
					<div>Договор</div>
					<span>Размер/лимит:</span> '.$value['currencyCode'].' '.$value['creditLimit'].'<br>
					<span>Финальн.платеж:</span> '.$this->get_time($value['paymentDueDate'],'ki_out').'<br>
					<span>Финальн.платеж %%:</span> '.$this->get_time($value['interestPaymentDueDate'],'ki_out').'<br>
					<span>Выплата осн.:</span> '.$value['paymtFreqText'].'<br>
					<span>Выплата %%:</span> '.$value['interestPaymentFrequencyText'].'<br>
					<span>Обеспечение:</span> '.$value['collateral2Text'].'
				</td>
				<td>
					<div>Состояние</div>
					<span>Открыт:</span> '.$this->get_time($value['openedDt'],'ki_out').'<br>
					<span>Статус:</span> '.$value['accountRatingText'].'<br>
					<span>Дата статуса:</span> '.$this->get_time($value['reportingDt'],'ki_out').'<br> <!--сомнительно-->
					<span>Дата окончания дог-ра:</span> '.$this->get_time($value['closedDt'],'ki_out').'<br>
					<span>Последн.выплата:</span> '.$this->get_time($value['lastPaymtDt'],'ki_out').'<br>
					<span>Последн.обновление:</span> '.$this->get_time($value['lastUpdatedDt'],'ki_out').'<br>
				</td>
				<td>
					<div>Баланс</div>
					<span>Всего выплачено:</span> '.$value['currencyCode'].' '.$value['curBalanceAmt'].'<br>
					<span>Задолженность:</span> '.$value['currencyCode'].' '.$value['amtOutstanding'].'<br>
					<span>Просрочено:</span> '.$value['currencyCode'].' '.$value['amtPastDue'].'<br>
					<span>След.платеж:</span> '.$value['currencyCode'].' '.$value['termsAmt'].'<br>
				</td>
			</tr>
			<tr '.$class.'>
				<td width="15%">
					<div>Просроч.платежей<br>('.$total_exceed.' Просроч. платежей)</div> <!--сомнительно-->
				</td>
				<td colspan="3">
					<div>Своевременность платежей (за '.$value['monthsReviewed'].' мес, последний - слева)</div>
				</td>
			</tr>
			<tr '.$class.'>
				<td width="15%">
					<span>Просрочек от 30 до 59 дн.:</span>'.$value['numDays30'].'<br>
					<span>Просрочек от 60 до 89 дн.:</span>'.$value['numDays60'].'<br>
					<span>Просрочек более, чем на 90 дн.:</span>'.$value['numDays90'].'<br>
				</td>
				<td colspan="3">
					'.$paymtPat.'
				</td>
			</tr>';
		$n++;
	}
	
	$out .= '
	<table class="ki-table">
		<tbody>
			<tr>
				<td width="15%"><div>Счета</div></td>
				<td>
					<table class="no-bg">
						<tbody>
							<tr>
								<td colspan="4"><div>Расшифровка своевременности платежей</div></td>
							</tr>
							<tr>
								<td>
									<span class="paymtPat paymtPat0">0</span>&nbsp;'.$label_translate['0'].'<br><br>
									<span class="paymtPat paymtPat1">1</span>&nbsp;'.$label_translate['1'].'<br><br>
									<span class="paymtPat paymtPatA">A</span>&nbsp;'.$label_translate['A'].'<br><br>
								</td>
								<td>
									<span class="paymtPat paymtPat2">2</span>&nbsp;'.$label_translate['2'].'<br><br>
									<span class="paymtPat paymtPat3">3</span>&nbsp;'.$label_translate['3'].'<br><br>
									<span class="paymtPat paymtPat4">4</span>&nbsp;'.$label_translate['4'].'<br><br>
								</td>
								<td>
									<span class="paymtPat paymtPat5">5</span>&nbsp;'.$label_translate['5'].'<br><br>
									<span class="paymtPat paymtPat7">7</span>&nbsp;'.$label_translate['7'].'<br><br>
									<span class="paymtPat paymtPat8">8</span>&nbsp;'.$label_translate['8'].'<br><br>
								</td>
								<td>
									<span class="paymtPat paymtPat9">9</span>&nbsp;'.$label_translate['9'].'<br><br>
									<span class="paymtPat paymtPatX">-</span>&nbsp;'.$label_translate['X'].'<br><br>
								</td>
							</tr>
						</tbody>
					</table>
					<table class="no-bg">
						<tbody>
							'.$row1.'
						</tbody>
					</table>
				</td>
			</tr>
		</tbody>
	</table>
	<div class="section-separator"><p></p></div>';
}

// Запросы
if ($row['InquiryReply']) {
	$row1 = '';
	
	if (!count($row['InquiryReply'][0])) {
		$arr = array();
		$arr[0] = $row['InquiryReply'];
		$row['InquiryReply'] = $arr;
	}
	
	foreach ($row['InquiryReply'] as $key => $value) {
		$row1.='
		<tr>
			<td>'.$value['inquiryPeriod'].'</td>
			<td>'.$value['inqPurposeText'].'</td>
			<td>'.$value['currencyCode'].' '.$value['inqAmount'].'</td>
		</tr>';
	}
	
	$out .= '
	<table class="ki-table">
		<tbody>
			<tr>
				<td width="15%"><div>Запросы</div></td>
				<td>
					<table>
						<tbody>
							<tr>
								<td><div>Период</div></td>
								<td><div>Цель запроса</div></td>
								<td><div>Объем кредита</div></td>
							</tr>
							'.$row1.'
						</tbody>
					</table>
				</td>
			</tr>
		</tbody>
	</table>
	<div class="section-separator"><p></p></div>';
}

// ищим ошибку
$row = $row_all;
if ($row['err']) {
	if (!count($row['err'][0])) {
		$arr = array();
		$arr[0] = $row['err'];
		$row['err'] = $arr;
	}
	
	foreach ($row['err'] as $key => $value) {
		$out .= '
		<table class="ki-table">
			<tbody>
				<tr>
					<td width="15%"><div>Ошибка</div></td>
					<td>
						<p>Код: '.$value['ctErr']['Code'].'</p>
						<p>Текст: '.$value['ctErr']['Text'].'</p>
					</td>
				</tr>
			</tbody>
		</table>';
	}
}

$row = $row_all['report'];
/* Вывод всех полей поочерёдно, в чистом виде
------------------------------------------------------------------------------*/
	$content.='';
	// даты
	if ($row['SubjectReply']) {
		$content.= '
		<tr>
			<td width="30%">ID этого запроса</td>
			<td>'.$this->ki_translate_vals($row['inqControlNum']).'</td>
		</tr>';
		$content.= '
		<tr>
			<td width="30%">Дата создания КИ в НБКИ</td>
			<td>'.$this->ki_translate_vals($row['SubjectReply']['fileSinceDt']).'</td>
		</tr>';
		$content.= '
		<tr>
			<td width="30%">Дата последнего обновления КИ в НБКИ</td>
			<td>'.$this->ki_translate_vals($row['SubjectReply']['lastUpdatedDt']).'</td>
		</tr>';
		$content.= '
		<tr>
			<td width="30%">Дата запроса КИ нашей системой</td>
			<td>'.$this->ki_translate_vals($data['date']).'</td>
		</tr>';
		$content.= '
		<tr>
			<td width="30%">Запросил сотрудник</td>
			<td>'.$this->ki_translate_vals($data['worker']).'</td>
		</tr>';
		
		$content = '
		<div class="section-separator"><span>Общая информация</span><p></p></div>
		<table>
			<tbody>
				'.$content.'
			</tbody>
		</table>';
	}
	$unique_id = $row['inqControlNum'];
	unset($row['inqControlNum']);
	unset($row['SubjectReply']);
	$content ='<br><br>
	<a id="ki-details-'.$unique_id.'" class="spoiler-btn" href="#">Просмотреть всю полученную информацию</a>
	<div class="spoiler ki-details-'.$unique_id.'">'.$content;
	
	// общая информация о заемщике
	$content.= $this->render_ki_section('Заёмщик', $row['PersonReply']);
	unset($row['PersonReply']);
	// паспорт
	$content.= $this->render_ki_section('Паспортные данные', $row['IdReply']);
	unset($row['IdReply']);
	// адреса
	$content.= $this->render_ki_section('Адреса', $row['AddressReply'], 'addressTypeText');
	unset($row['AddressReply']);
	// телефон
	$content.= $this->render_ki_section('Телефоны', $row['PhoneReply']);
	unset($row['PhoneReply']);
	// кредитная история
	$content.= $this->render_ki_section('Кредитная история', $row['AccountReply']);
	unset($row['AccountReply']);
	// общая статистика запросов к КИ за последнее время
	$term = '';
	if ($row['calc']['mostRecentInqText']) {
		 $term = ' в срок: '.$row['calc']['mostRecentInqText'];
	}
	$content.= $this->render_ki_section('Статистика по этому КИ'.$term, $row['calc']);
	unset($row['calc']);
	
	// статистика по каждому запросу к КИ за последнее время
	if (count($row['InquiryReply'])) {
		$content.='<a href="#" class="spoiler-btn" id="InquiryReply-sp-'.$unique_id.'">Подробнее</a>
		<div class="InquiryReply-sp'.$unique_id.' spoiler">';
		
			foreach($row['InquiryReply'] as $key => $value) {
				
				$content.='<div class="white-wrap">';
					$content.='<p>ID запроса: '.$value['inqControlNum'].'</p>';
					$content.='<p>Дата запроса КИ: '.$value['inquiryPeriod'].'</p>';
					$content.='<p>Цель обращения заёмщика: '.$value['inqPurposeText'].'</p>';
					$content.='<p>Сумма обращения: '.$value['inqAmount'].' '.$value['currencyCode'].'</p>';
				$content.='</div>';
			}
		
		$content.='</div>';
	}
	unset($row['InquiryReply']);
	
	// неразобранные поля
	$content.= $this->render_ki_section('Неразобранные поля', $row);
	$content.= '</div>';
	
	$out = '<div class="section-separator"><p><span></span></p></div><div class="ki-master-wrap">'.$out.'</div>';
	$xml_sourse = '<br><br><a href="#" class="spoiler-btn" id="ki-sourse'.$unique_id.'">Оригинал ответа от сервера</a><br><br>
	<div class="ki-sourse'.$unique_id.' spoiler white-wrap">'.$xml_sourse.'</div>';
	
	$row_all = '<a href="#" class="spoiler-btn" id="ki-sourse2'.$unique_id.'">Ответ в виде массива</a><br><br>
	<div class="ki-sourse2'.$unique_id.' spoiler white-wrap"><pre>'.var_export($row_all, true).'</pre></div><br>';
	
	return $out.$content.$xml_sourse.$row_all;
}

// определяем массив значений перед нами или одна запись, и обрабатывает соответсвующим способом
function render_ki_section($title, $row, $arr_prefix='') {
	$content = '';
	if (count($row)) {
		if (count($row[0])) {
			// массив записей
			$n = 0;
			foreach ($row as $arr) {
				$n++;
				if (!$arr_prefix) {
					$content.= '<h3>Запись #'.$n.'</h3>';
				} else {
					$content.= '<h3>'.$arr[$arr_prefix].'</h3>';
				}
				$content.= $this->wrap_ki_section($arr);
			}
		} else {
			// одна запись
			$content.= $this->wrap_ki_section($row);
		}
	}
	if ($content) {
		$content = '<div class="section-separator"><span>'.$title.'</span><p></p></div>'.$content;
	}
	
	return $content;
}

// оборачивает значения в HTML
function wrap_ki_section($row, $second_pass=false) {
	if (!$second_pass) {
		$debug_row = array(
			'serialNum'=> $row['serialNum'],
			'fileSinceDt'=> $row['fileSinceDt'],
			'lastUpdatedDt'=> $row['lastUpdatedDt'],
			'dataValidity'=> $row['dataValidity'],
			'freezeFlag'=> $row['freezeFlag'],
			'suppressFlag'=> $row['suppressFlag'],
			'disputedStatus'=> $row['disputedStatus'],
			'disputedRemarks'=> $row['disputedRemarks']
		);
		unset($row['serialNum']);
		unset($row['fileSinceDt']);
		unset($row['lastUpdatedDt']);
		unset($row['dataValidity']);
		unset($row['freezeFlag']);
		unset($row['suppressFlag']);
		unset($row['disputedStatus']);
		unset($row['disputedRemarks']);
	}
	
	foreach ($row as $field => $value) {
		$field = trim($field);
		if ($row[$field]) {
			$content.= '
			<tr>
				<td width="30%">'.$this->ki_translate($field).'</td>
				<td>'.$this->ki_translate_vals($value).'</td>
			</tr>';
		}
	}
	if ($content) {
		$content = '
		<table>
			<tbody>
				'.$content.'
			</tbody>
		</table>';
		
		$debug = '';
		if (!$second_pass) {
			$debug.= $this->wrap_ki_section($debug_row, true);
		}
		
		if ($debug) {
			$content.= '<a href="#" class="spoiler-btn" id="ki-spoiler-'.$debug_row['serialNum'].'">Отладочная информация</a>
			<div class="spoiler ki-spoiler-'.$debug_row['serialNum'].'">
				<table>
					<tbody>
						'.$debug.'
					</tbody>
				</table>
			</div>';
		}
	}
	
	return $content;
}

function check_req($req) {
	$no_value = array();
	foreach ($req as $key) {
		if (!$_POST[$key]) {
			$no_value[] = $key;
		}
	}
	return $no_value;
}

function ki_translate($field) {
	return $field;
	$translate = array(
	'serialNum' => 'ID секции КИ',
	'fileSinceDt' => 'Дата создания секции КИ',
	'lastUpdatedDt' => 'Дата последнего обновления секции КИ',
	'name1' => 'Фамилия',
	'first' => 'Имя',
	'paternal' => 'Отчество',
	'genderText' => 'Пол',
	'birthDt' => 'Дата рождения',
	'placeOfBirth' => 'Место рождения',
	'idTypeText' => 'Тип документа',
	'seriesNumber' => 'Серия',
	'idNum' => 'Номер',
	'issueCountry' => 'Место выдачи',
	'issueDate' => 'Дата выдачи',
	'issueAuthority' => 'Кем выдан',
	'countryCode' => 'Код страны',
	'city' => 'Город',
	'district' => 'Район',
	'block' => 'Квартал',
	'street' => 'Улица',
	'building' => 'Строение',
	'houseNumber' => 'Номер дома',
	'apartment' => 'Квартира',
	'phoneTypeText' => 'Тип',
	'number' => 'Номер',
	'ownerIndicText' => 'На кого брался',
	'acctTypeText' => 'Тип займа',
	'openedDt' => 'Дата выдачи займа',
	'lastPaymtDt' => 'Дата последнего платежа',
	'closedDt' => 'Дата закрытия',
	'reportingDt' => 'Date on which this data is valid',
	'collateral2Text' => 'Залог',
	'creditLimit' => 'This field contains either the maximum amount of credit authorized or the original loan amount, depending on the account type',
	'currencyCode' => 'Валюта',
	'curBalanceAmt' => 'The balance of the account as of the date in the Date Reported field',
	'amtPastDue' => 'The amount past due as of the date in the Date Reported field',
	'termsFrequency' => 'Частота выплат',
	'guarantorIndicatorCode' => 'Гарант',
	'guaranteeVolumeCode' => 'Объём гаранта',
	'bankGuaranteeIndicatorCode' => 'Гарантия банка',
	'bankGuaranteeVolumeCode' => 'Объём гарантии банка',
	'creditTotalAmt' => 'This field contains information about overall value of credit in accordance with credit contract',
	'termsAmt' => 'Сумма следующей выплаты',
	'amtOutstanding' => 'The total amount outstanding, including any interests or penalties accrued as of the date reported field. This amount is a positive whole number',
	'monthsReviewed' => 'The total number of months reviewed in the Payment Pattern',
	'numDays30' => 'Number of months where payment was between 30 and 59 days late within the payment pattern (past 180 months)',
	'numDays60' => 'Number of months where payment was between 60 and 89 days late within the payment pattern (past 180 months)',
	'numDays90' => 'Number of months where payment was more than 90 days late within the payment pattern',
	'paymtPat' => 'This is the Manner Of Payment (MOP) as of the Date Reported field. This field is used to build a history of the borrower’s payment habits on the account. Typically, if the borrower is past due on a payment, an amount also exists in the Amount Past Due field',
	'paymtPatStartDt' => 'The most recent reported month for the account',
	'paymtFreqText' => 'Description of the value in the termsFrequency tag',
	'accountRatingText' => 'The rating assigned to an account',
	'accountRatingDate' => 'Date for the Account Rating event',
	'paymentDueDate' => 'The date on which an installment or interest falls due',
	'interestPaymentDueDate' => 'The date when the payment of the interest falls due for the account',
	'interestPaymentFrequencyText' => 'The frequency of payments to be made for the interest amount',
	'totalAccts' => 'Total number of all trade accounts, both open and closed, for the subject',
	'totalActiveBalanceAccounts' => 'The number of accounts in active state',
	'negativeRating' => 'The number of accounts both open and closed, with a negative rating. The negative rating is incremented 1 for each account with an account status of more than 30 days overdue within the payment pattern (past 180 months)',
	'totalOfficialInfo' => 'The total number of official information in the credit history',
	'totalLegalItems' => 'The total number of legal items in the credit history',
	'totalBankruptcies' => 'The total number of bankruptcies in the credit history',
	'totalInquiries' => 'The number of times the credit history has been requested by a credit provider',
	'recentInquiries' => 'The number of times the credit history has been requested by a credit provider within the past 30 days',
	'collectionsInquiries' => 'The number of times the credit history has been requested by a credit provider within the past 24 months',
	'reportIssueDateTime' => 'The date and time of the inquiry request',
	'totalDisputedAccounts' => 'The total number of disputed trade accounts in the credit history',
	'totalDisputedBankruptcy' => 'The total number of disputed bankruptcies in the credit history',
	'totalDisputedLegalItem' => 'The total number of disputed legal items in the credit history',
	'totalDisputedOfficialInfo' => 'The total number of disputed official information segments in the credit history',
	'totalIPRecords' => 'The total number of IP Records in credit history',
	'totalRejectedIPRecords' => 'The total number of rejected information part records in the credit history',
	'totalApprovedIPRecords' => 'The total number of approved information part segments in the credit history',
	'totalDefaultFlagIPRecords' => 'The total number of default flag information part segments in the credit history',
	'totalDisputedIPItem' => 'The total number of disputed informational part segments in the credit history',
	'inqControlNum' => 'ID запроса',
	'inquiryPeriod' => 'Дата запроса КИ',
	'inqPurposeText' => 'Цель обращения заёмщика',
	'inqAmount' => 'Сумма обращения'
	);
	
	if ($translate[$field]) {
		$field = $translate[$field];
	}
	
	return $field;
}

function ki_translate_vals($field) {
	// для массивов
	if (is_array($field)) {
		if (!count($field)) {
			$field = '';
		} else {
			$field = '<pre>'.var_export($field, true).'</pre>';
		}
		return $field;
	}
	
	return $field;
	// для строк
	switch ($field) {
		case 'serialNum':
			
		break;
		case 'fileSinceDt':
			
		break;
		case 'lastUpdatedDt':
			
		break;
	}
	return $field;
}


// $row в $post не подставлять!
function check_nbki_fields($loan_id, $post = array()) {
	$req = array(
		'paspnom',
		'paspser',
		'paspdate',
		'paspkem',
		'pasp_location',
		'sname',
		'fname',
		'tname',
		'sex',
		'birth',
		'birthaddr',
		'city1',
		'street_type1',
		'street1',
		'house_number1',
		'apartment1'
	);
	
	$query2 = mysql_query("SELECT `user_id` FROM `wp_loans` WHERE id='$loan_id'");
	@$row2 = mysql_fetch_array($query2, MYSQL_ASSOC);
	$this->check_mysql_error();
	if (!$row2['user_id']) {
		$this->wrap_json_error('Займ не найден.');
	}
	
	$query3 = mysql_query("SELECT * FROM `wp_loans_users` WHERE id={$row2['user_id']}");
	@$row3 = mysql_fetch_array($query3, MYSQL_ASSOC);
	$this->check_mysql_error();
	if (!$row3['id']) {
		$this->wrap_json_error('Заёмщик не найден.');
	}
	
	if ($row3['address'] != 1) {
		$req[] = 'city2';
		$req[] = 'street_type2';
		$req[] = 'street2';
		$req[] = 'house_number2';
		$req[] = 'apartment2';
	}
	
	if (count($post)) {
		$row = $post;
	} else {
		$row = $row3;
	}
	
	$no_value = array();
	foreach ($req as $key) {
		if (!$row[$key]) {
			$no_value[] = $key;
		}
	}
	
	return $no_value;
}

function recount_remind_date($loan_id) {
	$row = $this->get_main_row($loan_id);
	if ($row['remind']) {
		// части вычислений находится в get_buh_stats, если займ не просрочен
		$stats = $this->get_buh_stats($row);
		
		if ($stats['expired']) {
			// когда нужно было напомнить об окончании займа
			list($stats['remind_date'], $trash) = explode(' ', $stats['last_doc_back_date']);
			$stats['remind_date'] = date_create($stats['remind_date']);
			date_add($stats['remind_date'], date_interval_create_from_date_string('-'.($row['remind']-1).' days'));
			$stats['remind_date'] = date_format($stats['remind_date'], 'Y-m-d');
		}
	}
	if (!$stats['remind_date']) {
		$stats['remind_date'] = '0000-00-00';
	}
	mysql_query("UPDATE `wp_loans` SET `remind_date`='{$stats['remind_date']}' WHERE `id`={$loan_id}");
}

} // end of class      

?>
<<<TEST>>>
<<<IGNORE>>>
