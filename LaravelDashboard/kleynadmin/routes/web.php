<?php

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| contains the "web" middleware group. Now create something great!
|
*/

Route::get('/', function () {
    return view('welcome');
});
Route::any('/', 'Adminpanel@Register')->name('Register');
Route::post('sendAPICall', 'Adminpanel@sendAPICall')->name('sendAPICall');
Route::any('dashboard', 'AdminPanel@dashboard')->name('dashboard');
Route::match(['get', 'post'], 'users', 'AdminPanel@users')->name('users');
Route::post('CreateEmployee','AdminPanel@InsertEmployee');
Route::post('DeleteEmployee','AdminPanel@DeleteEmployee');
Route::any('roles', 'AdminPanel@roles')->name('roles');
Route::any('logs', 'AdminPanel@logs')->name('logs');
Route::any('api', 'AdminPanel@api')->name('api');