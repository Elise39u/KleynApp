<?php

namespace App\Http\Controllers;
use Couchbase\UserSettings;
use Illuminate\Support\Facades\DB;
use Illuminate\Http\Request;
use App\Users;
use GuzzleHttp\Exception\GuzzleException;
use GuzzleHttp\Client;

class AdminPanel extends Controller
{

    private $client = null;

    public function Users(Request $request)
	{
		$users = Users::all();
		return view('users', ['users' => $users]);
	}

	public function  Register() {
        return view('register');
    }

    public function sendAPICall() {
	    if(isset($_POST['submit'])) {
            //$first_name = $_POST['first_name'] ;
            $username = $_POST['username'];
            $password = $_POST['password'];
            //$password_confirm = $_POST['password_confirmation'];
            $this->CallApi($username, $password);
        }
    }

    public function CallApi($username, $password)
    {
        $data = ["username" => $username, "password" => $password];
        $json_data = json_encode($data);
        $client = new Client(['base_uri' =>  'http://l2storm.dvc-icta.nl']);
        $r = $client->request('POST', '/api/login', [
            'headers' => [
                'Accept' => 'application/json',
                'Content-Type' => 'application/json'
            ],
            'body' => $json_data
          ]);
        echo $r->getBody()->getContents();
    }

	Public function InsertEmployee(Request $request)
	{
		$this->validate(
			$request, [
			'username' => 'required|unique:users',
			'email' => 'required|unique:users',
			'roleid' => 'required'
		]
		);
		function random_password()
		{
			$alphabet = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890';
			$password = array();
			$alpha_length = strlen($alphabet) - 1;
			for ($i = 0; $i < 8; $i++) {
				$n = rand(0, $alpha_length);
				$password[] = $alphabet[$n];
			}
			return implode($password);
		}

		$username = $request->input('username');
		$email = $request->input('email');
		$roleid = $request->input('roleid');

		$passwordmade = random_password();
		$passwordhash = password_hash($passwordmade, PASSWORD_BCRYPT);

		$data = array(
			'username' => $username,
			'email' => $email,
			'roleId' => $roleid,
			'password' => $passwordhash,
			'IsFrozen' => 0
		);
		DB::table('users')->insert($data);
		return back();
	}

	public function DeleteEmployee(Request $request)
	{
		$username = $request->input('username');
		DB::table('users')->where('username', $username)->delete();
		return back();
	}
	public function FreezeAccount(Request $request){
		$userState = $request->input('userState');
		$userRole = $request->input('userRole');
		if($userState = 1) {
			if ($userRole = 'Guest') {

			} else {

			}
		}elseif($userState = 0){
			if ($userRole = 'Guest') {

			} else {

			}
		}
	}
}
