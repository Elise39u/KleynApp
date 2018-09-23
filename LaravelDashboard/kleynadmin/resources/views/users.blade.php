@extends('layouts.layout')
@section('content')
<?php
/**
 * Created by PhpStorm.
 * User: DustDustin
 * Date: 20-Mar-18
 * Time: 8:36 AM
 */

    $title = "Users";
    $small = "List of users"

?>
@if ($errors->any())
<div class="alert alert-danger">
        @foreach ($errors->all() as $error)
          {{ $error }}<br>
        @endforeach
</div>
@endif
    <button type="button" data-toggle="modal" data-target="#AddEmployee" class="btn btn-success">
        <span class="fa fa-plus"></span> Add Employee
    </button>
<button type="button" data-toggle="modal" data-target="#AddGuest" class="btn btn-success">
    <span class="fa fa-plus"></span> Create Guest account
</button>
    <p>Employees</p>
    <table id="employee_table" class="table table-hover">
        <tr>
            <th>Username:</th>
            <th>Email:</th>
            <th>Role:</th>
            <th>Is frozen: </th>
            <th>Actions:</th>
        </tr>
       <?php foreach ($users as $employee) { ?>
        <tr>
            <td><?php echo $employee->username; ?></td>
            <td><?php echo $employee->email; ?></td>
            <td><?php echo $employee->roleId; ?></td>
            <?php if( $employee->IsFrozen == "1"){ ?>
                <td class="red">Yes</td>
            <?php } else{ ?>
                <td>No</td>
            <?php }?>
            <td>
                <button type="button" data-toggle="modal" data-employee="<?php echo $employee->username; ?>" data-target="#DeleteEmployee" class="btn btn-danger"><i class="fa fa-trash"></i></button>
                <button type="button" data-toggle="modal" data-target="#EditEmployee" class="btn btn-success"><i class="fa fa-edit"></i></button>
	            <?php if( $employee->IsFrozen == "1"){ ?>
                   <button type="button" title="Activate account" data-toggle="modal" data-target="#UnfreezeEmployee" class="btn btn-info"><i class="fa fa-play"></i></button>
	            <?php } else{ ?>
                 <button type="button" title="Deactivate account" data-toggle="modal" data-target="#FreezeEmployee" class="btn btn-info"><i class="fa fa-pause"></i></button>
	            <?php }?>
            </td>
        </tr>
        <?php } ?>
    </table>


<!-- Modals (pop ups) -->

    <div class="modal fade" id="AddEmployee" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Create new employee account.</h4>
                </div>
                <form action = "/CreateEmployee" method = "post">
                    <input type = "hidden" name = "_token" value = "<?php echo csrf_token(); ?>">
                    <div class="modal-body">
                        <p>Please fill in the forms to register a employee account.<br> The password will be generated and mailed to the email address</p>
                        <label for="username">Username: </label>
                        <input class="form-control" type="Text" placeholder="Username" name="username"/>
                        <label for="username">Email of employee: </label>
                        <input class="form-control" type="Text" placeholder="email" name="email"/>
                        <label for="roleid">Role of employee:</label>
                        <select class="form-control" id="roleid" name="roleid">
                            <option value=" ">select role....</option>
                            <option value="1">Administrator</option>
                            <option value="2">Terrainworker</option>
                            <option value="3">Salesman</option>
                            <option value="4">maintaince</option>
                        </select>
                    </div>
                    <div class="modal-footer">
                        <button type="submit" class="btn btn-kleynorange">Create acccount</button>
                    </div>
                </form>
            </div>

        </div>
    </div>

    <div class="modal fade" id="AddGuest" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Create Guest account.</h4>
                </div>
                <div class="modal-body">
                    <p>Please fill in the forms to register a guest account.<br> The password will be generated and mailed to the email address</p>
                    <label for="username">Username: </label>
                    <input class="form-control" type="Text" placeholder="Username" name="username"/>
                    <label for="email">Email of guest: </label>
                    <input class="form-control" type="Text" placeholder="email" name="email"/>
                    <label for="expires">Date of expire: </label>
                    <input class="form-control" type="date" placeholder="Date of expire" name="expires"/>
                </div>
                <div class="modal-footer">
                    <button type="submit" class="btn btn-kleynorange">Create acccount</button>
                </div>
            </div>

        </div>
    </div>
    <div class="modal fade" id="EditEmployee" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Edit Employee.</h4>
                </div>
                <div class="modal-body">
                    <p>You are now editing {Username}.</p>
                    <label for="username">Usename:</label>
                    <input type="Text" class="form-control" placeholder="Username" name="username"/>
                    <label for="email">Email of employee:</label>
                    <input type="Text" class="form-control" placeholder="email" name="email"/>
                    <label for="roleid">Role of Employee</label>
                    <select class="form-control" id="roleid" name="roleid">
                        <option value=" ">select role....</option>
                        <option value="1">Administrator</option>
                        <option value="2">Terrainworker</option>
                        <option value="3">Salesman</option>
                        <option value="4">maintaince</option>
                    </select>
                </div>
                <div class="modal-footer">
                    <button type="submit" class="btn btn-kleynorange">Save</button>
                </div>
            </div>

        </div>
    </div>
    <div class="modal fade" id="FreezeEmployee" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Deactivate Employee.</h4>
                </div>
                <div class="modal-body">
                    <p> Are you sure you want to deactivate this employee?</p>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-danger">No! Don't deactivate this employee.</button>
                    <button type="button" class="btn btn-success">Yes! Deactivate this employee.</button>
                </div>
            </div>

        </div>
    </div>
    <div class="modal fade" id="UnfreezeEmployee" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Activate Employee.</h4>
                </div>
                <div class="modal-body">
                    <p> Are you sure you want to activate this employee?</p>
                </div>
                <div class="modal-footer">
                    <button type="button" data-dismiss="modal" class="btn btn-danger">No! Don't activate this employee.</button>
                    <button type="button" class="btn btn-success">Yes! activate this employee.</button>
                </div>
            </div>

        </div>
    </div>
    <div class="modal fade" id="DeleteEmployee" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete Employee.</h4>
                </div>
                <form action = "/DeleteEmployee" method = "post">
                    <input type = "hidden" name = "_token" value = "<?php echo csrf_token(); ?>">
                    <div class="modal-body">
                        <p>Are you sure you want to delete <span style="font-weight: bold;" id="EmployeeTxt"></span>?</p>
                        <input id="employee" type="hidden" name="username" value="" readonly/>
                    </div>
                    <div class="modal-footer">
                        <button type="button" data-dismiss="modal" class="btn btn-danger">No! Don't delete this employee.</button>
                        <button type="submit" class="btn btn-success">Yes! Delete this employee.</button>
                    </div>
                </form>
            </div>

        </div>
    </div>


@endsection
