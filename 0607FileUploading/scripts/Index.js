$(function () {
    $("#uploadInput").change(function () {
        $("#uploadFile").val($(this).val());
    });
    $('#datetimepicker1').datetimepicker();

    $("#generateLink").click(function () {
        var id = $("#imageCount").data('image-id');
        var exp = $("#date").val();
        var url = window.location.host;
        $.post("/home/generatelink", { id: id, exp: exp }, function (result) {
            $("#message").text(url + "/home" + result);
        });
        $("#success-alert").show();
        $("#generate-div").hide();
        $("#shareLink").hide();
    });

    $("#loginBtn").click(function () {
        $('.modal').modal();
        $("#signinModalBtn").show();
        $("#registerModalBtn").hide();
        $("#modalTitle").text('Login');
        $("#userlabel").text('Username');
        $("#modalForm").attr('action', '/home/signin');
        $('#username').removeClass("register");
    });

    $("#registerBtn").click(function () {
        $('.modal').modal();
        $("#signinModalBtn").hide();
        $("#registerModalBtn").show();
        $("#modalTitle").text('Register');
        $("#modalForm").attr('action', '/home/register');
        $('#username').addClass("register");

    });

    //$(".register").keyup(function () {
    //    alert("hello");
    //    var username = $('[name="username"]').val();
    //    $.post("/home/register", { id: id, exp: exp }, function (result) {
    //        $("#message").text(url + "/home" + result);
    //    });
    //    $.get("/home/checkifavailable", { username: username }, function (result) {
    //        alert(username);
    //        if (result === null) {
    //            $("#userlabel").css('color', 'green');
    //            $("#userlabel").text('This username is availble!');
    //        } else {
    //            $("#userlabel").css('color', 'red');
    //            $("#userlabel").text('Username unavailable, choose another');
    //        }
    //    });

    //});
});