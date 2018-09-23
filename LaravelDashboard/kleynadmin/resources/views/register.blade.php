@extends('layouts.layout')
@section('content')
<?php
$title = "Register";
$small = "Register for api"
    ?>
<style>
    body{
        /* Safari 4-5, Chrome 1-9 */
        background: -webkit-gradient(radial, center center, 0, center center, 460, from(#1a82f7), to(#2F2727));

        /* Safari 5.1+, Chrome 10+ */
        background: -webkit-radial-gradient(circle, #1a82f7, #2F2727);

        /* Firefox 3.6+ */
        background: -moz-radial-gradient(circle, #1a82f7, #2F2727);

        /* IE 10 */
        background: -ms-radial-gradient(circle, #1a82f7, #2F2727);
        height:600px;
    }

    .centered-form{
        margin-top: 60px;
    }

    .centered-form .panel{
        background: rgba(255, 255, 255, 0.8);
        box-shadow: rgba(0, 0, 0, 0.3) 20px 20px 20px;
    }

    label.label-floatlabel {
        font-weight: bold;
        color: #46b8da;
        font-size: 11px;
    }
</style>
<div class="container">
    <div class="row centered-form">
        <div class="col-xs-12 col-sm-8 col-md-4 col-sm-offset-2 col-md-offset-4">
            <form method="post" action="/sendAPICall">
                <input type = "hidden" name = "_token" value = "<?php echo csrf_token(); ?>">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Please login for the api  <small>Else it wont like you</small></h3>
                </div>
                <div class="panel-body">
                    <form role="form">
                        <div class="form-group">
                            <input type="text" name="username" id="username" class="form-control input-sm" placeholder="username">
                        </div>

                                <div class="form-group">
                                    <input type="password" name="password" id="password" class="form-control input-sm" placeholder="Password">
                                </div>

                        <input type="submit" name="submit" value="Login" class="btn btn-info btn-block">

                    </form>
                </div>
            </div>
            </form>
        </div>
    </div>
</div>

@endsection