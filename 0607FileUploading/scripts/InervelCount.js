$(function () {
    var id = $("#imageCount").data("image-id");
    setInterval(function () {
        $.get("/home/getcount", { id: id }, function (result) {
            $("#imageCount").text("View Count: " + result);
        });
    }, 3000);

    $("#likeBtn").click(function () {
        var id = $(this).data("person-id");
        $.post("/home/like", { id: id }, function () {
            
        });
        $('#likeBtn').prop('disabled', true);
        $('#likeBtn').text(" Liked");
    });
});
