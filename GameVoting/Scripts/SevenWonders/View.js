var debug;
$(document).ready(function () {
    var splitUrl = window.location.href.split('/');
    var gameArg = splitUrl[splitUrl.length - 1];  //get the last element, which should be the id

    $.ajax({
        url: '/SevenWonders/GetGame',
        dataType: 'json',
        data: {
            gameId: gameArg
        },
        success: function (data) {
            if (data.Success) {
                var model = new GameModel(data.Payload);
                debug = model;
                ko.applyBindings(model);
            }
        }
    });
});