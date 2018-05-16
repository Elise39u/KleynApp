<?php
/**
 * Created by PhpStorm.
 * User: DustDustin
 * Date: 02-Mar-18
 * Time: 1:29 PM
 */

class Index extends CI_Controller
{
	public function __construct()
	{
		parent::__construct();
		$this->load->helper('url_helper');
		$this->load->library('session');
		$this->load->helper('form');
	}

	public function Index(){
		$this->load->view("template/Header");
		$this->load->view("Login");
		$this->load->view("template/Footer");
}
}